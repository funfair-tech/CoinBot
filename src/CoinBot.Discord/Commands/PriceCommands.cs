using System;
using System.Threading.Tasks;
using CoinBot.Clients.CoinMarketCap;
using CoinBot.Clients.FunFair;
using CoinBot.Core;
using CoinBot.Discord.Extensions;
using Discord.Commands;
using Microsoft.Extensions.Logging;

namespace CoinBot.Discord.Commands;

public sealed class PriceCommands : CommandBase
{
    private readonly CurrencyManager _currencyManager;
    private readonly ILogger<PriceCommands> _logger;

    public PriceCommands(CurrencyManager currencyManager, ILogger<PriceCommands> logger)
    {
        this._currencyManager = currencyManager;
        this._logger = logger;
    }

    [Command(text: "price")]
    [Summary(text: "get price info for a coin, e.g. !price FUN")]
    public async Task PriceAsync([Remainder] [Summary(text: "The symbol for the coin")] string symbol)
    {
        using (this.Context.Channel.EnterTypingState())
        {
            try
            {
                Currency? currency = this._currencyManager.Get(symbol);

                if (currency != null)
                {
                    CoinMarketCapCoin? details = currency.Getdetails<CoinMarketCapCoin>();

                    if (details != null)
                    {
                        await this.ReplyAsync($"{currency.Symbol} - ${details.GetPriceSummary()} ({details.GetChangeSummary()})");

                        return;
                    }

                    FunFairWalletCoin? walletCoinDetails = currency.Getdetails<FunFairWalletCoin>();

                    if (walletCoinDetails != null)
                    {
                        await this.ReplyAsync($"{currency.Symbol} - ${walletCoinDetails.GetPriceSummary()}");

                        return;
                    }
                }

                await this.ReplyAsync($"sorry, {symbol} was not found");
            }
            catch (Exception e)
            {
                this._logger.LogError(new(e.HResult), exception: e, message: e.Message);
                await this.ReplyAsync(message: "oops, something went wrong, sorry!");
            }
        }
    }
}