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

namespace CoinBot.Clients.Binance;

public sealed class BinanceClient : CoinClientBase, IMarketClient
{
    private const string HTTP_CLIENT_NAME = @"Binance";

    /// <summary>
    ///     The <see cref="Uri" /> of the CoinMarketCap endpoint.
    /// </summary>
    private static readonly Uri Endpoint = new(uriString: "https://www.binance.com/exchange/public/", uriKind: UriKind.Absolute);

    /// <summary>
    ///     The <see cref="JsonSerializerOptions" />.
    /// </summary>
    private readonly JsonSerializerOptions _serializerSettings;

    public BinanceClient(IHttpClientFactory httpClientFactory, ILogger<BinanceClient> logger)
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
    public string Name => @"Binance";

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<MarketSummaryDto>> GetAsync(ICoinBuilder builder)
    {
        try
        {
            IReadOnlyList<BinanceProduct> products = await this.GetProductsAsync();

            return products.Select(selector: product => this.CreateMarketSummaryDto(product: product, builder: builder))
                           .RemoveNulls()
                           .ToList();
        }
        catch (Exception exception)
        {
            EventId eventId = new(exception.HResult);
            this.Logger.LogError(eventId: eventId, exception: exception, message: exception.Message);

            throw;
        }
    }

    private MarketSummaryDto? CreateMarketSummaryDto(BinanceProduct product, ICoinBuilder builder)
    {
        // always look at the quoted currency first as if that does not exist, then no point creating doing any more
        Currency? marketCurrency = builder.Get(product.QuoteAsset);

        if (marketCurrency == null)
        {
            return null;
        }

        Currency baseCurrency = builder.Get(symbol: product.BaseAsset, name: product.BaseAssetName);

        return new(market: this.Name, baseCurrency: baseCurrency, marketCurrency: marketCurrency, volume: product.Volume, last: product.PrevClose, lastUpdated: null);
    }

    public static void Register(IServiceCollection services)
    {
        services.AddSingleton<IMarketClient, BinanceClient>();

        AddHttpClientFactorySupport(services: services, clientName: HTTP_CLIENT_NAME, endpoint: Endpoint);
    }

    /// <summary>
    ///     Get the market summaries.
    /// </summary>
    /// <returns></returns>
    private async Task<IReadOnlyList<BinanceProduct>> GetProductsAsync()
    {
        HttpClient httpClient = this.CreateHttpClient();

        using (HttpResponseMessage response = await httpClient.GetAsync(new Uri(uriString: "product", uriKind: UriKind.Relative)))
        {
            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();

            try
            {
                Wrapper? packet = JsonSerializer.Deserialize<Wrapper>(json: json, options: this._serializerSettings);

                return packet?.Data ?? Array.Empty<BinanceProduct>();
            }
            catch (Exception exception)
            {
                this.Logger.LogCritical(new(exception.HResult), exception: exception, message: "Could not convert packet");

                return Array.Empty<BinanceProduct>();
            }
        }
    }

    [SuppressMessage(category: "Microsoft.Performance", checkId: "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Used as data packet")]
    private sealed class Wrapper
    {
        [JsonPropertyName(name: @"data")]
        [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Local", Justification = "TODO: Review")]
        public BinanceProduct[]? Data { get; set; }
    }
}