namespace CoinBot.Core;

/// <summary>
///     Builder of currencies.
/// </summary>
public interface ICoinBuilder
{
    /// <summary>
    ///     Gets a currency for the symbol
    /// </summary>
    /// <param name="symbol">Symbol to retrieve/build.</param>
    /// <param name="name">The name of the symbol.</param>
    /// <returns>The symbol.</returns>
    Currency Get(string symbol, string name);

    /// <summary>
    ///     Gets a currency for the symbol
    /// </summary>
    /// <param name="symbol">Symbol to retrieve/build.</param>
    /// <returns>The symbol.</returns>
    Currency? Get(string symbol);
}