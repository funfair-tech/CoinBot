using Discord;
using Discord.Commands;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using CoinBot.Clients.CoinMarketCap;
using CoinBot.Core;
using CoinBot.Core.Extensions;
using CoinBot.Discord.Extensions;

namespace CoinBot.Discord.Commands
{
	public class CoinCommands : CommandBase
	{
		private readonly CurrencyManager _currencyManager;
		private readonly ILogger _logger;

		public CoinCommands(CurrencyManager currencyManager, ILogger logger)
		{
			_currencyManager = currencyManager;
			_logger = logger;
		}

		[Command("coin"), Summary("get info for a coin, e.g. !coin FUN")]
		public async Task Coin([Remainder, Summary("The symbol for the coin")] string symbol)
		{
			using (Context.Channel.EnterTypingState())
			{
				try
				{
					var currency = _currencyManager.Get(symbol);

					if (currency != null)
					{
						var builder = new EmbedBuilder();
						builder.WithTitle(currency.GetTitle());

						var details = currency.Getdetails<CoinMarketCapCoin>();
						if (details != null)
						{
							builder.Color = details.DayChange > 0 ? Color.Green : Color.Red;
							AddAuthor(builder);

							builder.WithDescription(details.GetDescription());
							builder.WithUrl(details.Url);
							if (currency.ImageUrl != null)
								builder.WithThumbnailUrl(currency.ImageUrl);
							builder.AddInlineField("Price", details.GetPrice());
							builder.AddInlineField("Change", details.GetChange());
							AddFooter(builder, details.LastUpdated);
						}

						await ReplyAsync(string.Empty, false, builder.Build());
					}
					else
					{
						await ReplyAsync($"sorry, {symbol} was not found");
					}
				}
				catch (Exception e)
				{
					_logger.LogError(new EventId(e.HResult), e, e.Message);
					await ReplyAsync("oops, something went wrong, sorry!");

					return;
				}
			}
		}

		[Command("snapshot"), Summary("get info on a list of coins, !snapshot FUN,BTC,IOTA,ETH,ETC")]
		public async Task Snapshot([Remainder, Summary("A comma separated list of coin symbols")] string symbols)
		{
			using (Context.Channel.EnterTypingState())
			{
				var symbolsList = symbols.Split(',').Select(s => s.Trim()).ToArray();
				var coins = new List<Currency>();
				IList<string> notFound = new List<string>();

				foreach (var symbol in symbolsList)
				{
					try
					{
						var currency = _currencyManager.Get(symbol);
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
						_logger.LogError(new EventId(e.HResult), e, e.Message);
						await ReplyAsync("oops, something went wrong, sorry!");

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

				var totalChange = coins.Sum(c => c.Getdetails<CoinMarketCapCoin>().DayChange);
				await MultiCoinReply(coins, totalChange > 0 ? Color.Green : Color.Red, "Snapshot", string.Join(", ", coins.Select(c => c.Symbol)));
			}
		}

		[Command("gainers"), Summary("get list of top 5 coins by Day Change of top 100 coins, e.g. !gainers")]
		public async Task Gainers()
		{
			using (Context.Channel.EnterTypingState())
			{
				IEnumerable<Currency> coins;
				try
				{
					coins = _currencyManager.Get(x =>
							x.Getdetails<CoinMarketCapCoin>()?.Rank <= 100)
						.OrderByDescending(x => x.Getdetails<CoinMarketCapCoin>().DayChange);
				}
				catch (Exception e)
				{
					_logger.LogError(new EventId(e.HResult), e, e.Message);
					await ReplyAsync("oops, something went wrong, sorry!");

					return;
				}

				await MultiCoinReply(coins.Take(5).ToList(), Color.Green, "Gainers", "The 5 coins in the top 100 with the biggest 24 hour gain");
			}
		}

		[Command("losers"), Summary("get list of bottom 5 coins by Day Change of top 100 coins, e.g. !losers")]
		public async Task Losers()
		{
			using (Context.Channel.EnterTypingState())
			{
				List<Currency> coins;
				try
				{
					coins = _currencyManager.Get(x =>
							x.Getdetails<CoinMarketCapCoin>()?.Rank <= 100)
						.OrderByDescending(x => x.Getdetails<CoinMarketCapCoin>().DayChange)
						.ToList();
					coins.Reverse();
				}
				catch (Exception e)
				{
					_logger.LogError(new EventId(e.HResult), e, e.Message);
					await ReplyAsync("oops, something went wrong, sorry!");

					return;
				}

				await MultiCoinReply(coins.Take(5).ToList(), Color.Red, "Losers", "The 5 coins in the top 100 with the biggest 24 hour loss");
			}
		}

		private async Task MultiCoinReply(IList<Currency> coins, Color color, string title, string description)
		{
			var builder = new EmbedBuilder();
			builder.WithTitle(title);
			builder.WithDescription(description);
			builder.Color = color;
			AddAuthor(builder);
			AddFooter(builder, coins.Max(c => c.Getdetails<CoinMarketCapCoin>().LastUpdated));

			foreach (var coin in coins)
			{
				var details = coin.Getdetails<CoinMarketCapCoin>();
				builder.Fields.Add(new EmbedFieldBuilder
				{
					Name = $"{coin.Name} ({coin.Symbol}) | {details.DayChange.AsPercentage()} | {details.GetPriceSummary()}",
					Value = $"[{details.GetChangeSummary()}{Environment.NewLine}Cap {details.MarketCap.AsUsdPrice()} | Vol {details.Volume.AsUsdPrice()} | Rank {details.Rank}]({details.Url})"
				});
			}

			await ReplyAsync(string.Empty, false, builder.Build());
		}
	}
}
