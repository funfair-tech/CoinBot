using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace CoinBot.Clients.Poloniex;

[SuppressMessage(category: "Microsoft.Performance", checkId: "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Used as data packet")]
internal sealed class PoloniexTicker
{
    [SuppressMessage(category: "ReSharper", checkId: "RedundantDefaultMemberInitializer", Justification = "TODO: Review")]
    public string Pair { get; set; } = default!;

    [JsonPropertyName(name: @"id")]
    public long Id { get; set; }

    [JsonPropertyName(name: @"last")]
    public decimal? Last { get; set; }

    [JsonPropertyName(name: @"lowestAsk")]
    public decimal? LowestAsk { get; set; }

    [JsonPropertyName(name: @"highestBid")]
    public decimal? HighestBid { get; set; }

    [JsonPropertyName(name: @"percentChange")]
    public decimal PercentChange { get; set; }

    [JsonPropertyName(name: @"baseVolume")]
    public decimal? BaseVolume { get; set; }

    [JsonPropertyName(name: @"quoteVolume")]
    public decimal? QuoteVolume { get; set; }

    [JsonPropertyName(name: @"isFrozen")]
    [SuppressMessage(category: "ReSharper", checkId: "RedundantDefaultMemberInitializer", Justification = "TODO: Review")]
    public string IsFrozen { get; set; } = default!;

    [JsonPropertyName(name: @"high24hr")]
    public decimal? High24Hr { get; set; }

    [JsonPropertyName(name: @"low24hr")]
    public decimal? Low24Hr { get; set; }
}