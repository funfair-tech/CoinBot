using CoinBot.Discord.Commands;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CoinBot.Discord.Extensions
{
	/// <summary>
	/// <see cref="CoinBot.Discord"/> <see cref="IServiceCollection"/> extension methods.
	/// </summary>
	public static class ServiceCollectionExtensions
	{
		/// <summary>
		/// The <see cref="IConfiguration"/> section key of the <see cref="DiscordBotSettings"/>.
		/// </summary>
		private const string DiscordBotSettingsSection = "DiscordBot";

		/// <summary>
		/// Adds the CoinBot to the <paramref name="services"/>.
		/// </summary>
		/// <param name="services">The <see cref="IServiceCollection"/>.</param>
		/// <param name="configuration">The <see cref="IConfiguration"/>.</param>
		/// <returns></returns>
		public static IServiceCollection AddCoinBot(this IServiceCollection services, IConfiguration configuration)
		{
			return services
				.Configure<DiscordBotSettings>(configuration.GetSection(DiscordBotSettingsSection))
				.AddSingleton(ctx =>
				{
					CommandService commandService = new CommandService(new CommandServiceConfig
					{
						DefaultRunMode = RunMode.Async,
						LogLevel = LogSeverity.Debug
					});

					// Add the command modules
					commandService.AddModuleAsync<CoinCommands>();
					commandService.AddModuleAsync<GlobalCommands>();
					commandService.AddModuleAsync<HelpCommands>();
					commandService.AddModuleAsync<PriceCommands>();
					commandService.AddModuleAsync<MarketsCommands>();
					commandService.AddModuleAsync<EventCommands>();
					return commandService;
				})
				.AddSingleton<DiscordBot>();
		}
	}
}
