using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace CoinBot.Clients.Gdax;

public sealed class GdaxProduct
{
    [JsonConstructor]
    public GdaxProduct(string id,
                       string baseCurrency,
                       string quoteCurrency,
                       string baseMinSize,
                       string baseMaxSize,
                       string quoteIncrement,
                       string displayName,
                       string status,
                       bool marginEnabled,
                       object statusMessage)
    {
        this.Id = id;
        this.BaseCurrency = baseCurrency;
        this.QuoteCurrency = quoteCurrency;
        this.BaseMinSize = baseMinSize;
        this.BaseMaxSize = baseMaxSize;
        this.QuoteIncrement = quoteIncrement;
        this.DisplayName = displayName;
        this.Status = status;
        this.MarginEnabled = marginEnabled;
        this.StatusMessage = statusMessage;
    }

    [JsonPropertyName(name: @"id")]
    public string Id { get; }

    [JsonPropertyName(name: @"base_currency")]
    [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Global", Justification = "For expansion")]
    public string BaseCurrency { get; }

    [JsonPropertyName(name: @"quote_currency")]
    [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Global", Justification = "For expansion")]
    public string QuoteCurrency { get; }

    [JsonPropertyName(name: @"base_min_size")]
    [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Global", Justification = "For expansion")]
    public string BaseMinSize { get; }

    [JsonPropertyName(name: @"base_max_size")]
    [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Global", Justification = "For expansion")]
    public string BaseMaxSize { get; }

    [JsonPropertyName(name: @"quote_increment")]
    [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Global", Justification = "For expansion")]
    public string QuoteIncrement { get; }

    [JsonPropertyName(name: @"display_name")]
    [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Global", Justification = "For expansion")]
    public string DisplayName { get; }

    [JsonPropertyName(name: @"status")]
    [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Global", Justification = "For expansion")]
    public string Status { get; }

    [JsonPropertyName(name: @"margin_enabled")]
    [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Global", Justification = "For expansion")]
    public bool MarginEnabled { get; }

    [JsonPropertyName(name: @"status_message")]
    [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Global", Justification = "For expansion")]
    public object StatusMessage { get; }
}