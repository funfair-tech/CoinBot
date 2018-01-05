using CoinBot.Discord.Extensions;
using Discord.Commands;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using CoinBot.Clients.CoinMarketCap;
using CoinBot.Core;

namespace CoinBot.Discord.Commands
{
	public class PriceCommands : CommandBase
	{
		private readonly CurrencyManager _currencyManager;
		private readonly ILogger _logger;

		public PriceCommands(CurrencyManager currencyManager, ILogger logger)
		{
			_currencyManager = currencyManager;
			_logger = logger;
		}

		[Command("price"), Summary("get price info for a coin, e.g. !price FUN")]
		public async Task Price([Remainder, Summary("The symbol for the coin")] string symbol)
		{
			using (Context.Channel.EnterTypingState())
			{
				try
				{
					var currency = _currencyManager.Get(symbol);
					
					if (currency != null)
					{
						var details = currency.Getdetails<CoinMarketCapCoin>();
						await ReplyAsync($"{currency.Symbol} - ${details.GetPriceSummary()} ({details.GetChangeSummary()})");
					}
					else
					{
						await ReplyAsync($"sorry, {symbol} was not found");
					}
				}
				catch (Exception e)
				{
					_logger.LogError(new EventId(e.HResult), e, e.Message);
					await ReplyAsync($"oops, something went wrong, sorry!");

					return;
				}
			}
		}
	}
}
