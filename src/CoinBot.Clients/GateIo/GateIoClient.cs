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

namespace CoinBot.Clients.GateIo
{
    public sealed class GateIoClient : CoinClientBase, IMarketClient
    {
        private const string HTTP_CLIENT_NAME = @"GateIo";

        private const char PAIR_SEPARATOR = '_';

        /// <summary>
        ///     The <see cref="Uri" /> of the CoinMarketCap endpoint.
        /// </summary>
        private static readonly Uri Endpoint = new Uri(uriString: "http://data.gate.io/api2/1/", UriKind.Absolute);

        /// <summary>
        ///     The <see cref="CurrencyManager" />.
        /// </summary>
        private readonly CurrencyManager _currencyManager;

        /// <summary>
        ///     The <see cref="JsonSerializerSettings" />.
        /// </summary>
        private readonly JsonSerializerSettings _serializerSettings;

        public GateIoClient(IHttpClientFactory httpClientFactory, ILogger<GateIoClient> logger, CurrencyManager currencyManager)
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
        public string Name => "Gate.io";

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<MarketSummaryDto>> GetAsync()
        {
            try
            {
                List<GateIoTicker> tickers = await this.GetTickersAsync();

                return tickers.Select(selector: this.CreateMarketSummaryDto)
                              .ToList();
            }
            catch (Exception e)
            {
                this.Logger.LogError(new EventId(e.HResult), e, e.Message);

                throw;
            }
        }

        private MarketSummaryDto CreateMarketSummaryDto(GateIoTicker marketSummary)
        {
            Currency? baseCurrency = this._currencyManager.Get(marketSummary.Pair.Substring(startIndex: 0, marketSummary.Pair.IndexOf(PAIR_SEPARATOR)));
            Currency? marketCurrency = this._currencyManager.Get(marketSummary.Pair.Substring(marketSummary.Pair.IndexOf(PAIR_SEPARATOR) + 1));

            return new MarketSummaryDto(market: "Gate.io",
                                        baseCurrency: baseCurrency,
                                        marketCurrency: marketCurrency,
                                        volume: marketSummary.BaseVolume,
                                        last: marketSummary.Last,
                                        lastUpdated: null);
        }

        /// <summary>
        ///     Get the market summaries.
        /// </summary>
        /// <returns></returns>
        private async Task<List<GateIoTicker>> GetTickersAsync()
        {
            HttpClient httpClient = this.CreateHttpClient();

            using (HttpResponseMessage response = await httpClient.GetAsync(new Uri(uriString: "tickers", UriKind.Relative)))
            {
                response.EnsureSuccessStatusCode();

                string json = await response.Content.ReadAsStringAsync();
                JObject jResponse = JObject.Parse(json);
                List<GateIoTicker> tickers = new List<GateIoTicker>();

                foreach (KeyValuePair<string, JToken?> jToken in jResponse)
                {
                    if (jToken.Value == null)
                    {
                        continue;
                    }

                    JObject? obj = JObject.Parse(jToken.Value.ToString());

                    if (obj == null)
                    {
                        continue;
                    }

                    GateIoTicker? ticker = JsonConvert.DeserializeObject<GateIoTicker>(obj.ToString(), this._serializerSettings);

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
            services.AddSingleton<IMarketClient, GateIoClient>();

            AddHttpClientFactorySupport(services, HTTP_CLIENT_NAME, Endpoint);
        }
    }
}