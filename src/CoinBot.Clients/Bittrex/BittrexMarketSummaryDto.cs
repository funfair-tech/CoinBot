using System;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace CoinBot.Clients.Bittrex
{
    [SuppressMessage(category: "Microsoft.Performance", checkId: "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Used as data packet")]
    internal sealed class BittrexMarketSummaryDto
    {
        [JsonProperty(propertyName: "MarketName")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string MarketName { get; set; } = default!;

        [JsonProperty(propertyName: "High")]
        public decimal? High { get; set; }

        [JsonProperty(propertyName: "Low")]
        public decimal? Low { get; set; }

        [JsonProperty(propertyName: "Volume")]
        public decimal? Volume { get; set; }

        [JsonProperty(propertyName: "Last")]
        public decimal? Last { get; set; }

        [JsonProperty(propertyName: "BaseVolume")]
        public decimal? BaseVolume { get; set; }

        [JsonProperty(propertyName: "TimeStamp")]
        public DateTime? TimeStamp { get; set; }

        [JsonProperty(propertyName: "Bid")]
        public decimal? Bid { get; set; }

        [JsonProperty(propertyName: "Ask")]
        public decimal? Ask { get; set; }
    }
}