using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace CoinBot.Clients.Gdax;

[SuppressMessage(category: "Microsoft.Performance", checkId: "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Used as data packet")]
internal sealed class GdaxTicker
{
    [SuppressMessage(category: "ReSharper", checkId: "RedundantDefaultMemberInitializer", Justification = "TODO: Review")]
    public string ProductId { get; set; } = default!;

    [JsonPropertyName(name: @"ask")]
    [SuppressMessage(category: "ReSharper", checkId: "RedundantDefaultMemberInitializer", Justification = "TODO: Review")]
    public string Ask { get; set; } = default!;

    [JsonPropertyName(name: @"bid")]
    [SuppressMessage(category: "ReSharper", checkId: "RedundantDefaultMemberInitializer", Justification = "TODO: Review")]
    public string Bid { get; set; } = default!;

    [JsonPropertyName(name: @"price")]
    public decimal? Price { get; set; }

    [JsonPropertyName(name: @"size")]
    [SuppressMessage(category: "ReSharper", checkId: "RedundantDefaultMemberInitializer", Justification = "TODO: Review")]
    public string Size { get; set; } = default!;

    [JsonPropertyName(name: @"time")]
    public DateTime? Time { get; set; }

    [JsonPropertyName(name: @"trade_id")]
    public long TradeId { get; set; }

    [JsonPropertyName(name: @"volume")]
    public decimal? Volume { get; set; }
}