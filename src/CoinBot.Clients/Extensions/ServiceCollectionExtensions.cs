using CoinBot.Clients.Binance;
using CoinBot.Clients.Bittrex;
using CoinBot.Clients.FunFair;
using CoinBot.Clients.GateIo;
using CoinBot.Clients.Gdax;
using CoinBot.Clients.Kraken;
using CoinBot.Clients.Poloniex;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CoinBot.Clients.Extensions;

/// <summary>
///     <see cref="IServiceCollection" /> extension methods.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Adds coin sources to the <paramref name="services" />.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" />.</param>
    /// <param name="configurationRoot">Configuration</param>
    /// <returns></returns>
    public static IServiceCollection AddClients(this IServiceCollection services, IConfigurationRoot configurationRoot)
    {
#if NOT_DEPRECATED
            // currently returning errors
            CoinMarketCapClient.Register(services);
#endif
        BinanceClient.Register(services);
        BittrexClient.Register(services);
        GdaxClient.Register(services);
        GateIoClient.Register(services);
        KrakenClient.Register(services);
        PoloniexClient.Register(services);

        FunFairClientBase.Register(services: services,
                                   configurationRoot.GetSection(key: "Sources:FunFair")
                                                    .Get<FunFairClientConfiguration>());

        return services;
    }
}