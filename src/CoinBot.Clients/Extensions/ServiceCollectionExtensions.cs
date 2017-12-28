using CoinBot.Clients.Bittrex;
using CoinBot.Clients.CoinMarketCap;
using CoinBot.Clients.GateIo;
using CoinBot.Clients.Gdax;
using CoinBot.Clients.Kraken;
using CoinBot.Clients.Liqui;
using CoinBot.Core;
using Microsoft.Extensions.DependencyInjection;

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
			return services
				.AddSingleton<ICoinClient, CoinMarketCapClient>()
				.AddSingleton<IMarketClient, BittrexClient>()
				.AddSingleton<IMarketClient, GdaxClient>()
				.AddSingleton<IMarketClient, GateIoClient>()
				.AddSingleton<IMarketClient, KrakenClient>()
				.AddSingleton<IMarketClient, LiquiClient>();
		}
	}
}
