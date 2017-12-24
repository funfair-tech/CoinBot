using Microsoft.Extensions.DependencyInjection;

namespace CoinBot.CoinSources.Extensions
{
	/// <summary>
	/// <see cref="CoinBot.CoinSources"/> <see cref="IServiceCollection"/> extension methods.
	/// </summary>
	public static class ServiceCollectionExtensions
	{
		/// <summary>
		/// Adds coin sources to the <paramref name="services"/>.
		/// </summary>
		/// <param name="services">The <see cref="IServiceCollection"/>.</param>
		/// <returns></returns>
		public static IServiceCollection AddCoinSources(this IServiceCollection services)
		{
			return services
				.AddSingleton<ICoinSource, CoinMarketCap.CoinMarketCap>();
		}
	}
}
