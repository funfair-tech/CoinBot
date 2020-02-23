using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CoinBot.Core;
using CoinBot.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CoinBot.Clients.Poloniex
{
    public sealed class PoloniexClient : CoinClientBase, IMarketClient
    {
        private const string HTTP_CLIENT_NAME = @"Poloniex";

        /// <summary>
        ///     The <see cref="Uri" /> of the CoinMarketCap endpoint.
        /// </summary>
        private static readonly Uri Endpoint = new Uri(uriString: "https://poloniex.com/", UriKind.Absolute);

        /// <summary>
        ///     The <see cref="CurrencyManager" />.
        /// </summary>
        private readonly CurrencyManager _currencyManager;

        /// <summary>
        ///     The <see cref="JsonSerializerSettings" />.
        /// </summary>
        private readonly JsonSerializerSettings _serializerSettings;

        public PoloniexClient(IHttpClientFactory httpClientFactory, ILogger<PoloniexClient> logger, CurrencyManager currencyManager)
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
        public string Name => "Poloniex";

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<MarketSummaryDto>> GetAsync()
        {
            try
            {
                List<PoloniexTicker> tickers = await this.GetTickersAsync();

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

        private MarketSummaryDto? CreateMarketSummaryDto(PoloniexTicker ticker)
        {
            Currency? baseCurrency = this._currencyManager.Get(ticker.Pair.Substring(startIndex: 0, ticker.Pair.IndexOf(value: '_')));

            if (baseCurrency == null)
            {
                return null;
            }

            Currency? marketCurrency = this._currencyManager.Get(ticker.Pair.Substring(ticker.Pair.IndexOf(value: '_') + 1));

            if (marketCurrency == null)
            {
                return null;
            }

            return new MarketSummaryDto(market: this.Name,
                                        baseCurrency: baseCurrency,
                                        marketCurrency: marketCurrency,
                                        volume: ticker.BaseVolume,
                                        last: ticker.Last,
                                        lastUpdated: null);
        }

        /// <summary>
        ///     Get the market summaries.
        /// </summary>
        /// <returns></returns>
        private async Task<List<PoloniexTicker>> GetTickersAsync()
        {
            HttpClient httpClient = this.CreateHttpClient();

            using (HttpResponseMessage response = await httpClient.GetAsync(new Uri(uriString: "public?command=returnTicker", UriKind.Relative)))
            {
                response.EnsureSuccessStatusCode();

                string json = await response.Content.ReadAsStringAsync();
                JObject jResponse = JObject.Parse(json);
                List<PoloniexTicker> tickers = new List<PoloniexTicker>();

                foreach (KeyValuePair<string, JToken?> jToken in jResponse)
                {
                    if (jToken.Value == null)
                    {
                        continue;
                    }

                    JObject obj = JObject.Parse(jToken.Value.ToString());
                    PoloniexTicker? ticker = JsonConvert.DeserializeObject<PoloniexTicker>(obj.ToString(), this._serializerSettings);

                    if (ticker != null)
                    {
                        ticker.Pair = jToken.Key;
                        tickers.Add(ticker);
                    }
                }

                return tickers;
            }
        }

        public static void Register(IServiceCollection services)
        {
            services.AddSingleton<IMarketClient, PoloniexClient>();

            AddHttpClientFactorySupport(services, HTTP_CLIENT_NAME, Endpoint);
        }
    }
}