using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using CoinBot.Clients.Binance;
using CoinBot.Clients.Bittrex;
using CoinBot.Clients.CoinMarketCap;
using CoinBot.Clients.GateIo;
using CoinBot.Clients.Gdax;
using CoinBot.Clients.Kraken;
using CoinBot.Clients.Liqui;
using CoinBot.Clients.Poloniex;
using Microsoft.Extensions.DependencyInjection;
using Polly;

namespace CoinBot.Clients.Extensions
{
    /// <summary>
    /// <see cref="IServiceCollection"/> extension methods.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds coin sources to the <paramref name="services"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <returns></returns>
        public static IServiceCollection AddClients(this IServiceCollection services)
        {
            CoinMarketCapClient.Register(services);
            BinanceClient.Register(services);
            BittrexClient.Register(services);
            GdaxClient.Register(services);
            GateIoClient.Register(services);
            KrakenClient.Register(services);
            LiquiClient.Register(services);
            PoloniexClient.Register(services);

            return services;
        }

        internal static void AddHttpClientFactorySupport(this IServiceCollection services, string clientName, Uri endpoint)
        {
            const int maxRetries = 3;

            services.AddHttpClient(clientName)
                    .ConfigureHttpClient(configureClient: httpClient =>
                                                          {
                                                              httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: @"application/json"));
                                                              httpClient.BaseAddress = endpoint;
                                                          })
                    .ConfigurePrimaryHttpMessageHandler(
                        configureHandler: x => new HttpClientHandler {AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate})
                    .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(value: 30)))
                    .AddTransientHttpErrorPolicy(configurePolicy: p => p.WaitAndRetryAsync(maxRetries, RetryDelayCalculator.Calculate));
        }
    }
}