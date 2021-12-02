using System;
using System.Text.Json.Serialization;
using CoinBot.Core;
using CoinBot.Core.JsonConverters;

namespace CoinBot.Clients.CoinMarketCap;

/// <inheritdoc />
public sealed class CoinMarketCapGlobalInfo : IGlobalInfo
{
    /// <inheritdoc />
    [JsonPropertyName(name: @"total_market_cap_usd")]
    public double? MarketCap { get; set; }

    /// <inheritdoc />
    [JsonPropertyName(name: @"total_24h_volume_usd")]
    public double? Volume { get; set; }

    /// <inheritdoc />
    [JsonPropertyName(name: @"bitcoin_percentage_of_market_cap")]
    public double? BtcDominance { get; set; }

    /// <inheritdoc />
    [JsonPropertyName(name: @"active_currencies")]
    public int? Currencies { get; set; }

    /// <inheritdoc />
    [JsonPropertyName(name: @"active_assets")]
    public int? Assets { get; set; }

    /// <inheritdoc />
    [JsonPropertyName(name: @"active_markets")]
    public int? Markets { get; set; }

    /// <inheritdoc />
    [JsonPropertyName(name: @"last_updated")]
    [JsonConverter(typeof(UnixTimeConverter))]
    public DateTime? LastUpdated { get; set; }
}