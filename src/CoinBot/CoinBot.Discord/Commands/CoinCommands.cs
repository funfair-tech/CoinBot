﻿using CoinBot.CoinSources;
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
                    decimal volume = Convert.ToDecimal(coin.Volume);
                    decimal dayChange = Convert.ToDecimal(coin.DayChange);

                    EmbedBuilder builder = new EmbedBuilder();
                    builder.WithTitle($"{coin.Symbol} - { coin.Name}");
                    builder.Color = dayChange > 0 ? Color.Green : Color.Red;

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
                    footerBuilder.IconUrl = $"https://files.coinmarketcap.com/static/img/coins/32x32/funfair.png";
                    footerBuilder.Text = "FunFair CoinBot - https://funfair.io";
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
