using System;

namespace CoinBot.Discord.Extensions
{
    public static class StringExtensions
    {
        public static string FormatCurrencyValue(this string s)
        {
            decimal value = Convert.ToDecimal(s);
            return value.ToString("#,##0.#################");
        }

        public static string FormatNumber(this string s)
        {
            int value = Convert.ToInt32(s);
            return value.ToString("#,##0");
        }
    }
}
