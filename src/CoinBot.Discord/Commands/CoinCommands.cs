using CoinBot.CoinSources;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.Logging;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

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

        [Command("coin"), Summary("get info for a coin, e.g. !coin FUN")]
        public async Task Coin([Remainder, Summary("The symbol for the coin")] string symbol)
        {
            using (Context.Channel.EnterTypingState())
            {
                ICoin coin;
                try
                {
                    coin = this._coinSource.Get(symbol);
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
                    decimal volume = Convert.ToDecimal(coin.Volume);
                    decimal dayChange = Convert.ToDecimal(coin.DayChange);

                    EmbedBuilder builder = new EmbedBuilder();
                    builder.WithTitle($"{coin.Name} ({coin.Symbol})");
                    builder.Color = dayChange > 0 ? Color.Green : Color.Red;

                    EmbedAuthorBuilder authorBuilder = new EmbedAuthorBuilder();
                    authorBuilder.Name = "FunFair CoinBot - right click above to block";
                    authorBuilder.Url = "https://funfair.io";
                    authorBuilder.IconUrl = "https://files.coinmarketcap.com/static/img/coins/32x32/funfair.png";
                    builder.WithAuthor(authorBuilder);

                    StringBuilder descriptionBuilder = new StringBuilder();
                    descriptionBuilder.AppendLine($"Market cap ${marketCap:n} (Rank {coin.Rank})");
                    descriptionBuilder.AppendLine($"24 hour volume: ${volume:n}");
                    builder.WithDescription(descriptionBuilder.ToString());
                    builder.WithUrl($"https://coinmarketcap.com/currencies/{coin.Id}/");
                    builder.WithThumbnailUrl($"https://files.coinmarketcap.com/static/img/coins/64x64/{coin.Id}.png");

                    StringBuilder priceStringBuilder = new StringBuilder();
                    priceStringBuilder.AppendLine($"${ price.ToString("#,##0.#################")}");
                    priceStringBuilder.AppendLine($"{coin.PriceBtc} BTC");
                    priceStringBuilder.AppendLine($"{coin.PriceEth} ETH");
                    builder.AddInlineField("Price", priceStringBuilder.ToString());

                    StringBuilder changeStringBuilder = new StringBuilder();
                    changeStringBuilder.AppendLine($"Hour: {coin.HourChange}%");
                    changeStringBuilder.AppendLine($"Day: {coin.DayChange}%");
                    changeStringBuilder.AppendLine($"Week: {coin.WeekChange}%");
                    builder.AddInlineField("Change", changeStringBuilder.ToString());

                    if (coin.LastUpdated.HasValue)
                    {
                        builder.Timestamp = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(coin.LastUpdated));
                    }

                    EmbedFooterBuilder footerBuilder = new EmbedFooterBuilder();
                    footerBuilder.Text = "Prices updated";
                    builder.Footer = footerBuilder;

                    await ReplyAsync(string.Empty, false, builder.Build());
                }
                else
                {
                    await ReplyAsync($"sorry, {symbol} was not found");
                }
            }
        }

        [Command("snapshot"), Summary("get info on up to 5 coins, !snapshot FUN,BTC,IOTA,ETH,ETC")]
        public async Task Snapshot([Remainder, Summary("A Comma separated list of coin symbols")] string symbols)
        {
            using (Context.Channel.EnterTypingState())
            {
                var symbolsList = symbols.Split(',');
                var coins = new List<ICoin>();

                foreach (var symbol in symbolsList)
                {
                    try
                    {
                        var coin = this._coinSource.Get(symbol.Trim());

                        if(coin != null)
                        {
                            coins.Add(coin);
                        }
                    
                       
                    }
                    catch (Exception e)
                    {
                        this._logger.LogError(new EventId(e.HResult), e, e.Message);
                        await ReplyAsync($"oops, something went wrong, sorry!");

                        return;
                    }
                }

                int max = 5;

                EmbedBuilder builder = new EmbedBuilder();
                builder.WithTitle("Snapshot");
               
                StringBuilder sBuilder = new StringBuilder();
                builder.Color = Color.DarkPurple;

                foreach (var coin in coins)
                {
                    decimal price = Convert.ToDecimal(coin.PriceUsd);
                    sBuilder.Append($"**{coin.Name} ({coin.Symbol})** \n\n**Price:** ${price.ToString("#,##0.#################")} / {coin.PriceBtc} BTC \n**Change:** 1 Hour: {coin.HourChange}%   24 Hour: {coin.DayChange}%    7 Day: {coin.WeekChange}%\n");
                    sBuilder.Append($"https://coinmarketcap.com/currencies/{coin.Id}/ \n\n");

                    max--;
                    if (max == 0) break;
                }

                builder.AddInlineField("-", sBuilder.ToString());
                builder.WithCurrentTimestamp();

                await ReplyAsync(string.Empty, false, builder.Build());
            }
        }

        [Command("gainers"), Summary("get list of top 5 coins by Day Change of top 100 coins, e.g. !gainers")]
        public async Task Gainers()
        {
               using(Context.Channel.EnterTypingState())
               {
                    IEnumerable<ICoin> coins;

                    try
                    {
                         coins = this._coinSource.GetTop100();
                    }
                    catch (Exception e)
                    {
                         this._logger.LogError(new EventId(e.HResult), e, e.Message);
                         await ReplyAsync($"oops, something went wrong, sorry!");

                         return;
                    }

                    await fiveCoinReply(coins, "Top 5");
                    
               }
        }

        [Command("losers"), Summary("get list of bottom 5 coins by Day Change of top 100 coins, e.g. !losers")]
        public async Task Losers()
        {
               using(Context.Channel.EnterTypingState())
               {
                    List<ICoin> coins;

                    try
                    {
                         coins = this._coinSource.GetTop100();

                         coins.Reverse();
                    }
                    catch (Exception e)
                    {
                         this._logger.LogError(new EventId(e.HResult), e, e.Message);
                         await ReplyAsync($"oops, something went wrong, sorry!");

                         return;
                    }

                    await fiveCoinReply(coins, "Bottom 5");
                    
               }
        }

        private async Task fiveCoinReply(IEnumerable<ICoin> coins, string title)
        {
              EmbedBuilder builder = new EmbedBuilder();
            StringBuilder sBuilder = new StringBuilder();
              int position = 0;

              builder.Color = title.Contains("Top") ? Color.Green : Color.Red;

             foreach (var coin in coins){
                    decimal price = Convert.ToDecimal(coin.PriceUsd);
                    sBuilder.Append($"{position + 1}. {coin.Name} ({coin.Symbol}) - ${price.ToString("#,##0.#################")} / {coin.PriceBtc} BTC {coin.HourChange}% / {coin.DayChange}% / {coin.WeekChange}%\n\n");
                    position++;
                    if (position == 5) break;   
                }

            builder.AddInlineField(title, sBuilder.ToString());

            sBuilder.Clear();

            sBuilder.Append($"Of the Top 100 coins sorted by 24 hour % change \n");
            sBuilder.Append($"Format 1h / 24 hours / 7 days");

            builder.AddInlineField("Description", sBuilder.ToString());

               await ReplyAsync(string.Empty, false, builder.Build());
        }
    }
}
