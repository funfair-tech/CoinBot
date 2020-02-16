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

        [Command("global"), Summary("get global crypto market information")]
        public async Task GlobalAsync()
        {
            using (this.Context.Channel.EnterTypingState())
            {
                IGlobalInfo? globalInfo = this._currencyManager.GetGlobalInfo();

                if (globalInfo == null)
                {
                    // TODO: report not available.
                    return;
                }

                EmbedBuilder builder = new EmbedBuilder {Color = Color.DarkPurple};
                builder.WithTitle("Global Currency Information");
                AddAuthor(builder);

                StringBuilder descriptionBuilder = new StringBuilder();
                descriptionBuilder.AppendLine($"Market cap {globalInfo.MarketCap.AsUsdPrice()}");
                descriptionBuilder.AppendLine($"24 hour volume: {globalInfo.Volume.AsUsdPrice()}");
                descriptionBuilder.AppendLine($"BTC dominance: {globalInfo.BtcDominance.AsPercentage()}");
                descriptionBuilder.AppendLine($"Currencies: {globalInfo.Currencies}");
                descriptionBuilder.AppendLine($"Assets: {globalInfo.Assets}");
                descriptionBuilder.AppendLine($"Markets: {globalInfo.Markets}");
                builder.WithDescription(descriptionBuilder.ToString());

                AddFooter(builder, globalInfo.LastUpdated);

                await this.ReplyAsync(string.Empty, false, builder.Build());
            }
        }
    }
}