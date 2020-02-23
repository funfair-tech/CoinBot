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

namespace CoinBot.Clients.Bittrex
{
    public sealed class BittrexClient : CoinClientBase, IMarketClient
    {
        private const string HTTP_CLIENT_NAME = @"Bittrex";

        /// <summary>
        ///     The <see cref="Uri" /> of the CoinMarketCap endpoint.
        /// </summary>
        private static readonly Uri Endpoint = new Uri(uriString: "https://bittrex.com/api/v1.1/public/", UriKind.Absolute);

        /// <summary>
        ///     The <see cref="CurrencyManager" />.
        /// </summary>
        private readonly CurrencyManager _currencyManager;

        /// <summary>
        ///     The <see cref="JsonSerializerSettings" />.
        /// </summary>
        private readonly JsonSerializerSettings _serializerSettings;

        public BittrexClient(IHttpClientFactory httpClientFactory, ILogger<BittrexClient> logger, CurrencyManager currencyManager)
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
        public string Name => "Bittrex";

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<MarketSummaryDto>> GetAsync()
        {
            try
            {
                IReadOnlyList<BittrexMarketSummaryDto> summaries = await this.GetMarketSummariesAsync();

                return summaries.Select(this.CreateMarketSummaryDto)
                                .RemoveNulls()
                                .ToList();
            }
            catch (Exception e)
            {
                this.Logger.LogError(new EventId(e.HResult), e, e.Message);

                throw;
            }
        }

        private MarketSummaryDto? CreateMarketSummaryDto(BittrexMarketSummaryDto marketSummary)
        {
            Currency? baseCurrency = this._currencyManager.Get(marketSummary.MarketName.Substring(startIndex: 0, marketSummary.MarketName.IndexOf(value: '-')));

            if (baseCurrency == null)
            {
                return null;
            }

            Currency? marketCurrency = this._currencyManager.Get(marketSummary.MarketName.Substring(marketSummary.MarketName.IndexOf(value: '-') + 1));

            if (marketCurrency == null)
            {
                return null;
            }

            return new MarketSummaryDto(market: this.Name,
                                        baseCurrency: baseCurrency,
                                        marketCurrency: marketCurrency,
                                        volume: marketSummary.BaseVolume,
                                        last: marketSummary.Last,
                                        lastUpdated: marketSummary.TimeStamp);
        }

        /// <summary>
        ///     Get the market summaries.
        /// </summary>
        /// <returns></returns>
        private async Task<IReadOnlyList<BittrexMarketSummaryDto>> GetMarketSummariesAsync()
        {
            HttpClient httpClient = this.CreateHttpClient();

            using (HttpResponseMessage response = await httpClient.GetAsync(new Uri(uriString: "getmarketsummaries", UriKind.Relative)))
            {
                response.EnsureSuccessStatusCode();

                string content = await response.Content.ReadAsStringAsync();

                BittrexMarketSummariesDto? summaries = JsonConvert.DeserializeObject<BittrexMarketSummariesDto>(content, this._serializerSettings);

                IReadOnlyList<BittrexMarketSummaryDto>? items = summaries?.Result;

                return items ?? Array.Empty<BittrexMarketSummaryDto>();
            }
        }

        public static void Register(IServiceCollection services)
        {
            services.AddSingleton<IMarketClient, BittrexClient>();

            AddHttpClientFactorySupport(services, HTTP_CLIENT_NAME, Endpoint);
        }
    }
}