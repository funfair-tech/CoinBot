﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CoinBot.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CoinBot.Clients.Liqui
{
    public sealed class LiquiClient : CoinClientBase, IMarketClient
    {
        private const string HTTP_CLIENT_NAME = @"Liqui";

        /// <summary>
        ///     The pair separator character.
        /// </summary>
        private const char PAIR_SEPARATOR = '_';

        /// <summary>
        ///     The <see cref="Uri" /> of the CoinMarketCap endpoint.
        /// </summary>
        private static readonly Uri Endpoint = new Uri(uriString: "https://api.liqui.io/api/3/", UriKind.Absolute);

        /// <summary>
        ///     The <see cref="CurrencyManager" />.
        /// </summary>
        private readonly CurrencyManager _currencyManager;

        /// <summary>
        ///     The <see cref="JsonSerializerSettings" />.
        /// </summary>
        private readonly JsonSerializerSettings _serializerSettings;

        public LiquiClient(IHttpClientFactory httpClientFactory, ILogger<LiquiClient> logger, CurrencyManager currencyManager)
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
        public string Name => "Liqui";

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<MarketSummaryDto>> GetAsync()
        {
            try
            {
                IReadOnlyList<string> pairs = await this.GetPairsAsync();

                LiquiTicker[] tickers = await Task.WhenAll(pairs.Select(this.GetTickerAsync));

                return tickers.Select(this.CreateMarketSummaryDto)
                              .ToList();
            }
            catch (Exception exception)
            {
                this.Logger.LogError(new EventId(exception.HResult), exception, exception.Message);

                throw;
            }
        }

        private MarketSummaryDto CreateMarketSummaryDto(LiquiTicker m)
        {
            return new MarketSummaryDto
                   {
                       BaseCurrrency = this._currencyManager.Get(m.Pair.Substring(startIndex: 0, m.Pair.IndexOf(PAIR_SEPARATOR))),
                       MarketCurrency = this._currencyManager.Get(m.Pair.Substring(m.Pair.IndexOf(PAIR_SEPARATOR) + 1)),
                       Market = "Liqui",
                       Volume = m.Vol,
                       LastUpdated = m.Updated,
                       Last = m.Last
                   };
        }

        /// <summary>
        ///     Get the market summaries.
        /// </summary>
        /// <returns></returns>
        private async Task<IReadOnlyList<string>> GetPairsAsync()
        {
            try
            {
                HttpClient httpClient = this.CreateHttpClient();

                using (HttpResponseMessage response = await httpClient.GetAsync(new Uri(uriString: "info", UriKind.Relative)))
                {
                    response.EnsureSuccessStatusCode();

                    string json = await response.Content.ReadAsStringAsync();
                    JObject jResponse = JObject.Parse(json);

                    return jResponse.GetValue(propertyName: "pairs")
                                    .Children()
                                    .Cast<JProperty>()
                                    .Select(selector: property => property.Name)
                                    .ToList();
                }
            }
            catch (Exception exception)
            {
                this.Logger.LogError(new EventId(exception.HResult), exception, exception.Message);

                return Array.Empty<string>();
            }
        }

        /// <summary>
        ///     Get the ticker.
        /// </summary>
        /// <returns></returns>
        private async Task<LiquiTicker> GetTickerAsync(string pair)
        {
            HttpClient httpClient = this.CreateHttpClient();

            using (HttpResponseMessage response = await httpClient.GetAsync(new Uri($"ticker/{pair}", UriKind.Relative)))
            {
                response.EnsureSuccessStatusCode();

                string json = await response.Content.ReadAsStringAsync();
                JObject jObject = JObject.Parse(json);
                LiquiTicker ticker = JsonConvert.DeserializeObject<LiquiTicker>(jObject.GetValue(pair)
                                                                                       .ToString(),
                                                                                this._serializerSettings);
                ticker.Pair = pair;

                return ticker;
            }
        }

        public static void Register(IServiceCollection services)
        {
            services.AddSingleton<IMarketClient, LiquiClient>();

            AddHttpClientFactorySupport(services, HTTP_CLIENT_NAME, Endpoint);
        }
    }
}