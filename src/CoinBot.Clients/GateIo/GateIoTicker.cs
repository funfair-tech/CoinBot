using Newtonsoft.Json;

namespace CoinBot.Clients.GateIo
{
    public sealed class GateIoTicker
    {
        // ReSharper disable once RedundantDefaultMemberInitializer
        public string Pair { get; set; } = default!;

        [JsonProperty(propertyName: "result")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string Result { get; set; } = default!;

        [JsonProperty(propertyName: "last")]
        public decimal? Last { get; set; }

        [JsonProperty(propertyName: "lowestAsk")]
        public decimal? LowestAsk { get; set; }

        [JsonProperty(propertyName: "highestBid")]
        public decimal? HighestBid { get; set; }

        [JsonProperty(propertyName: "percentChange")]
        public decimal? PercentChange { get; set; }

        [JsonProperty(propertyName: "baseVolume")]
        public decimal? BaseVolume { get; set; }

        [JsonProperty(propertyName: "quoteVolume")]
        public decimal? QuoteVolume { get; set; }

        [JsonProperty(propertyName: "high24hr")]
        public decimal? High24Hr { get; set; }

        [JsonProperty(propertyName: "low24hr")]
        public decimal? Low24Hr { get; set; }
    }
}