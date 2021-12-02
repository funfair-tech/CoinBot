using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CoinBot.Core;
using CoinBot.Core.Extensions;
using CoinBot.Core.Helpers;
using CoinBot.Core.JsonConverters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CoinBot.Clients.Kraken;

public sealed class KrakenClient : CoinClientBase, IMarketClient
{
    private const string HTTP_CLIENT_NAME = @"Kraken";

    /// <summary>
    ///     The <see cref="Uri" /> of the CoinMarketCap endpoint.
    /// </summary>
    private static readonly Uri Endpoint = new(uriString: "https://api.kraken.com/0/public/", uriKind: UriKind.Absolute);

    /// <summary>
    ///     The <see cref="JsonSerializerOptions" />.
    /// </summary>
    private readonly JsonSerializerOptions _serializerSettings;

    public KrakenClient(IHttpClientFactory httpClientFactory, ILogger<KrakenClient> logger)
        : base(httpClientFactory: httpClientFactory, clientName: HTTP_CLIENT_NAME, logger: logger)
    {
        this._serializerSettings = new()
                                   {
                                       DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                                       PropertyNameCaseInsensitive = false,
                                       PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                                       Converters = { new DecimalAsStringConverter() }
                                   };
    }

    /// <summary>
    ///     The Exchange name.
    /// </summary>
    public string Name => "Kraken";

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<MarketSummaryDto>> GetAsync(ICoinBuilder builder)
    {
        try
        {
            IReadOnlyList<KrakenAsset> assets = await this.GetAssetsAsync();

            bool IsValid(KrakenPair pair)
            {
                // todo: can't get kraken details on these markets
                if (pair.PairId.EndsWith(value: ".d", comparisonType: StringComparison.Ordinal))
                {
                    return false;
                }

                string? bc = FindCurrency(assets: assets, search: pair.BaseCurrency);

                if (bc == null)
                {
                    return false;
                }

                string? qc = FindCurrency(assets: assets, search: pair.QuoteCurrency);

                if (qc == null)
                {
                    return false;
                }

                Currency? baseCurrency = builder.Get(bc);

                if (baseCurrency == null)
                {
                    return false;
                }

                Currency? quoteCurrency = builder.Get(qc);

                if (quoteCurrency == null)
                {
                    return false;
                }

                return true;
            }

            IReadOnlyList<KrakenPair> pairs = await this.GetPairsAsync();

            IReadOnlyList<KrakenTicker?> tickers = await Batched.WhenAllAsync(concurrent: 5,
                                                                              pairs.Where(IsValid)
                                                                                   .Select(this.GetTickerAsync));

            return tickers.RemoveNulls()
                          .Select(selector: m => this.CreateMarketSummaryDto(assets: assets, ticker: m, builder: builder))
                          .RemoveNulls()
                          .ToList();
        }
        catch (Exception e)
        {
            this.Logger.LogError(new(e.HResult), exception: e, message: e.Message);

            throw;
        }
    }

    private MarketSummaryDto? CreateMarketSummaryDto(IReadOnlyList<KrakenAsset> assets, KrakenTicker ticker, ICoinBuilder builder)
    {
        if (ticker.Last == null || ticker.Volume == null)
        {
            return null;
        }

        string? baseCurrencySymbol = FindCurrency(assets: assets, search: ticker.BaseCurrency);

        if (baseCurrencySymbol == null)
        {
            return null;
        }

        string? marketCurrencySymbol = FindCurrency(assets: assets, search: ticker.QuoteCurrency);

        if (marketCurrencySymbol == null)
        {
            return null;
        }

        // always look at the quoted currency first as if that does not exist, then no point creating doing any more
        Currency? marketCurrency = builder.Get(marketCurrencySymbol);

        if (marketCurrency == null)
        {
            return null;
        }

        Currency? baseCurrency = builder.Get(baseCurrencySymbol);

        if (baseCurrency == null)
        {
            return null;
        }

        return new(market: this.Name, baseCurrency: baseCurrency, marketCurrency: marketCurrency, volume: ticker.Volume[1], last: ticker.Last[0], lastUpdated: null);
    }

    private static string? FindCurrency(IReadOnlyList<KrakenAsset> assets, string search)
    {
        KrakenAsset? found = assets.FirstOrDefault(predicate: a => StringComparer.InvariantCultureIgnoreCase.Equals(x: a.Id, y: search));

        if (found == null)
        {
            return null;
        }

        return NormalizeCurrency(found.Altname);
    }

    private static string NormalizeCurrency(string currencySymbol)
    {
        // Workaround for kraken

        if (currencySymbol.Equals(value: "xbt", comparisonType: StringComparison.OrdinalIgnoreCase))
        {
            return "btc";
        }

        return currencySymbol;
    }

