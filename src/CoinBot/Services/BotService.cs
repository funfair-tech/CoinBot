using System;
using System.Threading;
using System.Threading.Tasks;
using CoinBot.Discord;
using Discord;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace CoinBot.Services
{
    public sealed class BotService : BackgroundService
    {
        private readonly DiscordBot _bot;
        private readonly IOptions<DiscordBotSettings> _settings;

        public BotService(DiscordBot bot, IOptions<DiscordBotSettings> settings)
        {
            this._bot = bot;
            this._settings = settings;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Initialize the bot
            DiscordBotSettings botConfig = this._settings.Value;
            await this._bot.LoginAsync(tokenType: TokenType.Bot, token: botConfig.Token);

            // Start the bot & coinSource
            await this._bot.StartAsync();

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(value: 30), cancellationToken: stoppingToken);
            }

            await this._bot.LogoutAsync();
        }
    }
}