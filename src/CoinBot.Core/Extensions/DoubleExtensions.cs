using System;
using System.Globalization;

namespace CoinBot.Core.Extensions;

/// <summary>
///     <see cref="double" /> extension methods.
/// </summary>
public static class DoubleExtensions
{
    /// <summary>
    ///     The default response when formatting an input parameter that is null.
    /// </summary>
    private const string UNKNOWN_RESPONSE = "unknown";

    /// <summary>
    ///     The US <see cref="CultureInfo" />.
    /// </summary>
    private static readonly CultureInfo UsdCulture = new(name: "en-US");

    /// <summary>
    ///     Formats the <paramref name="d" /> as a USD currency.
    /// </summary>
    /// <param name="d">The value to format.</param>
    /// <param name="precision">The precision to use.</param>
    /// <returns></returns>
    public static string AsUsdPrice(this decimal? d, int? precision = null)
    {
        double? dbl = null;

        if (d != null)
        {
            dbl = Convert.ToDouble(d.Value);
        }

        return dbl.AsUsdPrice(precision);
    }

    /// <summary>
    ///     Formats the <paramref name="d" /> as a USD currency.
    /// </summary>
    /// <param name="d">The value to format.</param>
    /// <param name="precision">The precision to use.</param>
    /// <returns></returns>
    public static string AsUsdPrice(this double? d, int? precision = null)
    {
        return d?.ToString($"c{precision}", provider: UsdCulture) ?? UNKNOWN_RESPONSE;
    }

    /// <summary>
    ///     Formats the <paramref name="d" /> as a percentage.
    /// </summary>
    /// <param name="d">The value to format.</param>
    /// <returns></returns>
    public static string AsPercentage(this double? d)
    {
        return d != null
            ? $"{d}%"
            : UNKNOWN_RESPONSE;
    }
}