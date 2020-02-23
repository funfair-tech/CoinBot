using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace CoinBot.Clients.Poloniex
{
    [SuppressMessage(category: "Microsoft.Performance", checkId: "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Used as data packet")]
    internal sealed class PoloniexTicker
    {
        // ReSharper disable once RedundantDefaultMemberInitializer
        public string Pair { get; set; } = default!;

        [JsonProperty(propertyName: "id")]
        public long Id { get; set; }

        [JsonProperty(propertyName: "last")]
        public decimal? Last { get; set; }

        [JsonProperty(propertyName: "lowestAsk")]
        public decimal? LowestAsk { get; set; }

        [JsonProperty(propertyName: "highestBid")]
        public decimal? HighestBid { get; set; }

        [JsonProperty(propertyName: "percentChange")]
        public double PercentChange { get; set; }

        [JsonProperty(propertyName: "baseVolume")]
        public decimal? BaseVolume { get; set; }

        [JsonProperty(propertyName: "quoteVolume")]
        public decimal? QuoteVolume { get; set; }

        [JsonProperty(propertyName: "isFrozen")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string IsFrozen { get; set; } = default!;

        [JsonProperty(propertyName: "high24hr")]
        public decimal? High24Hr { get; set; }

        [JsonProperty(propertyName: "low24hr")]
        public decimal? Low24Hr { get; set; }
    }
}