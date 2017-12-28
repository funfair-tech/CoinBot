using Discord;
using Discord.Commands;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Threading.Tasks;
using CoinBot.Core;
using CoinBot.Core.Extensions;

namespace CoinBot.Discord.Commands
{
    public class GlobalCommands : CommandBase
    {
        private readonly CurrencyManager _currencyManager;
        private readonly ILogger _logger;

        public GlobalCommands(CurrencyManager currencyManager, ILogger logger)
        {
            this._currencyManager = currencyManager;
            this._logger = logger;
        }

        [Command("global"), Summary("get global crypto market information")]
        public async Task Global()
        {
            using (Context.Channel.EnterTypingState())
            {
                IGlobalInfo globalInfo = this._currencyManager.GetGlobalInfo();

                EmbedBuilder builder = new EmbedBuilder();
                builder.WithTitle("Global Currency Information");
                builder.Color = Color.DarkPurple;
                AddAuthor(builder);

                var descriptionBuilder = new StringBuilder();
                descriptionBuilder.AppendLine($"Market cap {globalInfo.MarketCap.AsUsdPrice()}");
                descriptionBuilder.AppendLine($"24 hour volume: {globalInfo.Volume.AsUsdPrice()}");
                descriptionBuilder.AppendLine($"BTC dominance: {globalInfo.BtcDominance.AsPercentage()}");
                descriptionBuilder.AppendLine($"Currencies: {globalInfo.Currencies}");
                descriptionBuilder.AppendLine($"Assets: {globalInfo.Assets}");
                descriptionBuilder.AppendLine($"Markets: {globalInfo.Markets}");
                builder.WithDescription(descriptionBuilder.ToString());
                
                AddFooter(builder, globalInfo.LastUpdated);

                await ReplyAsync(string.Empty, false, builder.Build());
            }
        }
    }
}
