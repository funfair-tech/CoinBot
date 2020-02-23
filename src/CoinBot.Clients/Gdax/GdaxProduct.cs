using Newtonsoft.Json;

namespace CoinBot.Clients.Gdax
{
    public sealed class GdaxProduct
    {
        [JsonProperty(propertyName: "id")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string Id { get; set; } = default!;

        [JsonProperty(propertyName: "base_currency")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string BaseCurrency { get; set; } = default!;

        [JsonProperty(propertyName: "quote_currency")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string QuoteCurrency { get; set; } = default!;

        [JsonProperty(propertyName: "base_min_size")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string BaseMinSize { get; set; } = default!;

        [JsonProperty(propertyName: "base_max_size")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string BaseMaxSize { get; set; } = default!;

        [JsonProperty(propertyName: "quote_increment")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string QuoteIncrement { get; set; } = default!;

        [JsonProperty(propertyName: "display_name")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string DisplayName { get; set; } = default!;

        [JsonProperty(propertyName: "status")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string Status { get; set; } = default!;

        [JsonProperty(propertyName: "margin_enabled")]
        public bool MarginEnabled { get; set; }

        [JsonProperty(propertyName: "status_message")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public object StatusMessage { get; set; } = default!;
    }
}