namespace CoinBot.Core.Extensions;

public static class CurrencyExtensions
{
    /// <summary>
    ///     Get the <paramref name="currency" /> title.
    /// </summary>
    /// <param name="currency">The <see cref="Currency" />.</param>
    /// <returns></returns>
    public static string GetTitle(this Currency currency)
    {
        return $"{currency.Name} ({currency.Symbol})";
    }
}