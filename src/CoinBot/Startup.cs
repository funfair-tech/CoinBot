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
			_configuration = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", optional: true)
				.AddEnvironmentVariables()
				.Build();

			// Build the service provider
			var services = new ServiceCollection();
			ConfigureServices(services);
			var provider = services.BuildServiceProvider();

			// Run the application
			Run(provider).GetAwaiter().GetResult();
		}

		/// <summary>
		/// Adds services to the <paramref name="services"/> container.
		/// </summary>
		/// <param name="services">The <see cref="IServiceCollection"/>.</param>
		private void ConfigureServices(IServiceCollection services)
		{
			Log.Logger = new LoggerConfiguration()
				.Enrich.FromLogContext()
				.WriteTo.Console()
				.CreateLogger();

			services
				.AddOptions()
				.AddSingleton(provider =>
				{
					// set up an ILogger
					var loggerFactory = new LoggerFactory().AddSerilog();
					return loggerFactory.CreateLogger(nameof(CoinBot));
				})
				.AddMemoryCache()
				.AddClients()
				.AddCore(_configuration)
				.AddCoinBot(_configuration);
		}

		/// <summary>
		/// Starts the <see cref="CoinBot"/>.
		/// </summary>
		/// <param name="provider">The <see cref="IServiceProvider"/>.</param>
		/// <returns></returns>
		private static async Task Run(IServiceProvider provider)
		{
			//set up a task completion source so we can quit on CTRL+C
			var exitSource = new TaskCompletionSource<bool>();
			Console.CancelKeyPress += (sender, eventArgs) =>
			{
				eventArgs.Cancel = true;
				exitSource.SetResult(true);
			};

			var coinManager = provider.GetRequiredService<CurrencyManager>();
			var marketManager = provider.GetRequiredService<MarketManager>();
			var bot = provider.GetRequiredService<DiscordBot>();

			// Initialize the bot
			var botConfig = provider.GetRequiredService<IOptions<DiscordBotSettings>>().Value;
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
