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

namespace CoinBot.Discord.Commands
{
    public sealed class MarketsCommands : CommandBase
    {
        private readonly CurrencyManager _currencyManager;
        private readonly ILogger _logger;
        private readonly MarketManager _marketManager;

        private readonly char[] _separators = {'-', '/', '\\', ','};

        public MarketsCommands(CurrencyManager currencyManager, MarketManager marketManager, ILogger logger)
        {
            this._currencyManager = currencyManager ?? throw new ArgumentNullException(nameof(currencyManager));
            this._marketManager = marketManager ?? throw new ArgumentNullException(nameof(marketManager));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [Command("markets")]
        [Summary("get price details per market for a coin, e.g. `!markets FUN` or `!markets ETH/FUN`.")]
        public async Task MarketsAsync([Remainder] [Summary("The input as a single coin symbol or a pair")]
                                       string input)
        {
            using (this.Context.Channel.EnterTypingState())
                try
                {
                    if (!this.GetCurrencies(input, out Currency? primaryCurrency, out Currency? secondaryCurrency))
                    {
                        await this.ReplyAsync($"Oops! I did not understand `{input}`.");

                        return;
                    }

                    List<MarketSummaryDto> markets = secondaryCurrency == null
                        ? this._marketManager.Get(primaryCurrency)
                              .ToList()
                        : this._marketManager.GetPair(primaryCurrency, secondaryCurrency)
                              .ToList();

                    if (!markets.Any())
                    {
                        await this.ReplyAsync($"sorry, no market details found for {input}");

                        return;
                    }

                    EmbedBuilder builder = new EmbedBuilder();
                    builder.WithTitle(primaryCurrency.GetTitle());
                    CoinMarketCapCoin details = primaryCurrency.Getdetails<CoinMarketCapCoin>();

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
                        ? markets.GroupBy(m => m.Market)
                                 .OrderByDescending(g => g.Sum(m => m.Volume * m.Last))
                        : markets.GroupBy(m => m.Market);

                    foreach (IGrouping<string, MarketSummaryDto> group in grouped)
                    {
                        string exchangeName = group.Key;
                        const int maxResults = 3;
                        int totalResults = group.Count();

                        StringBuilder marketDetails = new StringBuilder();

                        if (secondaryCurrency == null && totalResults > maxResults)
                        {
                            int diff = totalResults - maxResults;

                            if (totalResults < 10)
                            {
                                WriteMarketSummaries(marketDetails, group.Take(maxResults));

                                marketDetails.AppendLine();
                                marketDetails.AppendLine($"Found {diff} more {primaryCurrency.Symbol} market(s) at {exchangeName}:");
                                marketDetails.AppendLine(string.Join(", ",
                                                                     group.Skip(maxResults)
                                                                          .Select(m => $"{m.BaseCurrency.Symbol}/{m.MarketCurrency.Symbol}")));
                            }
                            else
                            {
                                marketDetails.AppendLine($"`Found {diff} more {primaryCurrency.Symbol} market(s) at {exchangeName}.`");
                            }
                        }
                        else
                        {
                            WriteMarketSummaries(marketDetails, group);
                        }

                        builder.AddField($"{exchangeName}", marketDetails);
                    }

                    DateTime? lastUpdated = markets.Min(m => m.LastUpdated);
                    AddFooter(builder, lastUpdated);

                    string operationName = (secondaryCurrency != null) ? $"{primaryCurrency.Name}/{secondaryCurrency.Name}" : primaryCurrency.Name;

                    await this.ReplyAsync($"Markets for `{operationName}`:", false, builder.Build());
                }
                catch (Exception e)
                {
                    this._logger.LogError(0, e, $"Something went wrong while processing the !markets command with input '{input}'");
                    await this.ReplyAsync("oops, something went wrong, sorry!");
                }
        }

        private static void WriteMarketSummaries(StringBuilder builder, IEnumerable<MarketSummaryDto> markets)
        {
            builder.Append("```");

            foreach (MarketSummaryDto market in markets)
            {
                builder.AppendLine(market.GetSummary());
            }

            builder.Append("```");
        }

        private bool GetCurrencies(string input, [NotNullWhen(true)] out Currency? primaryCurrency, [NotNullWhen(true)] out Currency? secondaryCurrency)
        {
            primaryCurrency = null;
            secondaryCurrency = null;
            int countSeparators = input.Count(c => this._separators.Contains(c));

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
                string first = input.Substring(0, input.IndexOfAny(this._separators));
                string second = input.Substring(input.IndexOfAny(this._separators) + 1);
                primaryCurrency = this._currencyManager.Get(first);
                secondaryCurrency = this._currencyManager.Get(second);
            }

            return true;
        }
    }
}