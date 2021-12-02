using System.Text;
using System.Threading.Tasks;
using CoinBot.Core;
using CoinBot.Core.Extensions;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.Logging;

namespace CoinBot.Discord.Commands;

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

            EmbedBuilder builder = new() { Color = Color.DarkPurple };
            builder.WithTitle(title: "Global Currency Information");
            AddAuthor(builder);

            StringBuilder descriptionBuilder = new();
            descriptionBuilder.Append("Market cap ")
                              .AppendLine(globalInfo.MarketCap.AsUsdPrice())
                              .Append("24 hour volume: ")
                              .AppendLine(globalInfo.Volume.AsUsdPrice())
                              .Append("BTC dominance: ")
                              .AppendLine(globalInfo.BtcDominance.AsPercentage())
                              .Append("Currencies: ")
                              .Append(globalInfo.Currencies)
                              .AppendLine()
                              .Append("Assets: ")
                              .Append(globalInfo.Assets)
                              .AppendLine()
                              .Append("Markets: ")
                              .Append(globalInfo.Markets)
                              .AppendLine();
            builder.WithDescription(descriptionBuilder.ToString());

            AddFooter(builder: builder, dateTime: globalInfo.LastUpdated);

            await this.ReplyAsync(message: string.Empty, isTTS: false, builder.Build());
        }
    }
}