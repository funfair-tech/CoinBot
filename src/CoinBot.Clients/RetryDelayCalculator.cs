using System;

namespace CoinBot.Clients
{
    internal static class RetryDelayCalculator
    {
        public static TimeSpan Calculate(int attempts)
        {
            return attempts > 1 ? TimeSpan.FromSeconds(Math.Pow(2.0, attempts)) : TimeSpan.Zero;
        }
    }
}