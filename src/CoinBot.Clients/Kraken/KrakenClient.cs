using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CoinBot.Core;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CoinBot.Clients.Kraken
{
    public sealed class KrakenClient : IMarketClient
    {
        /// <summary>
        ///     The <see cref="CurrencyManager" />.
        /// </summary>
        private readonly CurrencyManager _currencyManager;

        /// <summary>
        ///     The <see cref="Uri" /> of the CoinMarketCap endpoint.
        /// </summary>
        private readonly Uri _endpoint = new Uri(uriString: "https://api.kraken.com/0/public/", UriKind.Absolute);

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

        public KrakenClient(ILogger logger, CurrencyManager currencyManager)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._currencyManager = currencyManager ?? throw new ArgumentNullException(nameof(currencyManager));
            this._httpClient = new HttpClient {BaseAddress = this._endpoint};

            this._serializerSettings = new JsonSerializerSettings
                                       {
                                           Error = (sender, args) =>
                                                   {
                                                       EventId eventId = new EventId(args.ErrorContext.Error.HResult);
                                                       Exception ex = args.ErrorContext.Error.GetBaseException();
                                                       this._logger.LogError(eventId, ex, ex.Message);
                                                   }
                                       };
        }

        /// <summary>
        ///     The Exchange name.
        /// </summary>
        public string Name => "Kraken";

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<MarketSummaryDto>> GetAsync()
        {
            try
            {
                List<KrakenAsset> assets = await this.GetAssetsAsync();
                List<KrakenPair> pairs = await this.GetPairsAsync();
                List<KrakenTicker> tickers = new List<KrakenTicker>();

                foreach (KrakenPair pair in pairs)
                {
                    // todo: can't get kraken details on these markets
                    if (pair.PairId.EndsWith(value: ".d", StringComparison.Ordinal))
                    {
                        continue;
                    }

                    tickers.Add(await this.GetTickerAsync(pair));
                }

                return tickers.Select(selector: m =>
                                                {
                                                    string baseCurrency = assets.Find(match: a => StringComparer.InvariantCultureIgnoreCase.Equals(a.Id, m.BaseCurrency))
                                                                                .Altname;
                                                    string quoteCurrency = assets.Find(match: a => StringComparer.InvariantCultureIgnoreCase.Equals(a.Id, m.QuoteCurrency))
                                                                                 .Altname;

                                                    // Workaround for kraken
                                                    if (baseCurrency.Equals(value: "xbt", StringComparison.OrdinalIgnoreCase))
                                                    {
                                                        baseCurrency = "btc";
                                                    }

                                                    if (quoteCurrency.Equals(value: "xbt", StringComparison.OrdinalIgnoreCase))
                                                    {
                                                        quoteCurrency = "btc";
                                                    }

                                                    return new MarketSummaryDto
                                                           {
                                                               BaseCurrrency = this._currencyManager.Get(baseCurrency),
                                                               MarketCurrency = this._currencyManager.Get(quoteCurrency),
                                                               Market = "Kraken",
                                                               Volume = m.Volume[1],
                                                               Last = m.Last[0]
                                                           };
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
        ///     Get the ticker.
        /// </summary>
        /// <returns></returns>
        private async Task<List<KrakenAsset>> GetAssetsAsync()
        {
            using (HttpResponseMessage response = await this._httpClient.GetAsync(new Uri(uriString: "Assets", UriKind.Relative)))
            {
                string json = await response.Content.ReadAsStringAsync();
                JObject jObject = JObject.Parse(json);
                List<KrakenAsset> assets = jObject.GetValue(propertyName: "result")
                                                  .Children()
                                                  .Cast<JProperty>()
                                                  .Select(selector: property =>
                                                                    {
                                                                        KrakenAsset asset =
                                                                            JsonConvert.DeserializeObject<KrakenAsset>(property.Value.ToString(), this._serializerSettings);
                                                                        asset.Id = property.Name;

                                                                        return asset;
                                                                    })
                                                  .ToList();

                return assets;
            }
        }

        /// <summary>
        ///     Get the market summaries.
        /// </summary>
        /// <returns></returns>
        private async Task<List<KrakenPair>> GetPairsAsync()
        {
            using (HttpResponseMessage response = await this._httpClient.GetAsync(new Uri(uriString: "AssetPairs", UriKind.Relative)))
            {
                string json = await response.Content.ReadAsStringAsync();
                JObject jResponse = JObject.Parse(json);
                List<KrakenPair> pairs = jResponse.GetValue(propertyName: "result")
                                                  .Children()
                                                  .Cast<JProperty>()
                                                  .Select(selector: property =>
                                                                    {
                                                                        KrakenPair pair = JsonConvert.DeserializeObject<KrakenPair>(property.Value.ToString());
                                                                        pair.PairId = property.Name;

                                                                        return pair;
                                                                    })
                                                  .ToList();

                return pairs;
            }
        }

        /// <summary>
        ///     Get the ticker.
        /// </summary>
        /// <returns></returns>
        private async Task<KrakenTicker> GetTickerAsync(KrakenPair pair)
        {
            using (HttpResponseMessage response = await this._httpClient.GetAsync(new Uri($"Ticker?pair={pair.PairId}", UriKind.Relative)))
            {
                string json = await response.Content.ReadAsStringAsync();
                JObject jObject = JObject.Parse(json);
                KrakenTicker ticker = JsonConvert.DeserializeObject<KrakenTicker>(jObject[propertyName: "result"][pair.PairId]
                                                                                      .ToString(),
                                                                                  this._serializerSettings);
                ticker.BaseCurrency = pair.BaseCurrency;
                ticker.QuoteCurrency = pair.QuoteCurrency;

                return ticker;
            }
        }
    }
}