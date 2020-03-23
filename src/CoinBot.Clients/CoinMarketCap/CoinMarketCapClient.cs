﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using CoinBot.Core;
using CoinBot.Core.JsonConverters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CoinBot.Clients.CoinMarketCap
{
    public sealed class CoinMarketCapClient : CoinClientBase, ICoinClient
    {
        private const string HTTP_CLIENT_NAME = @"CoinMarketCap";

        /// <summary>
        ///     The <see cref="Uri" /> of the CoinMarketCap endpoint.
        /// </summary>
        private static readonly Uri Endpoint = new Uri(uriString: "https://api.coinmarketcap.com/v1/", UriKind.Absolute);

        /// <summary>
        ///     The <see cref="JsonSerializerOptions" />.
        /// </summary>
        private readonly JsonSerializerOptions _serializerSettings;

        public CoinMarketCapClient(IHttpClientFactory httpClientFactory, ILogger<CoinMarketCapClient> logger)
            : base(httpClientFactory, HTTP_CLIENT_NAME, logger)
        {
            this._serializerSettings = new JsonSerializerOptions
                                       {
                                           IgnoreNullValues = true,
                                           PropertyNameCaseInsensitive = false,
                                           PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                                           Converters = {new DecimalAsStringConverter()}
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
                HttpClient httpClient = this.CreateHttpClient();

                using (HttpResponseMessage response = await httpClient.GetAsync(new Uri(uriString: "ticker/?convert=ETH&limit=1000", UriKind.Relative)))
                {
                    response.EnsureSuccessStatusCode();

                    string json = await response.Content.ReadAsStringAsync();

                    return JsonSerializer.Deserialize<List<CoinMarketCapCoin>>(json, this._serializerSettings) ?? new List<CoinMarketCapCoin>();
                }
            }
            catch (Exception e)
            {
                this.Logger.LogError(new EventId(e.HResult), e, e.Message);

                throw;
            }
        }

        /// <inheritdoc />
        /// <summary>
        ///     update the global info from coinmarketcap
        /// </summary>
        /// <returns></returns>
        public async Task<IGlobalInfo?> GetGlobalInfoAsync()
        {
            try
            {
                HttpClient httpClient = this.CreateHttpClient();

                using (HttpResponseMessage response = await httpClient.GetAsync(new Uri(uriString: "global/", UriKind.Relative)))
                {
                    response.EnsureSuccessStatusCode();

                    string json = await response.Content.ReadAsStringAsync();

                    return JsonSerializer.Deserialize<CoinMarketCapGlobalInfo>(json, this._serializerSettings);
                }
            }
            catch (Exception e)
            {
                this.Logger.LogError(new EventId(e.HResult), e, e.Message);

                throw;
            }
        }

        public static void Register(IServiceCollection services)
        {
            services.AddSingleton<ICoinClient, CoinMarketCapClient>();

            AddHttpClientFactorySupport(services, HTTP_CLIENT_NAME, Endpoint);
        }
    }
}