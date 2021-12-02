using System;
using System.Collections.Generic;
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

namespace CoinBot.Clients.Gdax;

public sealed class GdaxClient : CoinClientBase, IMarketClient
{
    private const string HTTP_CLIENT_NAME = @"Gdax";

    /// <summary>
    ///     The <see cref="Uri" /> of the CoinMarketCap endpoint.
    /// </summary>
    private static readonly Uri Endpoint = new(uriString: "https://api.gdax.com/", uriKind: UriKind.Absolute);

    /// <summary>
    ///     The <see cref="JsonSerializerOptions" />.
    /// </summary>
    private readonly JsonSerializerOptions _serializerSettings;

    public GdaxClient(IHttpClientFactory httpClientFactory, ILogger<GdaxClient> logger)
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

    public string Name => "GDAX";

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<MarketSummaryDto>> GetAsync(ICoinBuilder builder)
    {
        try
        {
            IReadOnlyList<GdaxProduct> products = await this.GetProductsAsync();
            IReadOnlyList<GdaxTicker?> tickers = await Batched.WhenAllAsync(concurrent: 2, products.Select(selector: product => this.GetTickerAsync(product.Id)));

            return tickers.RemoveNulls()
                          .Select(selector: ticker => this.CreateMarketSummaryDto(ticker: ticker, builder: builder))
                          .RemoveNulls()
                          .ToList();
        }
        catch (Exception e)
        {
            this.Logger.LogError(new(e.HResult), exception: e, message: e.Message);

            throw;
        }
    }

    private MarketSummaryDto? CreateMarketSummaryDto(GdaxTicker ticker, ICoinBuilder builder)
    {
        // always look at the quoted currency first as if that does not exist, then no point creating doing any more
        Currency? marketCurrency = builder.Get(ticker.ProductId.Substring(ticker.ProductId.IndexOf(value: '-') + 1));

        if (marketCurrency == null)
        {
            return null;
        }

        Currency? baseCurrency = builder.Get(ticker.ProductId.Substring(startIndex: 0, ticker.ProductId.IndexOf(value: '-')));

        if (baseCurrency == null)
        {
            return null;
        }

        return new(market: this.Name, baseCurrency: baseCurrency, marketCurrency: marketCurrency, volume: ticker.Volume, last: ticker.Price, lastUpdated: ticker.Time);
    }

    /// <summary>
    ///     Get the products.
    /// </summary>
    /// <returns></returns>
    private async Task<GdaxTicker?> GetTickerAsync(string productId)
    {
        try
        {
            HttpClient httpClient = this.CreateHttpClient();

            using (HttpResponseMessage response = await httpClient.GetAsync(new Uri($"products/{productId}/ticker", uriKind: UriKind.Relative)))
            {
                response.EnsureSuccessStatusCode();

                string json = await response.Content.ReadAsStringAsync();

                try
                {
                    GdaxTicker? ticker = JsonSerializer.Deserialize<GdaxTicker>(json: json, options: this._serializerSettings);

                    if (ticker == null)
                    {
                        return null;
                    }

                    ticker.ProductId = productId;

                    return ticker;
                }
                catch (Exception exception)
                {
                    this.Logger.LogError(new(exception.HResult), exception: exception, message: "Failed to Deserialize");

                    return null;
                }
            }
        }
        catch (Exception exception)
        {
            this.Logger.LogError(new(exception.HResult), exception: exception, $"Failed to retrieve {productId}: {exception.Message}");

            return null;
        }
    }

    /// <summary>
    ///     Get the products.
    /// </summary>
    /// <returns></returns>
    private async Task<IReadOnlyList<GdaxProduct>> GetProductsAsync()
    {
        HttpClient httpClient = this.CreateHttpClient();

        using (HttpResponseMessage response = await httpClient.GetAsync(new Uri(uriString: "products/", uriKind: UriKind.Relative)))
        {
            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();

            try
            {
                IReadOnlyList<GdaxProduct>? items = JsonSerializer.Deserialize<List<GdaxProduct>>(json: json, options: this._serializerSettings);

                return items ?? Array.Empty<GdaxProduct>();
            }
            catch (Exception exception)
            {
                this.Logger.LogError(new(exception.HResult), exception: exception, message: "Failed to deserialize");

                return Array.Empty<GdaxProduct>();
            }
        }
    }

    public static void Register(IServiceCollection services)
    {
        services.AddSingleton<IMarketClient, GdaxClient>();

        AddHttpClientFactorySupport(services: services, clientName: HTTP_CLIENT_NAME, endpoint: Endpoint);
    }
}