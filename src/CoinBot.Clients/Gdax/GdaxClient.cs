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
    public class GdaxClient : IMarketClient
    {
        /// <summary>
        ///     The <see cref="CurrencyManager" />.
        /// </summary>
        private readonly CurrencyManager _currencyManager;

        /// <summary>
        ///     The <see cref="Uri" /> of the CoinMarketCap endpoint.
        /// </summary>
        private readonly Uri _endpoint = new Uri(uriString: "https://api.gdax.com/", UriKind.Absolute);

        /// <summary>
        ///     The <see cref="HttpClient" />.
        /// </summary>
        private readonly HttpClient _httpClient;

        /// <summary>
        ///     The <see cref="ILogger" />.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        ///     The <see cref="JsonSerializerSettings" />.
        /// </summary>
        private readonly JsonSerializerSettings _serializerSettings;

        public GdaxClient(ILogger logger, CurrencyManager currencyManager)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._currencyManager = currencyManager ?? throw new ArgumentNullException(nameof(currencyManager));

            this._httpClient = new HttpClient {BaseAddress = this._endpoint};
            this._httpClient.DefaultRequestHeaders.Add(name: "User-Agent", value: "CoinBot");

            this._serializerSettings = new JsonSerializerSettings
                                       {
                                           Error = (sender, args) =>
                                                   {
                                                       Exception ex = args.ErrorContext.Error.GetBaseException();
                                                       this._logger.LogError(new EventId(args.ErrorContext.Error.HResult), ex, ex.Message);
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
                List<GdaxTicker> tickers = new List<GdaxTicker>();

                foreach (GdaxProduct product in products)
                {
                    tickers.Add(await this.GetTickerAsync(product.Id));
                }

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
                this._logger.LogError(new EventId(e.HResult), e, e.Message);

                throw;
            }
        }

        /// <summary>
        ///     Get the products.
        /// </summary>
        /// <returns></returns>
        private async Task<GdaxTicker> GetTickerAsync(string productId)
        {
            using (HttpResponseMessage response = await this._httpClient.GetAsync(new Uri($"products/{productId}/ticker", UriKind.Relative)))
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
            using (HttpResponseMessage response = await this._httpClient.GetAsync(new Uri(uriString: "products/", UriKind.Relative)))
            {
                return JsonConvert.DeserializeObject<List<GdaxProduct>>(await response.Content.ReadAsStringAsync(), this._serializerSettings);
            }
        }

        public static void Register(IServiceCollection services)
        {
            // TODO: Add HTTP Client Factory
            services.AddSingleton<IMarketClient, GdaxClient>();
        }
    }
}