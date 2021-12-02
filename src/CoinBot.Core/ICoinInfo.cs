using System;
using System.Diagnostics.CodeAnalysis;

namespace CoinBot.Core;

/// <summary>
///     Coin information.
/// </summary>
public interface ICoinInfo
{
    /// <summary>
    ///     The coin identifier.
    /// </summary>
    string Id { get; }

    /// <summary>
    ///     The coin image.
    /// </summary>
    [SuppressMessage(category: "Microsoft.Design", checkId: "CA1056:UriPropertiesShouldNotBeStrings", Justification = "Model for serialization")]
    string ImageUrl { get; }

    /// <summary>
    ///     The coin name.
    /// </summary>
    string Name { get; }

    /// <summary>
    ///     The token symbol.
    /// </summary>
    string Symbol { get; }

    /// <summary>
    ///     The position of a Currency.
    /// </summary>
    int? Rank { get; }

    /// <summary>
    ///     The USD price.
    /// </summary>
    decimal? PriceUsd { get; }

    /// <summary>
    ///     The BTC price.
    /// </summary>
    decimal? PriceBtc { get; }

    /// <summary>
    ///     The ETH price.
    /// </summary>
    decimal? PriceEth { get; }

    /// <summary>
    ///     The 24H volume in USD.
    /// </summary>
    double? Volume { get; }

    /// <summary>
    ///     The USD market cap.
    /// </summary>
    double? MarketCap { get; }

    /// <summary>
    ///     The price change last hour as a percentage.
    /// </summary>
    double? HourChange { get; }

    /// <summary>
    ///     The price change last day as a percentage.
    /// </summary>
    double? DayChange { get; }

    /// <summary>
    ///     The price change last week as a percentage.
    /// </summary>
    double? WeekChange { get; }

    /// <summary>
    ///     The last time this coin info was updated.
    /// </summary>
    DateTime? LastUpdated { get; }
}