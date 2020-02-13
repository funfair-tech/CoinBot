using System;
using System.Threading.Tasks;
using CoinBot.Clients.CoinMarketCap;
using CoinBot.Core;
using CoinBot.Discord.Extensions;
using Discord.Commands;
using Microsoft.Extensions.Logging;

namespace CoinBot.Discord.Commands
{
    public class PriceCommands : CommandBase
    {
        private readonly CurrencyManager _currencyManager;
        private readonly ILogger _logger;

        public PriceCommands(CurrencyManager currencyManager, ILogger logger)
        {
            this._currencyManager = currencyManager;
            this._logger = logger;
        }

        [Command("price"), Summary("get price info for a coin, e.g. !price FUN")]
        public async Task PriceAsync([Remainder, Summary("The symbol for the coin")]
                                     string symbol)
        {
            using (this.Context.Channel.EnterTypingState())
            {
                try
                {
                    Currency currency = this._currencyManager.Get(symbol);

                    if (currency != null)
                    {
                        CoinMarketCapCoin details = currency.Getdetails<CoinMarketCapCoin>();
                        await this.ReplyAsync($"{currency.Symbol} - ${details.GetPriceSummary()} ({details.GetChangeSummary()})");
                    }
                    else
                    {
                        await this.ReplyAsync($"sorry, {symbol} was not found");
                    }
                }
                catch (Exception e)
                {
                    this._logger.LogError(new EventId(e.HResult), e, e.Message);
                    await this.ReplyAsync($"oops, something went wrong, sorry!");
                }
            }
        }
    }
}