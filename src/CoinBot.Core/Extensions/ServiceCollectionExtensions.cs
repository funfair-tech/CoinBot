using System.Collections.Generic;
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
        /// The <see cref="IConfiguration"/> section key of the <see cref="MarketManagerSettings"/>.
        /// </summary>
        private const string MARKET_MANAGER_SETTINGS_SECTION = "MarketManager";

        /// <summary>
        /// Adds coin sources to the <paramref name="services"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="configuration">The <see cref="IConfiguration"/>.</param>
        /// <returns></returns>
        public static IServiceCollection AddCore(this IServiceCollection services, IConfiguration configuration)
        {
            return services.Configure<MarketManagerSettings>(configuration.GetSection(MARKET_MANAGER_SETTINGS_SECTION))
                           .AddSingleton<CurrencyManager>()
                           .AddSingleton<MarketManager>();
        }
    }

    public static class EnumerableExtensions
    {
        public static IEnumerable<T> RemoveNulls<T>(this IEnumerable<T?>? items)
            where T : class
        {
            if (items != null)
            {
                foreach (T? item in items)
                {
                    if (item == null)
                    {
                        continue;
                    }

                    yield return item;
                }
            }
        }
    }
}