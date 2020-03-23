﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using CoinBot.Core;
using CoinBot.Core.Extensions;
using CoinBot.Core.JsonConverters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CoinBot.Clients.Gdax
{
    public sealed class GdaxClient : CoinClientBase, IMarketClient
    {
        private const string HTTP_CLIENT_NAME = @"Gdax";

        /// <summary>
        ///     The <see cref="Uri" /> of the CoinMarketCap endpoint.
        /// </summary>
        private static readonly Uri Endpoint = new Uri(uriString: "https://api.gdax.com/", UriKind.Absolute);

        /// <summary>
        ///     The <see cref="CurrencyManager" />.
        /// </summary>
        private readonly CurrencyManager _currencyManager;

        /// <summary>
        ///     The <see cref="JsonSerializerOptions" />.
        /// </summary>
        private readonly JsonSerializerOptions _serializerSettings;

        public GdaxClient(IHttpClientFactory httpClientFactory, ILogger<GdaxClient> logger, CurrencyManager currencyManager)
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

        public string Name => "GDAX";

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<MarketSummaryDto>> GetAsync()
        {
            try
            {
                IReadOnlyList<GdaxProduct> products = await this.GetProductsAsync();
                GdaxTicker[] tickers = await Task.WhenAll(products.Select(selector: product => this.GetTickerAsync(product.Id)));

                return tickers.Select(this.CreateMarketSummaryDto)
                              .RemoveNulls()
                              .ToList();
            }
            catch (Exception e)
            {
                this.Logger.LogError(new EventId(e.HResult), e, e.Message);

                throw;
            }
        }

        private MarketSummaryDto? CreateMarketSummaryDto(GdaxTicker ticker)
        {
            Currency? baseCurrency = this._currencyManager.Get(ticker.ProductId.Substring(startIndex: 0, ticker.ProductId.IndexOf(value: '-')));

            if (baseCurrency == null)
            {
                return null;
            }

            Currency? marketCurrency = this._currencyManager.Get(ticker.ProductId.Substring(ticker.ProductId.IndexOf(value: '-') + 1));

            if (marketCurrency == null)
            {
                return null;
            }

            return new MarketSummaryDto(market: this.Name,
                                        baseCurrency: baseCurrency,
                                        marketCurrency: marketCurrency,
                                        volume: ticker.Volume,
                                        last: ticker.Price,
                                        lastUpdated: ticker.Time);
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

                using (HttpResponseMessage response = await httpClient.GetAsync(new Uri($"products/{productId}/ticker", UriKind.Relative)))
                {
                    response.EnsureSuccessStatusCode();

                    string json = await response.Content.ReadAsStringAsync();

                    try
                    {
                        GdaxTicker? ticker = JsonSerializer.Deserialize<GdaxTicker>(json, this._serializerSettings);

                        ticker.ProductId = productId;

                        return ticker;
                    }
                    catch (Exception exception)
                    {
                        this.Logger.LogError(new EventId(exception.HResult), exception, message: "Failed to Deserialize");

                        return null;
                    }
                }
            }
            catch (Exception exception)
            {
                this.Logger.LogError(new EventId(exception.HResult), exception, $"Failed to retrieve {productId}: {exception.Message}");

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

            using (HttpResponseMessage response = await httpClient.GetAsync(new Uri(uriString: "products/", UriKind.Relative)))
            {
                response.EnsureSuccessStatusCode();

                string json = await response.Content.ReadAsStringAsync();

                try
                {
                    IReadOnlyList<GdaxProduct>? items = JsonSerializer.Deserialize<List<GdaxProduct>>(json, this._serializerSettings);

                    return items ?? Array.Empty<GdaxProduct>();
                }
                catch (Exception exception)
                {
                    this.Logger.LogError(new EventId(exception.HResult), exception, message: "Failed to deserialize");

                    return Array.Empty<GdaxProduct>();
                }
            }
        }

        public static void Register(IServiceCollection services)
        {
            services.AddSingleton<IMarketClient, GdaxClient>();

            AddHttpClientFactorySupport(services, HTTP_CLIENT_NAME, Endpoint);
        }
    }
}