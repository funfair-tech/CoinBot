using CoinBot.CoinSources;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.Logging;
using System;
using System.Text;
using System.Threading.Tasks;

namespace CoinBot.Discord.Commands
{
    public class CoinCommands : ModuleBase
    {
        private readonly ICoinSource _coinSource;
        private readonly ILogger _logger;

        public CoinCommands(ICoinSource coinSource, ILogger logger)
        {
            this._coinSource = coinSource;
            this._logger = logger;
        }

        [Command("coin"), Summary("get info for a coin")]
        public async Task Coin([Remainder, Summary("The symbol for the coin")] string symbol)
        {
            using (Context.Channel.EnterTypingState())
            {
                ICoin coin;
                try
                {
                    coin = this._coinSource.GetCoinBySymbol(symbol) ?? this._coinSource.GetCoinByName(symbol);
                }
                catch (Exception e)
                {
                    this._logger.LogError(new EventId(e.HResult), e, e.Message);
                    await ReplyAsync($"oops, something went wrong, sorry!");

                    return;
                }
                
                if (coin != null)
                {
                    decimal marketCap = Convert.ToDecimal(coin.MarketCap);
                    decimal price = Convert.ToDecimal(coin.PriceUsd);

                    EmbedBuilder builder = new EmbedBuilder();
                    builder.WithTitle($"{coin.Symbol} - {coin.Name} (Rank {coin.Rank}, Market Cap ${marketCap:n})");
                    builder.WithThumbnailUrl($"https://files.coinmarketcap.com/static/img/coins/64x64/{coin.Id}.png");

                    EmbedFieldBuilder priceFieldBuilder = new EmbedFieldBuilder();
                    priceFieldBuilder.Name = "Price";
                    StringBuilder priceStringBuilder = new StringBuilder();
                    priceStringBuilder.AppendLine($"${ price.ToString("#,##0.#################")}");
                    priceStringBuilder.AppendLine($"{coin.PriceBtc} BTC");
                    priceStringBuilder.AppendLine($"{coin.PriceEth} ETH");
                    priceFieldBuilder.Value = priceStringBuilder.ToString();
                    builder.Fields.Add(priceFieldBuilder);

                    EmbedFieldBuilder changeFieldBuilder = new EmbedFieldBuilder();
                    changeFieldBuilder.Name = "Change";
                    StringBuilder changeStringBuilder = new StringBuilder();
                    changeStringBuilder.AppendLine($"Hour: {coin.HourChange}%");
                    changeStringBuilder.AppendLine($"Day: {coin.DayChange}%");
                    changeStringBuilder.AppendLine($"Week: {coin.WeekChange}%");
                    changeFieldBuilder.Value = changeStringBuilder.ToString();
                    builder.Fields.Add(changeFieldBuilder);

                    if (coin.LastUpdated.HasValue)
                    {
                        DateTimeOffset updated = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(coin.LastUpdated));

                        EmbedFieldBuilder updatedFieldBuilder = new EmbedFieldBuilder();
                        updatedFieldBuilder.Name = "Last updated";
                        updatedFieldBuilder.Value = $"{updated.ToString("yyyy-MM-dd HH:mm:ss")} UTC";
                        builder.Fields.Add(updatedFieldBuilder);
                    }

                    EmbedFooterBuilder footerBuilder = new EmbedFooterBuilder();
                    footerBuilder.IconUrl = $"https://files.coinmarketcap.com/static/img/coins/32x32/funfair.png";
                    footerBuilder.Text = "FunFair CoinBot - learn about FunFair's blockchain casino platform at https://funfair.io";
                    builder.Footer = footerBuilder;

                    await ReplyAsync(string.Empty, false, builder);
                }
                else
                {
                    await ReplyAsync($"sorry, {symbol} was not found");
                }
            }
        }
    }
}
