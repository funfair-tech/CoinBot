using System;
using System.Diagnostics.CodeAnalysis;
using CoinBot.Clients.Extensions;
using CoinBot.Core.Extensions;
using CoinBot.Discord.Extensions;
using CoinBot.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace CoinBot
{
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

        /// <summary>
        ///     Adds services to the <paramref name="serviceCollection" /> container.
        /// </summary>
        /// <param name="hostBuilderContext">Host builder context.</param>
        /// <param name="serviceCollection">The <see cref="IServiceCollection" />.</param>
        [SuppressMessage(category: "Microsoft.Usage", checkId: "CA1801:ReviewUnusedParameters", Justification = "Simplifies interface")]
        public void ConfigureServices(HostBuilderContext hostBuilderContext, IServiceCollection serviceCollection)
        {
            Log.Logger = new LoggerConfiguration().Enrich.FromLogContext()
                                                  .WriteTo.Console()
                                                  .CreateLogger();

            serviceCollection.AddOptions()
                             .AddLogging()
                             .AddMemoryCache()
                             .AddClients(this._configuration)
                             .AddCore(this._configuration)
                             .AddCoinBot(this._configuration);

            serviceCollection.AddHostedService<BotService>();
            serviceCollection.AddHostedService<MarketService>();
        }

        /// <summary>
        ///     Starts the <see cref="CoinBot" />.
        /// </summary>
        /// <param name="provider">The <see cref="IServiceProvider" />.</param>
        /// <returns></returns>
        public static void Start(IServiceProvider provider)
        {
            ILoggerFactory loggerFactory = provider.GetRequiredService<ILoggerFactory>();

            loggerFactory.AddSerilog();
        }
    }
}