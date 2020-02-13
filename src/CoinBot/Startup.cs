using System;
using System.IO;
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

namespace CoinBot
{
    internal sealed class Startup
    {
        /// <summary>
        /// The <see cref="IConfigurationRoot"/>.
        /// </summary>
        private readonly IConfigurationRoot _configuration;

        /// <summary>
        /// Constructs a <see cref="Startup"/>.
        /// </summary>
        internal Startup()
        {
            // Load the application configuration
            this._configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                                                            .AddJsonFile("appsettings.json", true)
                                                            .AddJsonFile("appsettings-local.json", true)
                                                            .AddEnvironmentVariables()
                                                            .Build();
        }

        public async Task StartAsync()
        {
            // Build the service provider
            ServiceCollection services = new ServiceCollection();
            await this.ConfigureServicesAsync(services);
            ServiceProvider provider = services.BuildServiceProvider();

            // Run the application
            await RunAsync(provider);
        }

        /// <summary>
        /// Adds services to the <paramref name="services"/> container.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        private Task ConfigureServicesAsync(IServiceCollection services)
        {
            Log.Logger = new LoggerConfiguration().Enrich.FromLogContext()
                                                  .WriteTo.Console()
                                                  .CreateLogger();

            services.AddOptions()
                    .AddSingleton(provider =>
                                  {
                                      // set up an ILogger
                                      ILoggerFactory loggerFactory = new LoggerFactory().AddSerilog();

                                      return loggerFactory.CreateLogger(nameof(CoinBot));
                                  })
                    .AddMemoryCache()
                    .AddClients()
                    .AddCore(this._configuration);

            return services.AddCoinBotAsync(this._configuration);
        }

        /// <summary>
        /// Starts the <see cref="CoinBot"/>.
        /// </summary>
        /// <param name="provider">The <see cref="IServiceProvider"/>.</param>
        /// <returns></returns>
        private static async Task RunAsync(IServiceProvider provider)
        {
            //set up a task completion source so we can quit on CTRL+C
            TaskCompletionSource<bool> exitSource = new TaskCompletionSource<bool>();
            Console.CancelKeyPress += (sender, eventArgs) =>
                                      {
                                          eventArgs.Cancel = true;
                                          exitSource.SetResult(true);
                                      };

            CurrencyManager coinManager = provider.GetRequiredService<CurrencyManager>();
            MarketManager marketManager = provider.GetRequiredService<MarketManager>();
            DiscordBot bot = provider.GetRequiredService<DiscordBot>();

            // Initialize the bot
            DiscordBotSettings botConfig = provider.GetRequiredService<IOptions<DiscordBotSettings>>()
                                                   .Value;
            await bot.LoginAsync(TokenType.Bot, botConfig.Token);

            // Start the bot & coinSource
            await bot.StartAsync();
            coinManager.Start();
            marketManager.Start();

            // Keep the application alive until the exitSource task is completed.
            await exitSource.Task;

            // Stop the bot & coinSource
            await bot.LogoutAsync();
            coinManager.Stop();
            marketManager.Stop();
        }
    }
}