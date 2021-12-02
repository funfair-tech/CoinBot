namespace CoinBot.Core.Extensions;

/// <summary>
///     <see cref="decimal" /> extension methods.
/// </summary>
public static class DecimalExtensions
{
    /// <summary>
    ///     The default response when formatting an input parameter that is null.
    /// </summary>
    private const string UNKNOWN_RESPONSE = "unknown";

    /// <summary>
    ///     Formats the <paramref name="d" /> as a currency.
    /// </summary>
    /// <param name="d">The value to format.</param>
    /// <returns></returns>
    public static string AsPrice(this decimal? d)
    {
        return d?.ToString(format: "#,##0.#################") ?? UNKNOWN_RESPONSE;
    }

    /// <summary>
    ///     todo
    /// </summary>
    /// <param name="d"></param>
    /// <returns></returns>
    public static string AsVolume(this decimal? d)
    {
        return d?.ToString(format: "N2") ?? UNKNOWN_RESPONSE;
    }
}