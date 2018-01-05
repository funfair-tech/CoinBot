using CoinBot.CoinSources;
using CoinBot.Discord.Extensions;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Threading.Tasks;

namespace CoinBot.Discord.Commands
{
    public class GlobalCommands : CommandBase
    {
        private readonly ICoinSource _coinSource;
        private readonly ILogger _logger;

        public GlobalCommands(ICoinSource coinSource, ILogger logger)
        {
            this._coinSource = coinSource;
            this._logger = logger;
        }

        [Command("global"), Summary("get global crypto market information")]
        public async Task Global()
        {
            using (Context.Channel.EnterTypingState())
            {
                IGlobalInfo globalInfo = this._coinSource.GetGlobalInfo();

                EmbedBuilder builder = new EmbedBuilder();
                builder.WithTitle("Global Cryptocurrency Information");
                builder.Color = Color.DarkPurple;
                AddAuthor(builder);

                var descriptionBuilder = new StringBuilder();
                descriptionBuilder.AppendLine($"Market cap {globalInfo.MarketCap.AsUsdCurrency()}");
                descriptionBuilder.AppendLine($"24 hour volume: {globalInfo.Volume.AsUsdCurrency()}");
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
