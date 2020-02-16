using System;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace CoinBot.Clients.Bittrex
{
    [SuppressMessage(category: "Microsoft.Performance", checkId: "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Used as data packet")]
    internal sealed class BittrexMarketSummaryDto
    {
        [JsonProperty("MarketName")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string MarketName { get; set; } = default!;

        [JsonProperty("High")]
        public decimal? High { get; set; }

        [JsonProperty("Low")]
        public decimal? Low { get; set; }

        [JsonProperty("Volume")]
        public decimal? Volume { get; set; }

        [JsonProperty("Last")]
        public decimal? Last { get; set; }

        [JsonProperty("BaseVolume")]
        public decimal? BaseVolume { get; set; }

        [JsonProperty("TimeStamp")]
        public DateTime? TimeStamp { get; set; }

        [JsonProperty("Bid")]
        public decimal? Bid { get; set; }

        [JsonProperty("Ask")]
        public decimal? Ask { get; set; }
    }
}