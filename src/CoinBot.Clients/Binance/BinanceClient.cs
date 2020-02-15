using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CoinBot.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CoinBot.Clients.Binance
{
    public sealed class BinanceClient : CoinClientBase, IMarketClient
    {
        private const string HTTP_CLIENT_NAME = @"Binance";

        /// <summary>
        ///     The <see cref="Uri" /> of the CoinMarketCap endpoint.
        /// </summary>
        private static readonly Uri Endpoint = new Uri(uriString: "https://www.binance.com/exchange/public/", UriKind.Absolute);

        /// <summary>
        ///     The <see cref="CurrencyManager" />.
        /// </summary>
        private readonly CurrencyManager _currencyManager;

        /// <summary>
        ///     The <see cref="JsonSerializerSettings" />.
        /// </summary>
        private readonly JsonSerializerSettings _serializerSettings;

        public BinanceClient(IHttpClientFactory httpClientFactory, ILogger<BinanceClient> logger, CurrencyManager currencyManager)
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

        /// <summary>
        ///     The Exchange name.
        /// </summary>
        public string Name => @"Binance";

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<MarketSummaryDto>> GetAsync()
        {
            try
            {
                List<BinanceProduct> products = await this.GetProductsAsync();

                return products.Select(selector: p => new MarketSummaryDto
                                                      {
                                                          BaseCurrrency = this._currencyManager.Get(p.BaseAsset),
                                                          MarketCurrency = this._currencyManager.Get(p.QuoteAsset),
                                                          Market = "Binance",
                                                          Volume = p.Volume,
                                                          Last = p.PrevClose
                                                      })
                               .ToList();
            }
            catch (Exception e)
            {
                EventId eventId = new EventId(e.HResult);
                this.Logger.LogError(eventId, e, e.Message);

                throw;
            }
        }

        public static void Register(IServiceCollection services)
        {
            services.AddSingleton<IMarketClient, BinanceClient>();

            AddHttpClientFactorySupport(services, HTTP_CLIENT_NAME, Endpoint);
        }

        /// <summary>
        ///     Get the market summaries.
        /// </summary>
        /// <returns></returns>
        private async Task<List<BinanceProduct>> GetProductsAsync()
        {
            HttpClient httpClient = this.CreateHttpClient();

            using (HttpResponseMessage response = await httpClient.GetAsync(new Uri(uriString: "product", UriKind.Relative)))
            {
                string json = await response.Content.ReadAsStringAsync();
                JObject jObject = JObject.Parse(json);

                return JsonConvert.DeserializeObject<List<BinanceProduct>>(jObject[propertyName: "data"]
                                                                               .ToString(),
                                                                           this._serializerSettings);
            }
        }
    }
}