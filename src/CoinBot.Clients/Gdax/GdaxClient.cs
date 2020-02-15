﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CoinBot.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

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
        ///     The <see cref="JsonSerializerSettings" />.
        /// </summary>
        private readonly JsonSerializerSettings _serializerSettings;

        public GdaxClient(IHttpClientFactory httpClientFactory, ILogger<GdaxClient> logger, CurrencyManager currencyManager)
            : base(httpClientFactory, HTTP_CLIENT_NAME, logger)
        {
            this._currencyManager = currencyManager ?? throw new ArgumentNullException(nameof(currencyManager));

            this._serializerSettings = new JsonSerializerSettings
                                       {
                                           Error = (sender, args) =>
                                                   {
                                                       Exception ex = args.ErrorContext.Error.GetBaseException();
                                                       this.Logger.LogError(new EventId(args.ErrorContext.Error.HResult), ex, ex.Message);
                                                   }
                                       };
        }

        public string Name => "GDAX";

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<MarketSummaryDto>> GetAsync()
        {
            try
            {
                List<GdaxProduct> products = await this.GetProductsAsync();
                GdaxTicker[] tickers = await Task.WhenAll(products.Select(product => this.GetTickerAsync(product.Id)));

                return tickers.Select(selector: t => new MarketSummaryDto
                                                     {
                                                         BaseCurrrency = this._currencyManager.Get(t.ProductId.Substring(startIndex: 0, t.ProductId.IndexOf(value: '-'))),
                                                         MarketCurrency = this._currencyManager.Get(t.ProductId.Substring(t.ProductId.IndexOf(value: '-') + 1)),
                                                         Market = "GDAX",
                                                         Volume = t.Volume,
                                                         Last = t.Price,
                                                         LastUpdated = t.Time
                                                     })
                              .ToList();
            }
            catch (Exception e)
            {
                this.Logger.LogError(new EventId(e.HResult), e, e.Message);

                throw;
            }
        }

        /// <summary>
        ///     Get the products.
        /// </summary>
        /// <returns></returns>
        private async Task<GdaxTicker> GetTickerAsync(string productId)
        {
            HttpClient httpClient = this.CreateHttpClient();

            using (HttpResponseMessage response = await httpClient.GetAsync(new Uri($"products/{productId}/ticker", UriKind.Relative)))
            {
                GdaxTicker ticker = JsonConvert.DeserializeObject<GdaxTicker>(await response.Content.ReadAsStringAsync(), this._serializerSettings);
                ticker.ProductId = productId;

                return ticker;
            }
        }

        /// <summary>
        ///     Get the products.
        /// </summary>
        /// <returns></returns>
        private async Task<List<GdaxProduct>> GetProductsAsync()
        {
            HttpClient httpClient = this.CreateHttpClient();

            using (HttpResponseMessage response = await httpClient.GetAsync(new Uri(uriString: "products/", UriKind.Relative)))
            {
                return JsonConvert.DeserializeObject<List<GdaxProduct>>(await response.Content.ReadAsStringAsync(), this._serializerSettings);
            }
        }

        public static void Register(IServiceCollection services)
        {
            services.AddSingleton<IMarketClient, GdaxClient>();

            AddHttpClientFactorySupport(services, HTTP_CLIENT_NAME, Endpoint);
        }
    }
}