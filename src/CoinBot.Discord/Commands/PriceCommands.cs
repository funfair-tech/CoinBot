using CoinBot.CoinSources;
using CoinBot.Discord.Extensions;
using Discord.Commands;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace CoinBot.Discord.Commands
{
    public class PriceCommands : ModuleBase
    {
        private readonly ICoinSource _coinSource;
        private readonly ILogger _logger;

        public PriceCommands(ICoinSource coinSource, ILogger logger)
        {
            this._coinSource = coinSource;
            this._logger = logger;
        }

        [Command("price"), Summary("get price info for a coin, e.g. !price FUN")]
        public async Task Price([Remainder, Summary("The symbol for the coin")] string symbol)
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
                    await ReplyAsync($"oops, something went wrong, sorry!");

                    return;
                }

                if (coin != null)
                {
                    await ReplyAsync($"{coin.Symbol} - ${coin.GetPriceSummary()} ({coin.GetChangeSummary()})");
                }
                else
                {
                    await ReplyAsync($"sorry, {symbol} was not found");
                }
            }
        }
    }
}
