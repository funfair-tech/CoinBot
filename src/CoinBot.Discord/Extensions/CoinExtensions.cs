using CoinBot.CoinSources;
using System;

namespace CoinBot.Discord.Extensions
{
    public static class CoinExtensions
    {
        public static string GetChangeSummary(this ICoin coin)
        {
            return $"{coin.HourChange}% | {coin.DayChange}% | {coin.WeekChange}%";
        }

        public static string FormatPrice(this ICoin coin)
        {
            return coin.PriceUsd.FormatCurrencyValue();
        }

        public static string GetPriceSummary(this ICoin coin)
        {
            return $"{coin.FormatPrice()}/{coin.PriceBtc} BTC";
        }

        public static string FormatVolume(this ICoin coin)
        {
            return coin.Volume.FormatCurrencyValue();
        }

        public static string FormatMarketCap(this ICoin coin)
        {
            return coin.MarketCap.FormatCurrencyValue();
        }

        public static string GetCoinMarketCapLink(this ICoin coin)
        {
            return $"https://coinmarketcap.com/currencies/{coin.Id}/";
        }

        public static string GetCoinImageUrl(this ICoin coin)
        {
            return $"https://files.coinmarketcap.com/static/img/coins/64x64/{coin.Id}.png";
        }
    }
}
