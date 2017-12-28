using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoinBot.Clients.CoinMarketCap;
using CoinBot.Core;
using CoinBot.Core.Extensions;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.Logging;

namespace CoinBot.Discord.Commands
{
	public class MarketsCommands : CommandBase
	{
		private readonly CurrencyManager _currencyManager;
		private readonly ILogger _logger;
		private readonly MarketManager _marketManager;

		private readonly char[] _separators = { '-', '/', '\\', ',' };

		public MarketsCommands(CurrencyManager currencyManager, MarketManager marketManager, ILogger logger)
		{
			_currencyManager = currencyManager ?? throw new ArgumentNullException(nameof(currencyManager));
			_marketManager = marketManager ?? throw new ArgumentNullException(nameof(marketManager));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		[Command("markets")]
		[Summary("get price details per market for a coin, e.g. `!markets FUN` or `!markets ETH/FUN`.")]
		public async Task Markets([Remainder] [Summary("The input as a single coin symbol or a pair")] string input)
		{
			using (Context.Channel.EnterTypingState())
				try
				{
					if (!GetCurrencies(input, out var primaryCurrency, out var secondaryCurrency))
					{
						await ReplyAsync($"Oops! I did not understand `{input}`.");
						return;
					}

					var markets = secondaryCurrency == null
						? _marketManager.Get(primaryCurrency).ToList()
						: _marketManager.GetPair(primaryCurrency, secondaryCurrency).ToList();

					if (!markets.Any())
					{
						await ReplyAsync($"sorry, no market details found for {input}");
						return;
					}

					var builder = new EmbedBuilder();
					builder.WithTitle(primaryCurrency.GetTitle());
					var details = primaryCurrency.Getdetails<CoinMarketCapCoin>();
					if (details != null)
						builder.WithUrl(details.Url);
					AddAuthor(builder);
					if (primaryCurrency.ImageUrl != null)
						builder.WithThumbnailUrl(primaryCurrency.ImageUrl);

					// Group by exchange, and if looking for a pair orderby volume
					var grouped = secondaryCurrency != null
						? markets
							.GroupBy(m => m.Market)
							.OrderByDescending(g => g.Sum(m => m.Volume * m.Last))
						: markets
							.GroupBy(m => m.Market);

					foreach (var group in grouped)
					{
						const int maxResults = 3;
						var totalResults = group.Count();
						var exchangeName = group.Key;
						var marketDetails = new StringBuilder();
						foreach (var market in group.Take(maxResults))
							marketDetails.AppendLine($"{market.BaseCurrrency.Symbol}/{market.MarketCurrency.Symbol}: {market.Last.AsPrice()} (24h Vol.: {market.Volume.AsVolume()})");
						if (totalResults > maxResults)
						{
							var diff = totalResults - maxResults;
							marketDetails.AppendLine($"Found {diff} more {primaryCurrency.Symbol} market(s) at {exchangeName}");
						}

						builder.AddField(exchangeName, marketDetails);
					}

					var lastUpdated = markets.Min(m => m.LastUpdated);
					AddFooter(builder, lastUpdated);

					await ReplyAsync(string.Empty, false, builder.Build());
				}
				catch (Exception e)
				{
					_logger.LogError(0, e, $"Something went wrong while processing the !markets command with input '{input}'");
					await ReplyAsync("oops, something went wrong, sorry!");
				}
		}

		private bool GetCurrencies(string input, out Currency primaryCurrency, out Currency secondaryCurrency)
		{
			primaryCurrency = null;
			secondaryCurrency = null;
			var countSeparators = input.Count(c => _separators.Contains(c));
			if (countSeparators > 1)
				return false;

			if (countSeparators == 0)
				primaryCurrency = _currencyManager.Get(input);
			else
			{
				var first = input.Substring(0, input.IndexOfAny(_separators));
				var second = input.Substring(input.IndexOfAny(_separators) + 1);
				primaryCurrency = _currencyManager.Get(first);
				secondaryCurrency = _currencyManager.Get(second);
			}

			return true;
		}
	}
}
