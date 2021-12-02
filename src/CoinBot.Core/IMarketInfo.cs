using System;

namespace CoinBot.Core;

public interface IMarketInfo
{
    /// <summary>
    ///     The market name.
    /// </summary>
    string Name { get; }

    /// <summary>
    ///     The 24H volume.
    /// </summary>
    decimal? Volume { get; }

    decimal? Bid { get; }

    decimal? Ask { get; }

    decimal? Last { get; }

    DateTime? LastUpdated { get; }
}