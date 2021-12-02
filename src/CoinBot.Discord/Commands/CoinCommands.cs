using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoinBot.Clients.CoinMarketCap;
using CoinBot.Clients.FunFair;
using CoinBot.Core;
using CoinBot.Core.Extensions;
using CoinBot.Discord.Extensions;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.Logging;

namespace CoinBot.Discord.Commands;

public sealed class CoinCommands : CommandBase
{
    private readonly CurrencyManager _currencyManager;
    private readonly ILogger<CoinCommands> _logger;
    private readonly MarketManager _marketManager;

    public CoinCommands(CurrencyManager currencyManager, MarketManager marketManager, ILogger<CoinCommands> logger)
    {
        this._currencyManager = currencyManager;
        this._marketManager = marketManager;
        this._logger = logger;
    }

    [Command(text: "coin")]
    [Summary(text: "get info for a coin, e.g. !coin FUN")]
    public async Task CoinAsync([Remainder] [Summary(text: "The symbol for the coin")] string symbol)
    {
        using (this.Context.Channel.EnterTypingState())
        {
            try
            {
                Currency? currency = this._currencyManager.Get(symbol);

                if (currency?.IsFiat == false)
                {
                    EmbedBuilder builder = new();
                    builder.WithTitle(currency.GetTitle());

                    CoinMarketCapCoin? details = currency.Getdetails<CoinMarketCapCoin>();

                    if (details != null)
                    {
                        builder.Color = details.DayChange > 0
                            ? Color.Green
                            : Color.Red;
                        AddAuthor(builder);

                        builder.WithDescription(details.GetDescription());
                        builder.WithUrl(details.Url);

                        if (currency.ImageUrl != null)
                        {
                            builder.WithThumbnailUrl(currency.ImageUrl);
                        }

                        builder.AddField(name: "Price", details.GetPrice());
                        builder.AddField(name: "Change", details.GetChange());
                        AddFooter(builder: builder, dateTime: details.LastUpdated);
                    }
                    else
                    {
                        ICoinInfo walletDetails = this.GetCoinInfo(currency);

                        AddAuthor(builder);

                        if (currency.ImageUrl != null)
                        {
                            builder.WithThumbnailUrl(currency.ImageUrl);
                        }

                        builder.AddField(name: "Price", walletDetails.GetPrice());

                        AddFooter(builder: builder, dateTime: walletDetails.LastUpdated);
                    }

                    await this.ReplyAsync(message: string.Empty, isTTS: false, builder.Build());
                }
                else
                {
                    await this.ReplyAsync($"sorry, {symbol} was not found");
                }
            }
            catch (Exception e)
            {
                this._logger.LogError(new(e.HResult), exception: e, message: e.Message);
                await this.ReplyAsync(message: "oops, something went wrong, sorry!");
            }
        }
    }

    private ICoinInfo GetCoinInfo(Currency currency)
    {
        return currency.Getdetails<FunFairWalletCoin>() ?? (ICoinInfo)new InterpretedCoinInfo(currency: currency,
                                                                                              marketManager: this._marketManager,
                                                                                              this._currencyManager.Get(nameOrSymbol: @"USD"),
                                                                                              this._currencyManager.Get(nameOrSymbol: @"ETH"),
                                                                                              this._currencyManager.Get(nameOrSymbol: @"BTC"));
    }

    [Command(text: "snapshot")]
    [Summary(text: "get info on a list of coins, !snapshot FUN,BTC,IOTA,ETH,ETC")]
    public async Task SnapshotAsync([Remainder] [Summary(text: "A comma separated list of coin symbols")] string symbols)
    {
        using (this.Context.Channel.EnterTypingState())
        {
            string[] symbolsList = symbols.Split(separator: ',')
                                          .Select(selector: s => s.Trim())
                                          .ToArray();
            List<Currency> coins = new();
            IList<string> notFound = new List<string>();

            foreach (string symbol in symbolsList)
            {
                try
                {
                    Currency? currency = this._currencyManager.Get(symbol);

                    if (currency?.Getdetails<CoinMarketCapCoin>() != null)
                    {
                        coins.Add(currency);
                    }
                    else
                    {
                        notFound.Add(symbol);
                    }
                }
                catch (Exception e)
                {
                    this._logger.LogError(new(e.HResult), exception: e, message: e.Message);
                    await this.ReplyAsync(message: "oops, something went wrong, sorry!");

                    return;
                }
            }

            if (notFound.Count > 0)
            {
                if (notFound.Count > 1)
                {
                    await this.ReplyAsync($"sorry, {string.Join(separator: ", ", values: notFound)} were not found");
                }
                else
                {
                    await this.ReplyAsync($"sorry, {notFound[index: 0]} was not found");
                }
            }

            double? totalChange = coins.Sum(selector: c => c.Getdetails<CoinMarketCapCoin>()
                                                            ?.DayChange.GetValueOrDefault(defaultValue: 0d));
            await this.MultiCoinReplyAsync(coins: coins,
                                           totalChange > 0
                                               ? Color.Green
                                               : Color.Red,
                                           title: "Snapshot",
                                           string.Join(separator: ", ", coins.Select(selector: c => c.Symbol)));
        }
    }

