using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CoinBot.Clients.Extensions;
using CoinBot.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CoinBot.Clients.Bittrex
{
    public class BittrexClient : IMarketClient
    {
        private const string HTTP_CLIENT_NAME = @"Bittrex";

        /// <summary>
        ///     The <see cref="Uri" /> of the CoinMarketCap endpoint.
        /// </summary>
        private readonly static Uri Endpoint = new Uri(uriString: "https://bittrex.com/api/v1.1/public/", UriKind.Absolute);

        /// <summary>
        ///     The <see cref="CurrencyManager" />.
        /// </summary>
        private readonly CurrencyManager _currencyManager;

        /// <summary>
        ///     The http client factory
        /// </summary>
        private readonly IHttpClientFactory _httpClientFactory;

        /// <summary>
        ///     The <see cref="ILogger" />.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        ///     The <see cref="JsonSerializerSettings" />.
        /// </summary>
        private readonly JsonSerializerSettings _serializerSettings;

        public BittrexClient(IHttpClientFactory httpClientFactory, ILogger<BittrexClient> logger, CurrencyManager currencyManager)
        {
            this._httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._currencyManager = currencyManager ?? throw new ArgumentNullException(nameof(currencyManager));

            //this._httpClient = new HttpClient {BaseAddress = this._endpoint};

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
        public string Name => "Bittrex";

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<MarketSummaryDto>> GetAsync()
        {
            try
            {
                List<BittrexMarketSummaryDto> summaries = await this.GetMarketSummariesAsync();

                return summaries.Select(selector: m => new MarketSummaryDto
                                                       {
                                                           BaseCurrrency = this._currencyManager.Get(m.MarketName.Substring(startIndex: 0, m.MarketName.IndexOf(value: '-'))),
                                                           MarketCurrency = this._currencyManager.Get(m.MarketName.Substring(m.MarketName.IndexOf(value: '-') + 1)),
                                                           Market = "Bittrex",
                                                           Volume = m.BaseVolume,
                                                           Last = m.Last,
                                                           LastUpdated = m.TimeStamp
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
        private async Task<List<BittrexMarketSummaryDto>> GetMarketSummariesAsync()
        {
            HttpClient httpClient = this._httpClientFactory.CreateClient(HTTP_CLIENT_NAME);

            using (HttpResponseMessage response = await httpClient.GetAsync(new Uri(uriString: "getmarketsummaries", UriKind.Relative)))
            {
                BittrexMarketSummariesDto summaries =
                    JsonConvert.DeserializeObject<BittrexMarketSummariesDto>(await response.Content.ReadAsStringAsync(), this._serializerSettings);

                return summaries.Result;
            }
        }

        public static void Register(IServiceCollection services)
        {
            // TODO: Add Http Client Factory
            services.AddSingleton<IMarketClient, BittrexClient>();

            services.AddHttpClientFactorySupport(HTTP_CLIENT_NAME, Endpoint);
        }
    }
}