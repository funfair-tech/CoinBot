using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using CoinBot.Core;
using CoinBot.Core.Extensions;
using CoinBot.Core.JsonConverters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

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
        private readonly JsonSerializerOptions _serializerSettings;

        public GateIoClient(IHttpClientFactory httpClientFactory, ILogger<GateIoClient> logger, CurrencyManager currencyManager)
            : base(httpClientFactory, HTTP_CLIENT_NAME, logger)
        {
            this._currencyManager = currencyManager ?? throw new ArgumentNullException(nameof(currencyManager));

            this._serializerSettings = new JsonSerializerOptions
                                       {
                                           IgnoreNullValues = true,
                                           PropertyNameCaseInsensitive = false,
                                           PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                                           Converters = {new DecimalAsStringConverter()}
                                       };
        }

        /// <summary>
        ///     The Exchange name.
        /// </summary>
        public string Name => @"Gate.io";

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<MarketSummaryDto>> GetAsync()
        {
            try
            {
                IReadOnlyList<GateIoTicker> tickers = await this.GetTickersAsync();

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

        private MarketSummaryDto? CreateMarketSummaryDto(GateIoTicker marketSummary)
        {
            Currency? baseCurrency = this._currencyManager.Get(marketSummary.Pair.Substring(startIndex: 0, marketSummary.Pair.IndexOf(PAIR_SEPARATOR)));

            if (baseCurrency == null)
            {
                return null;
            }

            Currency? marketCurrency = this._currencyManager.Get(marketSummary.Pair.Substring(marketSummary.Pair.IndexOf(PAIR_SEPARATOR) + 1));

            if (marketCurrency == null)
            {
                return null;
            }

            return new MarketSummaryDto(market: this.Name,
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
        private async Task<IReadOnlyList<GateIoTicker>> GetTickersAsync()
        {
            HttpClient httpClient = this.CreateHttpClient();

            using (HttpResponseMessage response = await httpClient.GetAsync(new Uri(uriString: "tickers", UriKind.Relative)))
            {
                response.EnsureSuccessStatusCode();

                string json = await response.Content.ReadAsStringAsync();

                try
                {
                    Dictionary<string, GateIoTicker> items = JsonSerializer.Deserialize<Dictionary<string, GateIoTicker>>(json, this._serializerSettings);

                    foreach (KeyValuePair<string, GateIoTicker> item in items)
                    {
                        item.Value.Pair = item.Key;
                    }

                    return items.Values.ToArray();
                }
                catch (Exception exception)
                {
                    this.Logger.LogError(new EventId(exception.HResult), exception, message: "Could not deserialise");

                    return Array.Empty<GateIoTicker>();
                }
            }
        }

        public static void Register(IServiceCollection services)
        {
            services.AddSingleton<IMarketClient, GateIoClient>();

            AddHttpClientFactorySupport(services, HTTP_CLIENT_NAME, Endpoint);
        }
    }
}