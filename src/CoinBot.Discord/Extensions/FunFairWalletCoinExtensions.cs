using CoinBot.Clients.FunFair;
using CoinBot.Core.Extensions;

namespace CoinBot.Discord.Extensions;

public static class FunFairWalletCoinExtensions
{
    /// <summary>
    ///     The USD precision to use when formatting <see cref="FunFairWalletCoin" /> prices.
    /// </summary>
    private const int USD_PRICE_PRECISION = 7;

    /// <summary>
    ///     Get the <paramref name="details" /> price summary in USD and BTC.
    /// </summary>
    /// <param name="details"></param>
    /// <returns></returns>
    public static string GetPriceSummary(this FunFairWalletCoin details)
    {
        return $"{details.PriceUsd.AsUsdPrice(USD_PRICE_PRECISION)}";
    }
}