    /// <summary>
    ///     Get the ticker.
    /// </summary>
    /// <returns></returns>
    private async Task<IReadOnlyList<KrakenAsset>> GetAssetsAsync()
    {
        HttpClient httpClient = this.CreateHttpClient();

        using (HttpResponseMessage response = await httpClient.GetAsync(new Uri(uriString: "Assets", uriKind: UriKind.Relative)))
        {
            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();

            try
            {
                KrakenAssetResultWrapper? items = JsonSerializer.Deserialize<KrakenAssetResultWrapper>(json: json, options: this._serializerSettings);

                if (items?.Result != null)
                {
                    foreach ((string key, KrakenAsset value) in items.Result)
                    {
                        value.Id = key;
                    }
                }

                return items?.Result?.Values.ToArray() ?? Array.Empty<KrakenAsset>();
            }
            catch (Exception exception)
            {
                this.Logger.LogError(new(exception.HResult), exception: exception, message: "Failed to deserialise");

                return Array.Empty<KrakenAsset>();
            }
        }
    }

    /// <summary>
    ///     Get the market summaries.
    /// </summary>
    /// <returns></returns>
    private async Task<IReadOnlyList<KrakenPair>> GetPairsAsync()
    {
        HttpClient httpClient = this.CreateHttpClient();

        using (HttpResponseMessage response = await httpClient.GetAsync(new Uri(uriString: "AssetPairs", uriKind: UriKind.Relative)))
        {
            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();

            try
            {
                KrakenPairWrapper? items = JsonSerializer.Deserialize<KrakenPairWrapper>(json: json, options: this._serializerSettings);

                if (items?.Result == null)
                {
                    return Array.Empty<KrakenPair>();
                }

                foreach ((string key, KrakenPair value) in items.Result)
                {
                    value.PairId = key;
                }

                return items.Result.Values.ToArray();
            }
            catch (Exception exception)
            {
                this.Logger.LogError(new(exception.HResult), exception: exception, message: "Failed to deserialize");

                return Array.Empty<KrakenPair>();
            }
        }
    }

    /// <summary>
    ///     Get the ticker.
    /// </summary>
    /// <returns></returns>
    private async Task<KrakenTicker?> GetTickerAsync(KrakenPair pair)
    {
        try
        {
            HttpClient httpClient = this.CreateHttpClient();

            using (HttpResponseMessage response = await httpClient.GetAsync(new Uri($"Ticker?pair={pair.PairId}", uriKind: UriKind.Relative)))
            {
                response.EnsureSuccessStatusCode();

                string json = await response.Content.ReadAsStringAsync();

                try
                {
                    KrakenTickerWrapper? item = JsonSerializer.Deserialize<KrakenTickerWrapper>(json: json, options: this._serializerSettings);

                    if (item?.Result == null)
                    {
                        return null;
                    }

                    if (item.Result.TryGetValue(key: pair.PairId, out KrakenTicker? ticker))
                    {
                        ticker.BaseCurrency = pair.BaseCurrency;
                        ticker.QuoteCurrency = pair.QuoteCurrency;

                        return ticker;
                    }

                    return null;
                }
                catch (Exception exception)
                {
                    this.Logger.LogError(new(exception.HResult), exception: exception, message: "Failed to deserialize");

                    return null;
                }
            }
        }
        catch (Exception exception)
        {
            this.Logger.LogError(new(exception.HResult), exception: exception, $"Failed to retrieve {pair.BaseCurrency}/{pair.QuoteCurrency}: {exception.Message}");

            return null;
        }
    }

    public static void Register(IServiceCollection services)
    {
        services.AddSingleton<IMarketClient, KrakenClient>();

        AddHttpClientFactorySupport(services: services, clientName: HTTP_CLIENT_NAME, endpoint: Endpoint);
    }

    [SuppressMessage(category: "Microsoft.Performance", checkId: "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Used as data packet")]
    private sealed class KrakenPairWrapper
    {
        [JsonPropertyName(name: @"result")]
        [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Local", Justification = "TODO: Review")]
        public Dictionary<string, KrakenPair>? Result { get; set; }
    }

    [SuppressMessage(category: "Microsoft.Performance", checkId: "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Used as data packet")]
    private sealed class KrakenTickerWrapper
    {
        [JsonPropertyName(name: @"result")]
        [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Local", Justification = "TODO: Review")]
        public Dictionary<string, KrakenTicker>? Result { get; set; }
    }

    [SuppressMessage(category: "Microsoft.Performance", checkId: "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Used as data packet")]
    private sealed class KrakenAssetResultWrapper
    {
        [JsonPropertyName(name: @"result")]
        [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Local", Justification = "TODO: Review")]
        public Dictionary<string, KrakenAsset>? Result { get; set; }
    }
}