    [Command(text: "gainers")]
    [Summary(text: "get list of top 5 coins by Day Change of top 100 coins, e.g. !gainers")]
    public async Task GainersAsync()
    {
        using (this.Context.Channel.EnterTypingState())
        {
            IEnumerable<Currency> coins;

            try
            {
                coins = this._currencyManager.Get(predicate: x => x.Getdetails<CoinMarketCapCoin>()
                                                                   ?.Rank <= 100)
                            .OrderByDescending(keySelector: x => x.Getdetails<CoinMarketCapCoin>()
                                                                  ?.DayChange.GetValueOrDefault());
            }
            catch (Exception e)
            {
                this._logger.LogError(new(e.HResult), exception: e, message: e.Message);
                await this.ReplyAsync(message: "oops, something went wrong, sorry!");

                return;
            }

            await this.MultiCoinReplyAsync(coins.Take(count: 5)
                                                .ToList(),
                                           color: Color.Green,
                                           title: "Gainers",
                                           description: "The 5 coins in the top 100 with the biggest 24 hour gain");
        }
    }

    [Command(text: "losers")]
    [Summary(text: "get list of bottom 5 coins by Day Change of top 100 coins, e.g. !losers")]
    public async Task LosersAsync()
    {
        using (this.Context.Channel.EnterTypingState())
        {
            List<Currency> coins;

            try
            {
                coins = this._currencyManager.Get(predicate: x => x.Getdetails<CoinMarketCapCoin>()
                                                                   ?.Rank <= 100)
                            .RemoveNulls()
                            .OrderByDescending(keySelector: x => x.Getdetails<CoinMarketCapCoin>()
                                                                  ?.DayChange ?? 0d)
                            .ToList();
                coins.Reverse();
            }
            catch (Exception e)
            {
                this._logger.LogError(new(e.HResult), exception: e, message: e.Message);
                await this.ReplyAsync(message: "oops, something went wrong, sorry!");

                return;
            }

            await this.MultiCoinReplyAsync(coins.Take(count: 5)
                                                .ToList(),
                                           color: Color.Red,
                                           title: "Losers",
                                           description: "The 5 coins in the top 100 with the biggest 24 hour loss");
        }
    }

    private Task MultiCoinReplyAsync(IList<Currency> coins, Color color, string title, string description)
    {
        EmbedBuilder builder = new() { Color = color };
        builder.WithTitle(title);
        builder.WithDescription(description);
        AddAuthor(builder);
        AddFooter(builder: builder,
                  coins.Max(selector: c => c.Getdetails<CoinMarketCapCoin>()
                                            ?.LastUpdated.GetValueOrDefault(DateTime.MinValue)));

        foreach (Currency coin in coins)
        {
            CoinMarketCapCoin? details = coin.Getdetails<CoinMarketCapCoin>();

            if (details == null)
            {
                continue;
            }

            builder.Fields.Add(new()
                               {
                                   Name = $"{coin.Name} ({coin.Symbol}) | {details.DayChange.AsPercentage()} | {details.GetPriceSummary()}",
                                   Value =
                                       $"[{details.GetChangeSummary()}{Environment.NewLine}Cap {details.MarketCap.AsUsdPrice()} | Vol {details.Volume.AsUsdPrice()} | Rank {details.Rank}]({details.Url})"
                               });
        }

        return this.ReplyAsync(message: string.Empty, isTTS: false, builder.Build());
    }
}