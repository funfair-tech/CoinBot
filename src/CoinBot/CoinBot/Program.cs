using CoinBot.Discord;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
            ILoggerFactory loggerFactory = new LoggerFactory().AddConsole();
            ILogger logger = loggerFactory.CreateLogger("CoinBot");

            ServiceCollection serviceCollection = new ServiceCollection();
            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            Bot bot = new Bot(BotToken.Load("token.json"), serviceProvider, logger);
            await bot.Start();

            Console.ReadLine();

            await bot.Stop();
        }
    }
}