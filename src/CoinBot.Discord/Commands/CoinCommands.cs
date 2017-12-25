using CoinBot.CoinSources;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using CoinBot.Discord.Extensions;

namespace CoinBot.Discord.Commands
{
    public class CoinCommands : CommandBase
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
                    await ReplyAsync("oops, something went wrong, sorry!");

                    return;
                }
                
                if (coin != null)
                {
                    var builder = new EmbedBuilder();
                    builder.WithTitle(coin.GetTitle());
                    builder.Color = coin.DayChange > 0 ? Color.Green : Color.Red;
                    AddAuthor(builder);

                    builder.WithDescription(coin.GetDescription());
                    builder.WithUrl(coin.GetCoinMarketCapLink());
                    builder.WithThumbnailUrl(coin.GetCoinImageUrl());
                    builder.AddInlineField("Price", coin.GetPrice());
                    builder.AddInlineField("Change", coin.GetChange());

                    AddFooter(builder, coin.LastUpdated);

                    await ReplyAsync(string.Empty, false, builder.Build());
                }
                else
                {
                    await ReplyAsync($"sorry, {symbol} was not found");
                }
            }
        }

        [Command("snapshot"), Summary("get info on a list of coins, !snapshot FUN,BTC,IOTA,ETH,ETC")]
        public async Task Snapshot([Remainder, Summary("A comma separated list of coin symbols")] string symbols)
        {
            using (Context.Channel.EnterTypingState())
            {
                string[] symbolsList = symbols.Split(',').Select(s => s.Trim()).ToArray();
                IList<ICoin> coins = new List<ICoin>();
                IList<string> notFound = new List<string>();

                foreach (string symbol in symbolsList)
                {
                    try
                    {
                        ICoin coin = this._coinSource.Get(symbol);
                        if (coin != null)
                        {
                            coins.Add(coin);
                        }
                        else
                        {
                            notFound.Add(symbol);
                        }
                    }
                    catch (Exception e)
                    {
                        this._logger.LogError(new EventId(e.HResult), e, e.Message);
                        await ReplyAsync($"oops, something went wrong, sorry!");

                        return;
                    }
                }

                if (notFound.Count > 0)
                {
                    if (notFound.Count > 1)
                    {
                        await ReplyAsync($"sorry, {string.Join(", ", notFound)} were not found");
                    }
                    else
                    {
                        await ReplyAsync($"sorry, {notFound[0]} was not found");
                    }
                }

                var totalChange = coins.Sum(c => c.DayChange);
                await MultiCoinReply(coins, totalChange > 0 ? Color.Green : Color.Red, "Snapshot", string.Join(", ", coins.Select(c => c.Symbol)));
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

                    await MultiCoinReply(coins.Take(5).ToList(), Color.Green, "Gainers", "The 5 coins in the top 100 with the biggest 24 hour gain");
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
                         await ReplyAsync("oops, something went wrong, sorry!");

                         return;
                    }

                    await MultiCoinReply(coins.Take(5).ToList(), Color.Red, "Losers", "The 5 coins in the top 100 with the biggest 24 hour loss");
               }
        }

        private async Task MultiCoinReply(IList<ICoin> coins, Color color, string title, string description)
        {
            var builder = new EmbedBuilder();
            builder.WithTitle(title);
            builder.WithDescription(description);
            builder.Color = color;
            AddAuthor(builder);
            AddFooter(builder, coins.Max(c => c.LastUpdated));

            foreach (var coin in coins)
            {
                builder.Fields.Add(new EmbedFieldBuilder
                {
                    Name = $"{coin.Name} ({coin.Symbol}) | {coin.DayChange.AsPercentage()} | ${coin.GetPriceSummary()}",
                    Value = $"[{coin.GetChangeSummary()}{Environment.NewLine}Cap {coin.MarketCap.AsUsdCurrency()} | Vol {coin.Volume.AsUsdCurrency()} | Rank {coin.Rank}]({coin.GetCoinMarketCapLink()})"
                });
            }

            await ReplyAsync(string.Empty, false, builder.Build());
        }
    }
}
