using System;
using System.Threading.Tasks;
using CoinBot.Clients.Extensions;
using CoinBot.Core;
using CoinBot.Core.Extensions;
using CoinBot.Discord;
using CoinBot.Discord.Extensions;
using Discord;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;

namespace CoinBot;

internal sealed class Startup
{
    /// <summary>
    ///     The <see cref="IConfigurationRoot" />.
    /// </summary>
    private readonly IConfigurationRoot _configuration;

    /// <summary>
    ///     Constructs a <see cref="Startup" />.
    /// </summary>
    internal Startup()
    {
        // Load the application configuration
        this._configuration = new ConfigurationBuilder().SetBasePath(ApplicationConfig.ConfigurationFilesPath)
                                                        .AddJsonFile(path: "appsettings.json", optional: true)
                                                        .AddJsonFile(path: "appsettings-local.json", optional: true)
                                                        .AddEnvironmentVariables()
                                                        .Build();
    }

    public Task StartAsync()
    {
        // Build the service provider
        ServiceCollection services = new();
        this.ConfigureServices(services);
        ServiceProvider provider = services.BuildServiceProvider();

        // Run the application
        return RunAsync(provider);
    }

    /// <summary>
    ///     Adds services to the <paramref name="services" /> container.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" />.</param>
    private void ConfigureServices(IServiceCollection services)
    {
        Log.Logger = new LoggerConfiguration().Enrich.FromLogContext()
                                              .WriteTo.Console()
                                              .CreateLogger();

        services.AddOptions()
                .AddLogging()
                .AddMemoryCache()
                .AddClients(this._configuration)
                .AddCore(this._configuration)
                .AddCoinBot(this._configuration);
    }

    /// <summary>
    ///     Starts the <see cref="CoinBot" />.
    /// </summary>
    /// <param name="provider">The <see cref="IServiceProvider" />.</param>
    /// <returns></returns>
    private static async Task RunAsync(IServiceProvider provider)
    {
        ILoggerFactory loggerFactory = provider.GetRequiredService<ILoggerFactory>();

        loggerFactory.AddSerilog();

        //set up a task completion source so we can quit on CTRL+C
        TaskCompletionSource<bool> exitSource = new();
        Console.CancelKeyPress += (_, eventArgs) =>
                                  {
                                      eventArgs.Cancel = true;
                                      exitSource.SetResult(result: true);
                                  };
        await provider.AddCommandsAsync();

        MarketManager marketManager = provider.GetRequiredService<MarketManager>();
        DiscordBot bot = provider.GetRequiredService<DiscordBot>();

        // Initialize the bot
        DiscordBotSettings botConfig = provider.GetRequiredService<IOptions<DiscordBotSettings>>()
                                               .Value;
        await bot.LoginAsync(tokenType: TokenType.Bot, token: botConfig.Token);

        // Start the bot & coinSource
        await bot.StartAsync();
        marketManager.Start();

        // Keep the application alive until the exitSource task is completed.
        await exitSource.Task;

        // Stop the bot & coinSource
        await bot.LogoutAsync();
        marketManager.Stop();
    }
}