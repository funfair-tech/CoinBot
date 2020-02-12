using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CoinBot.Core;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CoinBot.Clients.Poloniex
{
    public class PoloniexClient : IMarketClient
    {
        /// <summary>
        ///     The <see cref="CurrencyManager" />.
        /// </summary>
        private readonly CurrencyManager _currencyManager;

        /// <summary>
        ///     The <see cref="Uri" /> of the CoinMarketCap endpoint.
        /// </summary>
        private readonly Uri _endpoint = new Uri(uriString: "https://poloniex.com/", UriKind.Absolute);

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

        public PoloniexClient(ILogger logger, CurrencyManager currencyManager)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._currencyManager = currencyManager ?? throw new ArgumentNullException(nameof(currencyManager));
            this._httpClient = new HttpClient {BaseAddress = this._endpoint};

            this._serializerSettings = new JsonSerializerSettings
                                       {
                                           Error = (sender, args) =>
                                                   {
                                                       Exception ex = args.ErrorContext.Error.GetBaseException();
                                                       this._logger.LogError(new EventId(args.ErrorContext.Error.HResult), ex, ex.Message);
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
                List<PoloniexTicker> tickers = await this.GetTickers();

                return tickers.Select(selector: t => new MarketSummaryDto
                                                     {
                                                         BaseCurrrency = this._currencyManager.Get(t.Pair.Substring(startIndex: 0, t.Pair.IndexOf(value: '_'))),
                                                         MarketCurrency = this._currencyManager.Get(t.Pair.Substring(t.Pair.IndexOf(value: '_') + 1)),
                                                         Market = "Poloniex",
                                                         Volume = t.BaseVolume,
                                                         Last = t.Last
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
        ///     Get the market summaries.
        /// </summary>
        /// <returns></returns>
        private async Task<List<PoloniexTicker>> GetTickers()
        {
            using (HttpResponseMessage response = await this._httpClient.GetAsync(new Uri(uriString: "public?command=returnTicker", UriKind.Relative)))
            {
                string json = await response.Content.ReadAsStringAsync();
                JObject jResponse = JObject.Parse(json);
                List<PoloniexTicker> tickers = new List<PoloniexTicker>();

                foreach (KeyValuePair<string, JToken> jToken in jResponse)
                {
                    JObject obj = JObject.Parse(jToken.Value.ToString());
                    PoloniexTicker ticker = JsonConvert.DeserializeObject<PoloniexTicker>(obj.ToString(), this._serializerSettings);
                    ticker.Pair = jToken.Key;
                    tickers.Add(ticker);
                }

                return tickers;
            }
        }
    }
}