using System.Text;
using System.Threading.Tasks;
using CoinBot.Core;
using CoinBot.Core.Extensions;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.Logging;

namespace CoinBot.Discord.Commands
{
    public sealed class GlobalCommands : CommandBase
    {
        private readonly CurrencyManager _currencyManager;
        private readonly ILogger<GlobalCommands> _logger;

        public GlobalCommands(CurrencyManager currencyManager, ILogger<GlobalCommands> logger)
        {
            this._currencyManager = currencyManager;
            this._logger = logger;
        }

        [Command(text: "global")]
        [Summary(text: "get global crypto market information")]
        public async Task GlobalAsync()
        {
            using (this.Context.Channel.EnterTypingState())
            {
                IGlobalInfo? globalInfo = this._currencyManager.GlobalInfo;

                if (globalInfo == null)
                {
                    this._logger.LogWarning(message: "Global info is not available");

                    return;
                }

                EmbedBuilder builder = new() {Color = Color.DarkPurple};
                builder.WithTitle(title: "Global Currency Information");
                AddAuthor(builder);

                StringBuilder descriptionBuilder = new();
                descriptionBuilder.AppendLine($"Market cap {globalInfo.MarketCap.AsUsdPrice()}");
                descriptionBuilder.AppendLine($"24 hour volume: {globalInfo.Volume.AsUsdPrice()}");
                descriptionBuilder.AppendLine($"BTC dominance: {globalInfo.BtcDominance.AsPercentage()}");
                descriptionBuilder.AppendLine($"Currencies: {globalInfo.Currencies}");
                descriptionBuilder.AppendLine($"Assets: {globalInfo.Assets}");
                descriptionBuilder.AppendLine($"Markets: {globalInfo.Markets}");
                builder.WithDescription(descriptionBuilder.ToString());

                AddFooter(builder: builder, dateTime: globalInfo.LastUpdated);

                await this.ReplyAsync(message: string.Empty, isTTS: false, builder.Build());
            }
        }
    }
}