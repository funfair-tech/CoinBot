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
			this._currencyManager = currencyManager;
		    this._logger = logger;
		}

		[Command("coin"), Summary("get info for a coin, e.g. !coin FUN")]
		public async Task Coin([Remainder, Summary("The symbol for the coin")] string symbol)
		{
			using (this.Context.Channel.EnterTypingState())
			{
				try
				{
					Currency currency = this._currencyManager.Get(symbol);

					if (currency != null)
					{
						EmbedBuilder builder = new EmbedBuilder();
						builder.WithTitle(currency.GetTitle());

						CoinMarketCapCoin details = currency.Getdetails<CoinMarketCapCoin>();
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

						await this.ReplyAsync(string.Empty, false, builder.Build());
					}
					else
					{
						await this.ReplyAsync($"sorry, {symbol} was not found");
					}
				}
				catch (Exception e)
				{
				    this._logger.LogError(new EventId(e.HResult), e, e.Message);
					await this.ReplyAsync("oops, something went wrong, sorry!");

					return;
				}
			}
		}

		[Command("snapshot"), Summary("get info on a list of coins, !snapshot FUN,BTC,IOTA,ETH,ETC")]
		public async Task Snapshot([Remainder, Summary("A comma separated list of coin symbols")] string symbols)
		{
			using (this.Context.Channel.EnterTypingState())
			{
				string[] symbolsList = symbols.Split(',').Select(s => s.Trim()).ToArray();
				List<Currency> coins = new List<Currency>();
				IList<string> notFound = new List<string>();

				foreach (string symbol in symbolsList)
				{
					try
					{
						Currency currency = this._currencyManager.Get(symbol);
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
					    this._logger.LogError(new EventId(e.HResult), e, e.Message);
						await this.ReplyAsync("oops, something went wrong, sorry!");

						return;
					}
				}

				if (notFound.Count > 0)
				{
					if (notFound.Count > 1)
					{
						await this.ReplyAsync($"sorry, {string.Join(", ", notFound)} were not found");
					}
					else
					{
						await this.ReplyAsync($"sorry, {notFound[0]} was not found");
					}
				}

				double? totalChange = coins.Sum(c => c.Getdetails<CoinMarketCapCoin>().DayChange);
				await this.MultiCoinReply(coins, totalChange > 0 ? Color.Green : Color.Red, "Snapshot", string.Join(", ", coins.Select(c => c.Symbol)));
			}
		}

		[Command("gainers"), Summary("get list of top 5 coins by Day Change of top 100 coins, e.g. !gainers")]
		public async Task Gainers()
		{
			using (this.Context.Channel.EnterTypingState())
			{
				IEnumerable<Currency> coins;
				try
				{
					coins = this._currencyManager.Get(x =>
							x.Getdetails<CoinMarketCapCoin>()?.Rank <= 100)
						.OrderByDescending(x => x.Getdetails<CoinMarketCapCoin>().DayChange);
				}
				catch (Exception e)
				{
				    this._logger.LogError(new EventId(e.HResult), e, e.Message);
					await this.ReplyAsync("oops, something went wrong, sorry!");

					return;
				}

				await this.MultiCoinReply(coins.Take(5).ToList(), Color.Green, "Gainers", "The 5 coins in the top 100 with the biggest 24 hour gain");
			}
		}

		[Command("losers"), Summary("get list of bottom 5 coins by Day Change of top 100 coins, e.g. !losers")]
		public async Task Losers()
		{
			using (this.Context.Channel.EnterTypingState())
			{
				List<Currency> coins;
				try
				{
					coins = this._currencyManager.Get(x =>
							x.Getdetails<CoinMarketCapCoin>()?.Rank <= 100)
						.OrderByDescending(x => x.Getdetails<CoinMarketCapCoin>().DayChange)
						.ToList();
					coins.Reverse();
				}
				catch (Exception e)
				{
				    this._logger.LogError(new EventId(e.HResult), e, e.Message);
					await this.ReplyAsync("oops, something went wrong, sorry!");

					return;
				}

				await this.MultiCoinReply(coins.Take(5).ToList(), Color.Red, "Losers", "The 5 coins in the top 100 with the biggest 24 hour loss");
			}
		}

		private async Task MultiCoinReply(IList<Currency> coins, Color color, string title, string description)
		{
			EmbedBuilder builder = new EmbedBuilder();
			builder.WithTitle(title);
			builder.WithDescription(description);
			builder.Color = color;
			AddAuthor(builder);
			AddFooter(builder, coins.Max(c => c.Getdetails<CoinMarketCapCoin>().LastUpdated));

			foreach (Currency coin in coins)
			{
				CoinMarketCapCoin details = coin.Getdetails<CoinMarketCapCoin>();
				builder.Fields.Add(new EmbedFieldBuilder
				{
					Name = $"{coin.Name} ({coin.Symbol}) | {details.DayChange.AsPercentage()} | {details.GetPriceSummary()}",
					Value = $"[{details.GetChangeSummary()}{Environment.NewLine}Cap {details.MarketCap.AsUsdPrice()} | Vol {details.Volume.AsUsdPrice()} | Rank {details.Rank}]({details.Url})"
				});
			}

			await this.ReplyAsync(string.Empty, false, builder.Build());
		}
	}
}
