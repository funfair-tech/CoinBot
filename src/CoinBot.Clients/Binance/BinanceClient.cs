using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CoinBot.Core;
using CoinBot.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

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
                IReadOnlyList<BinanceProduct> products = await this.GetProductsAsync();

                return products.Select(selector: this.CreateMarketSummaryDto)
                               .RemoveNulls()
                               .ToList();
            }
            catch (Exception exception)
            {
                EventId eventId = new EventId(exception.HResult);
                this.Logger.LogError(eventId, exception, exception.Message);

                throw;
            }
        }

        private MarketSummaryDto? CreateMarketSummaryDto(BinanceProduct product)
        {
            Currency? baseCurrency = this._currencyManager.Get(product.BaseAsset);

            if (baseCurrency == null)
            {
                return null;
            }

            Currency? marketCurrency = this._currencyManager.Get(product.QuoteAsset);

            if (marketCurrency == null)
            {
                return null;
            }

            return new MarketSummaryDto(market: "Binance",
                                        baseCurrency: baseCurrency,
                                        marketCurrency: marketCurrency,
                                        volume: product.Volume,
                                        last: product.PrevClose,
                                        lastUpdated: null);
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
        private async Task<IReadOnlyList<BinanceProduct>> GetProductsAsync()
        {
            HttpClient httpClient = this.CreateHttpClient();

            using (HttpResponseMessage response = await httpClient.GetAsync(new Uri(uriString: "product", UriKind.Relative)))
            {
                response.EnsureSuccessStatusCode();

                string json = await response.Content.ReadAsStringAsync();

                Wrapper? packet = JsonConvert.DeserializeObject<Wrapper>(json, this._serializerSettings);

                return ((IReadOnlyList<BinanceProduct>?) packet?.Data) ?? Array.Empty<BinanceProduct>();

                // WAS:
#if OLD
                       JObject jObject = JObject.Parse(json);

                return JsonConvert.DeserializeObject<List<BinanceProduct>>(jObject[propertyName: "data"]
                                                                               .ToString(),
                                                                           this._serializerSettings);
#endif
            }
        }

        [SuppressMessage(category: "Microsoft.Performance", checkId: "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Used as data packet")]
        private sealed class Wrapper
        {
            [JsonProperty("data")]

            // ReSharper disable once RedundantDefaultMemberInitializer
            public List<BinanceProduct> Data { get; set; } = default!;
        }
    }
}