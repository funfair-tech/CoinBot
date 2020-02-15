using System;
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

                return tickers.Select(selector: this.CreateMarketSummaryDto)
                              .ToList();
            }
            catch (Exception e)
            {
                this.Logger.LogError(new EventId(e.HResult), e, e.Message);

                throw;
            }
        }

        private MarketSummaryDto CreateMarketSummaryDto(GdaxTicker t)
        {
            Currency? baseCurrency = this._currencyManager.Get(t.ProductId.Substring(startIndex: 0, t.ProductId.IndexOf(value: '-')));
            Currency? marketCurrency = this._currencyManager.Get(t.ProductId.Substring(t.ProductId.IndexOf(value: '-') + 1));

            return new MarketSummaryDto(market: "GDAX", baseCurrency: baseCurrency, marketCurrency: marketCurrency, volume: t.Volume, last: t.Price, lastUpdated: t.Time);
        }

        /// <summary>
        ///     Get the products.
        /// </summary>
        /// <returns></returns>
        private async Task<GdaxTicker?> GetTickerAsync(string productId)
        {
            HttpClient httpClient = this.CreateHttpClient();

            using (HttpResponseMessage response = await httpClient.GetAsync(new Uri($"products/{productId}/ticker", UriKind.Relative)))
            {
                response.EnsureSuccessStatusCode();

                GdaxTicker? ticker = JsonConvert.DeserializeObject<GdaxTicker>(await response.Content.ReadAsStringAsync(), this._serializerSettings);

                if (ticker == null)
                {
                    return null;
                }

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
                response.EnsureSuccessStatusCode();

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