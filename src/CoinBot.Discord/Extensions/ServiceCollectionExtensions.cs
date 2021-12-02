using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using CoinBot.Discord.Commands;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CoinBot.Discord.Extensions;

/// <summary>
///     <see cref="CoinBot.Discord" /> <see cref="IServiceCollection" /> extension methods.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     The <see cref="IConfiguration" /> section key of the <see cref="DiscordBotSettings" />.
    /// </summary>
    private const string DISCORD_BOT_SETTINGS_SECTION = "DiscordBot";

    /// <summary>
    ///     Adds the CoinBot to the <paramref name="services" />.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" />.</param>
    /// <param name="configuration">The <see cref="IConfiguration" />.</param>
    /// <returns></returns>
    [SuppressMessage(category: "Microsoft.Reliability", checkId: "CA2000:DisposeObjectsBeforeLosingScope", Justification = "Ownership transferred into a singleton")]
    public static IServiceCollection AddCoinBot(this IServiceCollection services, IConfiguration configuration)
    {
        CommandService commandService = new(new() { DefaultRunMode = RunMode.Async, LogLevel = LogSeverity.Debug });

        return services.Configure<DiscordBotSettings>(configuration.GetSection(DISCORD_BOT_SETTINGS_SECTION))
                       .AddSingleton(commandService)
                       .AddSingleton<DiscordBot>();
    }

    public static async Task<CommandService> AddCommandsAsync(this IServiceProvider serviceProvider)
    {
        CommandService commandService = serviceProvider.GetRequiredService<CommandService>();

        // Add the command modules
        await commandService.AddModuleAsync<CoinCommands>(serviceProvider);
        await commandService.AddModuleAsync<GlobalCommands>(serviceProvider);
        await commandService.AddModuleAsync<HelpCommands>(serviceProvider);
        await commandService.AddModuleAsync<PriceCommands>(serviceProvider);
        await commandService.AddModuleAsync<MarketsCommands>(serviceProvider);

        return commandService;
    }
}