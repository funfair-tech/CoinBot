namespace CoinBot.Core;

public sealed class MarketManagerSettings
{
    /// <summary>
    ///     The exchange refresh interval in minutes.
    /// </summary>
    public int RefreshInterval { get; set; } = 2;
}