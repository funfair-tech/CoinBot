using System.Threading.Tasks;
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
        private const string DISCORD_BOT_SETTINGS_SECTION = "DiscordBot";

        /// <summary>
        /// Adds the CoinBot to the <paramref name="services"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="configuration">The <see cref="IConfiguration"/>.</param>
        /// <returns></returns>
        public static async Task<IServiceCollection> AddCoinBotAsync(this IServiceCollection services, IConfiguration configuration)
        {
            CommandService commands = await AddCommandsAsync();

            return services.Configure<DiscordBotSettings>(configuration.GetSection(DISCORD_BOT_SETTINGS_SECTION))
                           .AddSingleton(commands)
                           .AddSingleton<DiscordBot>();
        }

        private static async Task<CommandService> AddCommandsAsync()
        {
            CommandService commandService = new CommandService(new CommandServiceConfig {DefaultRunMode = RunMode.Async, LogLevel = LogSeverity.Debug});

            // Add the command modules
            await commandService.AddModuleAsync<CoinCommands>();
            await commandService.AddModuleAsync<GlobalCommands>();
            await commandService.AddModuleAsync<HelpCommands>();
            await commandService.AddModuleAsync<PriceCommands>();
            await commandService.AddModuleAsync<MarketsCommands>();

            return commandService;
        }
    }
}