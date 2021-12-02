using System.Text;
using CoinBot.Clients.CoinMarketCap;
using CoinBot.Core;
using CoinBot.Core.Extensions;

namespace CoinBot.Discord.Extensions;

/// <summary>
///     <see cref="CoinMarketCapCoin" /> extension methods.
/// </summary>
public static class CoinMarketCapCoinExtensions
{
    /// <summary>
    ///     The USD precision to use when formatting <see cref="CoinMarketCapCoin" /> prices.
    /// </summary>
    private const int USD_PRICE_PRECISION = 7;

    /// <summary>
    ///     Get information about the price change from last hour, day and week.
    /// </summary>
    /// <param name="details">The <see cref="CoinMarketCapCoin" />.</param>
    /// <returns></returns>
    public static string GetChange(this CoinMarketCapCoin details)
    {
        StringBuilder changeStringBuilder = new();
        changeStringBuilder.Append("Hour: ")
                           .AppendLine(details.HourChange.AsPercentage())
                           .Append("Day: ")
                           .AppendLine(details.DayChange.AsPercentage())
                           .Append("Week: ")
                           .AppendLine(details.WeekChange.AsPercentage());

        return changeStringBuilder.ToString();
    }

    /// <summary>
    ///     Get a summary about the price change from last hour, day and week.
    /// </summary>
    /// <param name="details"></param>
    /// <returns></returns>
    public static string GetChangeSummary(this CoinMarketCapCoin details)
    {
        return $"{details.HourChange.AsPercentage()} | {details.DayChange.AsPercentage()} | {details.WeekChange.AsPercentage()}";
    }

    /// <summary>
    ///     Get the <paramref name="details" /> description, including market cap, rank and 24H volume.
    /// </summary>
    /// <param name="details">The <see cref="CoinMarketCapCoin" />.</param>
    /// <returns></returns>
    public static string GetDescription(this CoinMarketCapCoin details)
    {
        StringBuilder descriptionBuilder = new();

        return descriptionBuilder.Append("Market cap ")
                                 .Append(details.MarketCap.AsUsdPrice())
                                 .Append(" (Rank ")
                                 .Append(details.Rank)
                                 .AppendLine(")")
                                 .Append("24 hour volume: ")
                                 .AppendLine(details.Volume.AsUsdPrice())
                                 .ToString();
    }

    /// <summary>
    ///     Get the <paramref name="details" /> price in USD, BTC and ETH.
    /// </summary>
    /// <param name="details">The <see cref="CoinMarketCapCoin" />.</param>
    /// <returns></returns>
    public static string GetPrice(this ICoinInfo details)
    {
        StringBuilder priceStringBuilder = new();

        if (details.PriceUsd != null)
        {
            priceStringBuilder.AppendLine(details.PriceUsd.AsUsdPrice(USD_PRICE_PRECISION));
        }

        if (details.PriceBtc != null)
        {
            priceStringBuilder.Append(details.PriceBtc)
                              .AppendLine(" BTC");
        }

        if (details.PriceEth != null)
        {
            priceStringBuilder.Append(details.PriceEth)
                              .AppendLine(" ETH");
        }

        return priceStringBuilder.ToString();
    }

    /// <summary>
    ///     Get the <paramref name="details" /> price summary in USD and BTC.
    /// </summary>
    /// <param name="details"></param>
    /// <returns></returns>
    public static string GetPriceSummary(this CoinMarketCapCoin details)
    {
        return $"{details.PriceUsd.AsUsdPrice(USD_PRICE_PRECISION)}/{details.PriceBtc} BTC";
    }
}