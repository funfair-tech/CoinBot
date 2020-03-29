using System;

namespace CoinBot.Clients.CoinMarketCap
{
    public static class Helpers
    {
        public static Uri CurrencyImageUrl(string symbol)
        {
            return new Uri($"https://raw.githubusercontent.com/cjdowner/cryptocurrency-icons/master/128/color/{symbol.ToLowerInvariant()}.png");
        }
    }
}