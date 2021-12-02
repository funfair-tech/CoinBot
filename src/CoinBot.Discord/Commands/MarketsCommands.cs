using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinBot.Clients.CoinMarketCap;
using CoinBot.Core;
using CoinBot.Core.Extensions;
using CoinBot.Discord.Extensions;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.Logging;

namespace CoinBot.Discord.Commands;

public sealed class MarketsCommands : CommandBase
{
    private readonly CurrencyManager _currencyManager;
    private readonly ILogger<MarketsCommands> _logger;
    private readonly MarketManager _marketManager;

    private readonly char[] _separators =
    {
        '-',
        '/',
        '\\',
        ','
    };

    public MarketsCommands(CurrencyManager currencyManager, MarketManager marketManager, ILogger<MarketsCommands> logger)
    {
        this._currencyManager = currencyManager ?? throw new ArgumentNullException(nameof(currencyManager));
        this._marketManager = marketManager ?? throw new ArgumentNullException(nameof(marketManager));
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [Command(text: "markets")]
    [Summary(text: "get price details per market for a coin, e.g. `!markets FUN` or `!markets ETH/FUN`.")]
    public async Task MarketsAsync([Remainder] [Summary(text: "The input as a single coin symbol or a pair")] string input)
    {
        using (this.Context.Channel.EnterTypingState())
        {
            try
            {
                if (!this.GetCurrencies(input: input, out Currency? primaryCurrency, out Currency? secondaryCurrency))
                {
                    await this.ReplyAsync($"Oops! I did not understand `{input}`.");

                    return;
                }

                List<MarketSummaryDto> markets = secondaryCurrency == null
                    ? this._marketManager.Get(primaryCurrency)
                          .ToList()
                    : this._marketManager.GetPair(currency1: primaryCurrency, currency2: secondaryCurrency)
                          .ToList();

                if (markets.Count == 0)
                {
                    await this.ReplyAsync($"sorry, no market details found for {input}");

                    return;
                }

                EmbedBuilder builder = new();
                builder.WithTitle(primaryCurrency.GetTitle());
                CoinMarketCapCoin? details = primaryCurrency.Getdetails<CoinMarketCapCoin>();

                if (details != null)
                {
                    builder.WithUrl(details.Url);
                }

                AddAuthor(builder);

                if (primaryCurrency.ImageUrl != null)
                {
                    builder.WithThumbnailUrl(primaryCurrency.ImageUrl);
                }

                // Group by exchange, and if looking for a pair orderby volume
                IEnumerable<IGrouping<string, MarketSummaryDto>> grouped = secondaryCurrency != null
                    ? markets.GroupBy(keySelector: m => m.Market)
                             .OrderByDescending(keySelector: g => g.Sum(selector: m => m.Volume * m.Last))
                    : markets.GroupBy(keySelector: m => m.Market);

                foreach (IGrouping<string, MarketSummaryDto> group in grouped)
                {
                    string exchangeName = group.Key;
                    const int maxResults = 3;
                    int totalResults = group.Count();

                    StringBuilder marketDetails = new();

                    if (secondaryCurrency == null && totalResults > maxResults)
                    {
                        int diff = totalResults - maxResults;

                        if (totalResults < 10)
                        {
                            WriteMarketSummaries(builder: marketDetails, group.Take(maxResults));

                            marketDetails.AppendLine()
                                         .Append("Found ")
                                         .Append(diff)
                                         .Append(" more ")
                                         .Append(primaryCurrency.Symbol)
                                         .Append(" market(s) at ")
                                         .Append(exchangeName)
                                         .AppendLine(":")
                                         .AppendJoin(separator: ", ",
                                                     group.Skip(maxResults)
                                                          .Select(selector: m => $"{m.BaseCurrency.Symbol}/{m.MarketCurrency.Symbol}"))
                                         .AppendLine();
                        }
                        else
                        {
                            marketDetails.Append("`Found ")
                                         .Append(diff)
                                         .Append(" more ")
                                         .Append(primaryCurrency.Symbol)
                                         .Append(" market(s) at ")
                                         .Append(exchangeName)
                                         .AppendLine(".`");
                        }
                    }
                    else
                    {
                        WriteMarketSummaries(builder: marketDetails, markets: group);
                    }

                    builder.AddField($"{exchangeName}", value: marketDetails);
                }

                DateTime? lastUpdated = markets.Min(selector: m => m.LastUpdated);
                AddFooter(builder: builder, dateTime: lastUpdated);

                string operationName = secondaryCurrency != null
                    ? $"{primaryCurrency.Name}/{secondaryCurrency.Name}"
                    : primaryCurrency.Name;

                await this.ReplyAsync($"Markets for `{operationName}`:", isTTS: false, builder.Build());
            }
            catch (Exception e)
            {
                this._logger.LogError(eventId: 0, exception: e, $"Something went wrong while processing the !markets command with input '{input}'");
                await this.ReplyAsync(message: "oops, something went wrong, sorry!");
            }
        }
    }

    private static void WriteMarketSummaries(StringBuilder builder, IEnumerable<MarketSummaryDto> markets)
    {
        builder.Append(value: "```");

        foreach (MarketSummaryDto market in markets)
        {
            builder.AppendLine(market.GetSummary());
        }

        builder.Append(value: "```");
    }

    private bool GetCurrencies(string input, [NotNullWhen(returnValue: true)] out Currency? primaryCurrency, out Currency? secondaryCurrency)
    {
        primaryCurrency = null;
        secondaryCurrency = null;
        int countSeparators = input.Count(predicate: c => this._separators.Contains(c));

        if (countSeparators > 1)
        {
            return false;
        }

        if (countSeparators == 0)
        {
            primaryCurrency = this._currencyManager.Get(input);
        }
        else
        {
            string first = input.Substring(startIndex: 0, input.IndexOfAny(this._separators));
            string second = input.Substring(input.IndexOfAny(this._separators) + 1);
            primaryCurrency = this._currencyManager.Get(first);
            secondaryCurrency = this._currencyManager.Get(second);
        }

        return primaryCurrency != null;
    }
}