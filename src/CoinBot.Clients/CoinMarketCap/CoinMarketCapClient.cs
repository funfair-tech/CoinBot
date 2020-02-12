using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using CoinBot.Core;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CoinBot.Clients.CoinMarketCap
{
    public class CoinMarketCapClient : ICoinClient
    {
        /// <summary>
        ///     The <see cref="Uri" /> of the CoinMarketCap endpoint.
        /// </summary>
        private readonly Uri _endpoint = new Uri(uriString: "https://api.coinmarketcap.com/v1/", UriKind.Absolute);

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

        public CoinMarketCapClient(ILogger logger)
        {
            this._logger = logger;
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

        /// <inheritdoc />
        /// <summary>
        ///     get the list of coin info from coinmarketcap
        /// </summary>
        /// <returns></returns>
        public async Task<IReadOnlyCollection<ICoinInfo>> GetCoinInfoAsync()
        {
            try
            {
                using (HttpResponseMessage response = await this._httpClient.GetAsync(new Uri(uriString: "ticker/?convert=ETH&limit=1000", UriKind.Relative)))
                {
                    return JsonConvert.DeserializeObject<List<CoinMarketCapCoin>>(await response.Content.ReadAsStringAsync(), this._serializerSettings);
                }
            }
            catch (Exception e)
            {
                this._logger.LogError(new EventId(e.HResult), e, e.Message);

                throw;
            }
        }

        /// <inheritdoc />
        /// <summary>
        ///     update the global info from coinmarketcap
        /// </summary>
        /// <returns></returns>
        public async Task<IGlobalInfo> GetGlobalInfo()
        {
            try
            {
                using (HttpResponseMessage response = await this._httpClient.GetAsync(new Uri(uriString: "global/", UriKind.Relative)))
                {
                    return JsonConvert.DeserializeObject<CoinMarketCapGlobalInfo>(await response.Content.ReadAsStringAsync(), this._serializerSettings);
                }
            }
            catch (Exception e)
            {
                this._logger.LogError(new EventId(e.HResult), e, e.Message);

                throw;
            }
        }
    }
}