using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace CoinBot.Clients.Bittrex;

[SuppressMessage(category: "Microsoft.Performance", checkId: "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Used as data packet")]
internal sealed class BittrexMarketSummaryDto
{
    [JsonPropertyName(name: @"MarketName")]
    [SuppressMessage(category: "ReSharper", checkId: "RedundantDefaultMemberInitializer", Justification = "TODO: Review")]
    public string MarketName { get; set; } = default!;

    [JsonPropertyName(name: @"High")]
    public decimal? High { get; set; }

    [JsonPropertyName(name: @"Low")]
    public decimal? Low { get; set; }

    [JsonPropertyName(name: @"Volume")]
    public decimal? Volume { get; set; }

    [JsonPropertyName(name: @"Last")]
    public decimal? Last { get; set; }

    [JsonPropertyName(name: @"BaseVolume")]
    public decimal? BaseVolume { get; set; }

    [JsonPropertyName(name: @"TimeStamp")]
    public DateTime? TimeStamp { get; set; }

    [JsonPropertyName(name: @"Bid")]
    public decimal? Bid { get; set; }

    [JsonPropertyName(name: @"Ask")]
    public decimal? Ask { get; set; }
}