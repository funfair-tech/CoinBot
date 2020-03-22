using System.Text.Json.Serialization;

namespace CoinBot.Clients.GateIo
{
    public sealed class GateIoTicker
    {
        // ReSharper disable once RedundantDefaultMemberInitializer
        public string Pair { get; set; } = default!;

        [JsonPropertyName(name: @"result")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string Result { get; set; } = default!;

        [JsonPropertyName(name: @"last")]
        public decimal? Last { get; set; }

        [JsonPropertyName(name: @"lowestAsk")]
        public decimal? LowestAsk { get; set; }

        [JsonPropertyName(name: @"highestBid")]
        public decimal? HighestBid { get; set; }

        [JsonPropertyName(name: @"percentChange")]
        public decimal? PercentChange { get; set; }

        [JsonPropertyName(name: @"baseVolume")]
        public decimal? BaseVolume { get; set; }

        [JsonPropertyName(name: @"quoteVolume")]
        public decimal? QuoteVolume { get; set; }

        [JsonPropertyName(name: @"high24hr")]
        public decimal? High24Hr { get; set; }

        [JsonPropertyName(name: @"low24hr")]
        public decimal? Low24Hr { get; set; }
    }
}