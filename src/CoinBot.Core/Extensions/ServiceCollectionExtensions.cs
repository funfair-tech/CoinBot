using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CoinBot.Core.Extensions
{
	/// <summary>
	/// <see cref="IServiceCollection"/> extension methods.
	/// </summary>
	public static class ServiceCollectionExtensions
	{
		/// <summary>
		/// The <see cref="IConfiguration"/> section key of the <see cref="CoinMarketCalSettings"/>.
		/// </summary>
		private const string CoinMarketCalSettingsSection = "CoinMarketCal";

		/// <summary>
		/// The <see cref="IConfiguration"/> section key of the <see cref="MarketManagerSettings"/>.
		/// </summary>
		private const string MarketManagerSettingsSection = "MarketManager";

		/// <summary>
		/// Adds coin sources to the <paramref name="services"/>.
		/// </summary>
		/// <param name="services">The <see cref="IServiceCollection"/>.</param>
		/// <param name="configuration">The <see cref="IConfiguration"/>.</param>
		/// <returns></returns>
		public static IServiceCollection AddCore(this IServiceCollection services, IConfiguration configuration)
		{
			return services
				.Configure<MarketManagerSettings>(configuration.GetSection(MarketManagerSettingsSection))
				.Configure<CoinMarketCalSettings>(configuration.GetSection(CoinMarketCalSettingsSection))
				.AddSingleton<CurrencyManager>()
				.AddSingleton<MarketManager>()
				.AddSingleton<EventManager>();
		}
	}
}
