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

namespace CoinBot.Clients.Poloniex
{
    public sealed class PoloniexClient : CoinClientBase, IMarketClient
    {
        private const string HTTP_CLIENT_NAME = @"Poloniex";

        /// <summary>
        ///     The <see cref="Uri" /> of the CoinMarketCap endpoint.
        /// </summary>
        private static readonly Uri Endpoint = new(uriString: "https://poloniex.com/", uriKind: UriKind.Absolute);

        /// <summary>
        ///     The <see cref="JsonSerializerOptions" />.
        /// </summary>
        private readonly JsonSerializerOptions _serializerSettings;

        public PoloniexClient(IHttpClientFactory httpClientFactory, ILogger<PoloniexClient> logger)
            : base(httpClientFactory: httpClientFactory, clientName: HTTP_CLIENT_NAME, logger: logger)
        {
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
        public string Name => "Poloniex";

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<MarketSummaryDto>> GetAsync(ICoinBuilder builder)
        {
            try
            {
                IReadOnlyList<PoloniexTicker> tickers = await this.GetTickersAsync();

                return tickers.Select(selector: ticker => this.CreateMarketSummaryDto(ticker: ticker, builder: builder))
                              .RemoveNulls()
                              .ToList();
            }
            catch (Exception e)
            {
                this.Logger.LogError(new EventId(e.HResult), exception: e, message: e.Message);

                throw;
            }
        }

        private MarketSummaryDto? CreateMarketSummaryDto(PoloniexTicker ticker, ICoinBuilder builder)
        {
            Currency? baseCurrency = builder.Get(ticker.Pair.Substring(startIndex: 0, ticker.Pair.IndexOf(value: '_')));

            if (baseCurrency == null)
            {
                return null;
            }

            Currency? marketCurrency = builder.Get(ticker.Pair.Substring(ticker.Pair.IndexOf(value: '_') + 1));

            if (marketCurrency == null)
            {
                return null;
            }

            return new MarketSummaryDto(market: this.Name, baseCurrency: baseCurrency, marketCurrency: marketCurrency, volume: ticker.BaseVolume, last: ticker.Last, lastUpdated: null);
        }

        /// <summary>
        ///     Get the market summaries.
        /// </summary>
        /// <returns></returns>
        private async Task<IReadOnlyList<PoloniexTicker>> GetTickersAsync()
        {
            HttpClient httpClient = this.CreateHttpClient();

            using (HttpResponseMessage response = await httpClient.GetAsync(new Uri(uriString: "public?command=returnTicker", uriKind: UriKind.Relative)))
            {
                response.EnsureSuccessStatusCode();

                string json = await response.Content.ReadAsStringAsync();

                try
                {
                    Dictionary<string, PoloniexTicker> tickers = JsonSerializer.Deserialize<Dictionary<string, PoloniexTicker>>(json: json, options: this._serializerSettings);

                    foreach (KeyValuePair<string, PoloniexTicker> item in tickers)
                    {
                        item.Value.Pair = item.Key;
                    }

                    return tickers.Values.ToArray();
                }
                catch (Exception exception)
                {
                    this.Logger.LogError(new EventId(exception.HResult), exception: exception, message: "Failed to deserialize");

                    return Array.Empty<PoloniexTicker>();
                }
            }
        }

        public static void Register(IServiceCollection services)
        {
            services.AddSingleton<IMarketClient, PoloniexClient>();

            AddHttpClientFactorySupport(services: services, clientName: HTTP_CLIENT_NAME, endpoint: Endpoint);
        }
    }
}