﻿using CoinBot.CoinSources;
using CoinBot.CoinSources.CoinMarketCap;
using CoinBot.Discord;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Threading.Tasks;

namespace CoinBotCore
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            // set up an ILogger
            ILoggerFactory loggerFactory = new LoggerFactory().AddSerilog();
            Microsoft.Extensions.Logging.ILogger logger = loggerFactory.CreateLogger("CoinBot");

            // set up the CoinMarketCap source
            ICoinSource coinSource = new CoinMarketCap(logger);
            coinSource.Start();

            // Create our DI container
            ServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(coinSource);
            serviceCollection.AddSingleton(logger);
            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            // create the discord bot and start it
            DiscordBot bot = new DiscordBot(DiscordBotToken.Load("token.json"), serviceProvider, logger);
            await bot.Start();

            Console.ReadLine();

            // stop the discord bot
            await bot.Stop();

            // stop the source
            coinSource.Stop();
        }
    }
}