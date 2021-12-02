using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace CoinBot.Clients.Kraken;

public sealed class KrakenTicker
{
    [SuppressMessage(category: "ReSharper", checkId: "RedundantDefaultMemberInitializer", Justification = "TODO: Review")]
    public string BaseCurrency { get; set; } = default!;

    [SuppressMessage(category: "ReSharper", checkId: "RedundantDefaultMemberInitializer", Justification = "TODO: Review")]
    public string QuoteCurrency { get; set; } = default!;

    /// <summary>
    ///     last trade closed array(price, lot volume)
    /// </summary>
    [JsonPropertyName(name: @"c")]
    [SuppressMessage(category: "ReSharper", checkId: "RedundantDefaultMemberInitializer", Justification = "TODO: Review")]
    [SuppressMessage(category: "ReSharper", checkId: "AutoPropertyCanBeMadeGetOnly.Global", Justification = "TODO: Review")]
    public decimal[]? Last { get; set; } = default!;

    /// <summary>
    ///     24 Hour volume array(today, last 24 hours)
    /// </summary>
    [JsonPropertyName(name: @"v")]
    [SuppressMessage(category: "ReSharper", checkId: "RedundantDefaultMemberInitializer", Justification = "TODO: Review")]
    [SuppressMessage(category: "ReSharper", checkId: "AutoPropertyCanBeMadeGetOnly.Global", Justification = "TODO: Review")]
    public decimal[]? Volume { get; set; } = default!;
}