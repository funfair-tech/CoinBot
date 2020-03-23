using System.Text.Json.Serialization;

namespace CoinBot.Clients.Gdax
{
    public sealed class GdaxProduct
    {
        [JsonPropertyName(name: @"id")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string Id { get; set; } = default!;

        [JsonPropertyName(name: @"base_currency")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string BaseCurrency { get; set; } = default!;

        [JsonPropertyName(name: @"quote_currency")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string QuoteCurrency { get; set; } = default!;

        [JsonPropertyName(name: @"base_min_size")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string BaseMinSize { get; set; } = default!;

        [JsonPropertyName(name: @"base_max_size")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string BaseMaxSize { get; set; } = default!;

        [JsonPropertyName(name: @"quote_increment")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string QuoteIncrement { get; set; } = default!;

        [JsonPropertyName(name: @"display_name")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string DisplayName { get; set; } = default!;

        [JsonPropertyName(name: @"status")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string Status { get; set; } = default!;

        [JsonPropertyName(name: @"margin_enabled")]
        public bool MarginEnabled { get; set; }

        [JsonPropertyName(name: @"status_message")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public object StatusMessage { get; set; } = default!;
    }
}