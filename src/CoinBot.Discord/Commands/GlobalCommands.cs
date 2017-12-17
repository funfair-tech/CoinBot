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
                builder.WithTitle($"Global Cryptocurrency Information");
                builder.Color = Color.DarkPurple;
                AddAuthor(builder);

                StringBuilder descriptionBuilder = new StringBuilder();
                descriptionBuilder.AppendLine($"Market cap ${globalInfo.MarketCap.FormatCurrencyValue()}");
                descriptionBuilder.AppendLine($"24 hour volume: ${globalInfo.Volume.FormatCurrencyValue()}");
                descriptionBuilder.AppendLine($"BTC dominance: {globalInfo.BTCDominance}%");
                descriptionBuilder.AppendLine($"Currencies: {globalInfo.Currencies.FormatNumber()}");
                descriptionBuilder.AppendLine($"Assets: {globalInfo.Assets.FormatNumber()}");
                descriptionBuilder.AppendLine($"Markets: {globalInfo.Markets.FormatNumber()}");
                builder.WithDescription(descriptionBuilder.ToString());
                
                AddFooter(builder, globalInfo.LastUpdated);

                await ReplyAsync(string.Empty, false, builder.Build());
            }
        }
    }
}
