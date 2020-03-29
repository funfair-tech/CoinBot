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
using CoinBot.Core.JsonConverters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CoinBot.Clients.Kraken
{
    public sealed class KrakenClient : CoinClientBase, IMarketClient
    {
        private const string HTTP_CLIENT_NAME = @"Kraken";

        /// <summary>
        ///     The <see cref="Uri" /> of the CoinMarketCap endpoint.
        /// </summary>
        private static readonly Uri Endpoint = new Uri(uriString: "https://api.kraken.com/0/public/", UriKind.Absolute);

        /// <summary>
        ///     The <see cref="CurrencyManager" />.
        /// </summary>
        private readonly CurrencyManager _currencyManager;

        /// <summary>
        ///     The <see cref="JsonSerializerOptions" />.
        /// </summary>
        private readonly JsonSerializerOptions _serializerSettings;

        public KrakenClient(IHttpClientFactory httpClientFactory, ILogger<KrakenClient> logger, CurrencyManager currencyManager)
            : base(httpClientFactory, HTTP_CLIENT_NAME, logger)
        {
            this._currencyManager = currencyManager ?? throw new ArgumentNullException(nameof(currencyManager));

            this._serializerSettings = new JsonSerializerOptions
                                       {
                                           IgnoreNullValues = true,
                                           PropertyNameCaseInsensitive = false,
                                           PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                                           Converters = {new DecimalAsStringConverter()}
                                       };
        }

        /// <summary>
        ///     The Exchange name.
        /// </summary>
        public string Name => "Kraken";

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<MarketSummaryDto>> GetAsync()
        {
            try
            {
                IReadOnlyList<KrakenAsset> assets = await this.GetAssetsAsync();
                IReadOnlyList<KrakenPair> pairs = await this.GetPairsAsync();

                static bool IsValid(KrakenPair pair)
                {
                    // todo: can't get kraken details on these markets
                    return !pair.PairId.EndsWith(value: ".d", StringComparison.Ordinal);
                }

                KrakenTicker?[] tickers = await Task.WhenAll(pairs.Where(IsValid)
                                                                  .Select(this.GetTickerAsync));

                return tickers.RemoveNulls()
                              .Select(selector: m => this.CreateMarketSummaryDto(assets, m))
                              .RemoveNulls()
                              .ToList();
            }
            catch (Exception e)
            {
                this.Logger.LogError(new EventId(e.HResult), e, e.Message);

                throw;
            }
        }

        private MarketSummaryDto? CreateMarketSummaryDto(IReadOnlyList<KrakenAsset> assets, KrakenTicker ticker)
        {
            string? baseCurrencySymbol = FindCurrency(assets, ticker.BaseCurrency);

            if (baseCurrencySymbol == null)
            {
                return null;
            }

            string? marketCurrencySymbol = FindCurrency(assets, ticker.QuoteCurrency);

            if (marketCurrencySymbol == null)
            {
                return null;
            }

            Currency? baseCurrency = this._currencyManager.Get(baseCurrencySymbol);

            if (baseCurrency == null)
            {
                return null;
            }

            Currency? marketCurrency = this._currencyManager.Get(marketCurrencySymbol);

            if (marketCurrency == null)
            {
                return null;
            }

            return new MarketSummaryDto(market: this.Name,
                                        baseCurrency: baseCurrency,
                                        marketCurrency: marketCurrency,
                                        volume: ticker.Volume[1],
                                        last: ticker.Last[0],
                                        lastUpdated: null);
        }

        private static string? FindCurrency(IReadOnlyList<KrakenAsset> assets, string search)
        {
            KrakenAsset? found = assets.FirstOrDefault(predicate: a => StringComparer.InvariantCultureIgnoreCase.Equals(a.Id, search));

            if (found == null)
            {
                return null;
            }

            return NormalizeCurrency(found.Altname);
        }

        private static string NormalizeCurrency(string currencySymbol)
        {
            // Workaround for kraken

            if (currencySymbol.Equals(value: "xbt", StringComparison.OrdinalIgnoreCase))
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

            using (HttpResponseMessage response = await httpClient.GetAsync(new Uri(uriString: "Assets", UriKind.Relative)))
            {
                response.EnsureSuccessStatusCode();

                string json = await response.Content.ReadAsStringAsync();

                try
                {
                    KrakenAssetResultWrapper items = JsonSerializer.Deserialize<KrakenAssetResultWrapper>(json, this._serializerSettings);

                    if (items.Result != null)
                    {
                        foreach (KeyValuePair<string, KrakenAsset> item in items.Result)
                        {
                            item.Value.Id = item.Key;
                        }
                    }

                    return items.Result?.Values.ToArray() ?? Array.Empty<KrakenAsset>();
                }
                catch (Exception exception)
                {
                    this.Logger.LogError(new EventId(exception.HResult), exception, message: "Failed to deserialise");

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

            using (HttpResponseMessage response = await httpClient.GetAsync(new Uri(uriString: "AssetPairs", UriKind.Relative)))
            {
                response.EnsureSuccessStatusCode();

                string json = await response.Content.ReadAsStringAsync();

                try
                {
                    KrakenPairWrapper items = JsonSerializer.Deserialize<KrakenPairWrapper>(json, this._serializerSettings);

                    if (items.Result == null)
                    {
                        return Array.Empty<KrakenPair>();
                    }

                    foreach (KeyValuePair<string, KrakenPair> item in items.Result)
                    {
                        item.Value.PairId = item.Key;
                    }

                    return items.Result.Values.ToArray();
                }
                catch (Exception exception)
                {
                    this.Logger.LogError(new EventId(exception.HResult), exception, message: "Failed to deserialize");

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

                using (HttpResponseMessage response = await httpClient.GetAsync(new Uri($"Ticker?pair={pair.PairId}", UriKind.Relative)))
                {
                    response.EnsureSuccessStatusCode();

                    string json = await response.Content.ReadAsStringAsync();

                    try
                    {
                        KrakenTickerWrapper item = JsonSerializer.Deserialize<KrakenTickerWrapper>(json, this._serializerSettings);

                        if (item.Result == null)
                        {
                            return null;
                        }

                        item.Result.BaseCurrency = pair.BaseCurrency;
                        item.Result.QuoteCurrency = pair.QuoteCurrency;

                        return item.Result;
                    }
                    catch (Exception exception)
                    {
                        this.Logger.LogError(new EventId(exception.HResult), exception, message: "Failed to deserialize");

                        return null;
                    }
                }
            }
            catch (Exception exception)
            {
                this.Logger.LogError(new EventId(exception.HResult), exception, $"Failed to retrieve {pair.BaseCurrency}/{pair.QuoteCurrency}: {exception.Message}");

                return null;
            }
        }

        public static void Register(IServiceCollection services)
        {
            services.AddSingleton<IMarketClient, KrakenClient>();

            AddHttpClientFactorySupport(services, HTTP_CLIENT_NAME, Endpoint);
        }

        [SuppressMessage(category: "Microsoft.Performance", checkId: "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Used as data packet")]
        private sealed class KrakenPairWrapper
        {
            [JsonPropertyName(name: @"result")]
            public Dictionary<string, KrakenPair>? Result { get; set; }
        }

        [SuppressMessage(category: "Microsoft.Performance", checkId: "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Used as data packet")]
        private sealed class KrakenTickerWrapper
        {
            [JsonPropertyName(name: @"result")]
            public KrakenTicker? Result { get; set; }
        }

        [SuppressMessage(category: "Microsoft.Performance", checkId: "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Used as data packet")]
        private sealed class KrakenAssetResultWrapper
        {
            [JsonPropertyName(name: @"result")]
            public Dictionary<string, KrakenAsset>? Result { get; set; }
        }
    }
}