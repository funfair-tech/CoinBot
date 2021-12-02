using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace CoinBot.Clients.Gdax;

public sealed class GdaxProduct
{
    [JsonPropertyName(name: @"id")]
    [SuppressMessage(category: "ReSharper", checkId: "RedundantDefaultMemberInitializer", Justification = "TODO: Review")]
    public string Id { get; set; } = default!;

    [JsonPropertyName(name: @"base_currency")]
    [SuppressMessage(category: "ReSharper", checkId: "RedundantDefaultMemberInitializer", Justification = "TODO: Review")]
    public string BaseCurrency { get; set; } = default!;

    [JsonPropertyName(name: @"quote_currency")]
    [SuppressMessage(category: "ReSharper", checkId: "RedundantDefaultMemberInitializer", Justification = "TODO: Review")]
    public string QuoteCurrency { get; set; } = default!;

    [JsonPropertyName(name: @"base_min_size")]
    [SuppressMessage(category: "ReSharper", checkId: "RedundantDefaultMemberInitializer", Justification = "TODO: Review")]
    public string BaseMinSize { get; set; } = default!;

    [JsonPropertyName(name: @"base_max_size")]
    [SuppressMessage(category: "ReSharper", checkId: "RedundantDefaultMemberInitializer", Justification = "TODO: Review")]
    public string BaseMaxSize { get; set; } = default!;

    [JsonPropertyName(name: @"quote_increment")]
    [SuppressMessage(category: "ReSharper", checkId: "RedundantDefaultMemberInitializer", Justification = "TODO: Review")]
    public string QuoteIncrement { get; set; } = default!;

    [JsonPropertyName(name: @"display_name")]
    [SuppressMessage(category: "ReSharper", checkId: "RedundantDefaultMemberInitializer", Justification = "TODO: Review")]
    public string DisplayName { get; set; } = default!;

    [JsonPropertyName(name: @"status")]
    [SuppressMessage(category: "ReSharper", checkId: "RedundantDefaultMemberInitializer", Justification = "TODO: Review")]
    public string Status { get; set; } = default!;

    [JsonPropertyName(name: @"margin_enabled")]
    public bool MarginEnabled { get; set; }

    [JsonPropertyName(name: @"status_message")]
    [SuppressMessage(category: "ReSharper", checkId: "RedundantDefaultMemberInitializer", Justification = "TODO: Review")]
    public object StatusMessage { get; set; } = default!;
}