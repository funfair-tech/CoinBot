using System;

namespace CoinBot.Core.Helpers;

public static class Images
{
    public static Uri CurrencyImageUrl(string symbol)
    {
        return new($"https://raw.githubusercontent.com/cjdowner/cryptocurrency-icons/master/128/color/{symbol.ToLowerInvariant()}.png");
    }
}