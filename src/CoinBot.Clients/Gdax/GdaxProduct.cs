using Newtonsoft.Json;

namespace CoinBot.Clients.Gdax
{
    public sealed class GdaxProduct
    {
        [JsonProperty("id")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string Id { get; set; } = default!;

        [JsonProperty("base_currency")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string BaseCurrency { get; set; } = default!;

        [JsonProperty("quote_currency")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string QuoteCurrency { get; set; } = default!;

        [JsonProperty("base_min_size")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string BaseMinSize { get; set; } = default!;

        [JsonProperty("base_max_size")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string BaseMaxSize { get; set; } = default!;

        [JsonProperty("quote_increment")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string QuoteIncrement { get; set; } = default!;

        [JsonProperty("display_name")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string DisplayName { get; set; } = default!;

        [JsonProperty("status")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string Status { get; set; } = default!;

        [JsonProperty("margin_enabled")]
        public bool MarginEnabled { get; set; }

        [JsonProperty("status_message")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public object StatusMessage { get; set; } = default!;
    }
}