using System.Text;
using CoinBot.Clients.CoinMarketCap;
using CoinBot.Core;
using CoinBot.Core.Extensions;

namespace CoinBot.Discord.Extensions
{
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
            changeStringBuilder.AppendLine($"Hour: {details.HourChange.AsPercentage()}");
            changeStringBuilder.AppendLine($"Day: {details.DayChange.AsPercentage()}");
            changeStringBuilder.AppendLine($"Week: {details.WeekChange.AsPercentage()}");

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
            descriptionBuilder.AppendLine($"Market cap {details.MarketCap.AsUsdPrice()} (Rank {details.Rank})");
            descriptionBuilder.AppendLine($"24 hour volume: {details.Volume.AsUsdPrice()}");

            return descriptionBuilder.ToString();
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
                priceStringBuilder.AppendLine($"{details.PriceBtc} BTC");
            }

            if (details.PriceEth != null)
            {
                priceStringBuilder.AppendLine($"{details.PriceEth} ETH");
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
}