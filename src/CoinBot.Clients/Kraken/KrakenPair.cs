using Newtonsoft.Json;

namespace CoinBot.Clients.Kraken
{
    public sealed class KrakenPair
    {
        // ReSharper disable once RedundantDefaultMemberInitializer
        public string PairId { get; set; } = default!;

        [JsonProperty("base")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string BaseCurrency { get; set; } = default!;

        [JsonProperty("quote")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string QuoteCurrency { get; set; } = default!;
    }
}