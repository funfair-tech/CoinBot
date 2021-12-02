using System;

namespace CoinBot.Core;

/// <summary>
///     Global coin market information.
/// </summary>
public interface IGlobalInfo
{
    /// <summary>
    ///     The combined market cap in USD.
    /// </summary>
    double? MarketCap { get; }

    /// <summary>
    ///     The combined 24H volume in USD.
    /// </summary>
    double? Volume { get; }

    /// <summary>
    ///     The bitcoin dominance in the crypto market as a percentage.
    /// </summary>
    double? BtcDominance { get; }

    /// <summary>
    ///     The amount of active currencies.
    /// </summary>
    int? Currencies { get; }

    /// <summary>
    ///     The amount of active assets.
    /// </summary>
    int? Assets { get; }

    /// <summary>
    ///     The amount of active markets.
    /// </summary>
    int? Markets { get; }

    /// <summary>
    ///     Indicates when this <see cref="IGlobalInfo" /> was last updated.
    /// </summary>
    DateTime? LastUpdated { get; }
}