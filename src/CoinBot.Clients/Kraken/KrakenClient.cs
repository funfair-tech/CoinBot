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

namespace CoinBot.Clients.Kraken
{
    public sealed class KrakenClient : CoinClientBase, IMarketClient
    {
        private const string HTTP_CLIENT_NAME = @"Kraken";

        /// <summary>
        ///     The <see cref="Uri" /> of the CoinMarketCap endpoint.
        /// </summary>
        private static readonly Uri Endpoint = new Uri(uriString: "https://api.kraken.com/0/public/", UriKind.Absolute);

        /// <summary>
        ///     The <see cref="CurrencyManager" />.
        /// </summary>
        private readonly CurrencyManager _currencyManager;

        /// <summary>
        ///     The <see cref="JsonSerializerSettings" />.
        /// </summary>
        private readonly JsonSerializerSettings _serializerSettings;

        public KrakenClient(IHttpClientFactory httpClientFactory, ILogger<KrakenClient> logger, CurrencyManager currencyManager)
            : base(httpClientFactory, HTTP_CLIENT_NAME, logger)
        {
            this._currencyManager = currencyManager ?? throw new ArgumentNullException(nameof(currencyManager));

            this._serializerSettings = new JsonSerializerSettings
                                       {
                                           Error = (sender, args) =>
                                                   {
                                                       EventId eventId = new EventId(args.ErrorContext.Error.HResult);
                                                       Exception ex = args.ErrorContext.Error.GetBaseException();
                                                       this.Logger.LogError(eventId, ex, ex.Message);
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
                IReadOnlyList<KrakenAsset> assets = await this.GetAssetsAsync();
                IReadOnlyList<KrakenPair> pairs = await this.GetPairsAsync();

                static bool IsValid(KrakenPair pair)
                {
                    // todo: can't get kraken details on these markets
                    return !pair.PairId.EndsWith(value: ".d", StringComparison.Ordinal);
                }

                KrakenTicker[] tickers = await Task.WhenAll(pairs.Where(IsValid)
                                                                 .Select(this.GetTickerAsync));

                return tickers.Select(selector: m => this.CreateMarketSummaryDto(assets, m))
                              .RemoveNulls()
                              .ToList();
            }
            catch (Exception e)
            {
                this.Logger.LogError(new EventId(e.HResult), e, e.Message);

                throw;
            }
        }

        private MarketSummaryDto? CreateMarketSummaryDto(IReadOnlyList<KrakenAsset> assets, KrakenTicker ticker)
        {
            string? baseCurrencySymbol = FindCurrency(assets, ticker.BaseCurrency);

            if (baseCurrencySymbol == null)
            {
                return null;
            }

            string? marketCurrencySymbol = FindCurrency(assets, ticker.QuoteCurrency);

            if (marketCurrencySymbol == null)
            {
                return null;
            }

            Currency? baseCurrency = this._currencyManager.Get(baseCurrencySymbol);

            if (baseCurrency == null)
            {
                return null;
            }

            Currency? marketCurrency = this._currencyManager.Get(marketCurrencySymbol);

            if (marketCurrency == null)
            {
                return null;
            }

            return new MarketSummaryDto(market: this.Name,
                                        baseCurrency: baseCurrency,
                                        marketCurrency: marketCurrency,
                                        volume: ticker.Volume[1],
                                        last: ticker.Last[0],
                                        lastUpdated: null);
        }

        private static string? FindCurrency(IReadOnlyList<KrakenAsset> assets, string search)
        {
            KrakenAsset? found = assets.FirstOrDefault(predicate: a => StringComparer.InvariantCultureIgnoreCase.Equals(a.Id, search));

            if (found == null)
            {
                return null;
            }

            return NormalizeCurrency(found.Altname);
        }

        private static string NormalizeCurrency(string currencySymbol)
        {
            // Workaround for kraken

            if (currencySymbol.Equals(value: "xbt", StringComparison.OrdinalIgnoreCase))
            {
                return "btc";
            }

            return currencySymbol;
        }

        /// <summary>
        ///     Get the ticker.
        /// </summary>
        /// <returns></returns>
        private async Task<IReadOnlyList<KrakenAsset>> GetAssetsAsync()
        {
            HttpClient httpClient = this.CreateHttpClient();

            using (HttpResponseMessage response = await httpClient.GetAsync(new Uri(uriString: "Assets", UriKind.Relative)))
            {
                response.EnsureSuccessStatusCode();

                string json = await response.Content.ReadAsStringAsync();
                JObject? jObject = JObject.Parse(json);

                if (jObject == null)
                {
                    return Array.Empty<KrakenAsset>();
                }

                return jObject.GetValue(propertyName: "result")
                              .RemoveNulls()
                              .Children()
                              .Cast<JProperty>()
                              .Select(selector: property =>
                                                {
                                                    KrakenAsset? asset = JsonConvert.DeserializeObject<KrakenAsset>(property.Value.ToString(), this._serializerSettings);

                                                    if (asset == null)
                                                    {
                                                        return null;
                                                    }

                                                    asset.Id = property.Name;

                                                    return asset;
                                                })
                              .RemoveNulls()
                              .ToArray();
            }
        }

        /// <summary>
        ///     Get the market summaries.
        /// </summary>
        /// <returns></returns>
        private async Task<IReadOnlyList<KrakenPair>> GetPairsAsync()
        {
            HttpClient httpClient = this.CreateHttpClient();

            using (HttpResponseMessage response = await httpClient.GetAsync(new Uri(uriString: "AssetPairs", UriKind.Relative)))
            {
                response.EnsureSuccessStatusCode();

                string json = await response.Content.ReadAsStringAsync();
                JObject? jResponse = JObject.Parse(json);

                if (jResponse == null)
                {
                    return Array.Empty<KrakenPair>();
                }

                return jResponse.GetValue(propertyName: "result")
                                .RemoveNulls()
                                .Children()
                                .Cast<JProperty>()
                                .Select(selector: property =>
                                                  {
                                                      KrakenPair? pair = JsonConvert.DeserializeObject<KrakenPair>(property.Value.ToString());

                                                      if (pair == null)
                                                      {
                                                          return null;
                                                      }

                                                      pair.PairId = property.Name;

                                                      return pair;
                                                  })
                                .RemoveNulls()
                                .ToList();
            }
        }

        /// <summary>
        ///     Get the ticker.
        /// </summary>
        /// <returns></returns>
        private async Task<KrakenTicker?> GetTickerAsync(KrakenPair pair)
        {
            HttpClient httpClient = this.CreateHttpClient();

            using (HttpResponseMessage response = await httpClient.GetAsync(new Uri($"Ticker?pair={pair.PairId}", UriKind.Relative)))
            {
                response.EnsureSuccessStatusCode();

                string json = await response.Content.ReadAsStringAsync();
                JObject? jObject = JObject.Parse(json);

                if (jObject == null)
                {
                    return null;
                }

                JToken? result = jObject[propertyName: "result"];

                if (result == null)
                {
                    return null;
                }

                JToken? pairItem = result[pair.PairId];

                if (pairItem == null)
                {
                    return null;
                }

                KrakenTicker? ticker = JsonConvert.DeserializeObject<KrakenTicker>(pairItem.ToString(), this._serializerSettings);

                if (ticker == null)
                {
                    return null;
                }

                ticker.BaseCurrency = pair.BaseCurrency;
                ticker.QuoteCurrency = pair.QuoteCurrency;

                return ticker;
            }
        }

        public static void Register(IServiceCollection services)
        {
            services.AddSingleton<IMarketClient, KrakenClient>();

            AddHttpClientFactorySupport(services, HTTP_CLIENT_NAME, Endpoint);
        }
    }
}