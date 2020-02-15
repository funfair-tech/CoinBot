﻿using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace CoinBot.Clients.Poloniex
{
    [SuppressMessage(category: "Microsoft.Performance", checkId: "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Used as data packet")]
    internal sealed class PoloniexTicker
    {
        // ReSharper disable once RedundantDefaultMemberInitializer
        public string Pair { get; set; } = default!;

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("last")]
        public decimal? Last { get; set; }

        [JsonProperty("lowestAsk")]
        public decimal? LowestAsk { get; set; }

        [JsonProperty("highestBid")]
        public decimal? HighestBid { get; set; }

        [JsonProperty("percentChange")]
        public double PercentChange { get; set; }

        [JsonProperty("baseVolume")]
        public decimal? BaseVolume { get; set; }

        [JsonProperty("quoteVolume")]
        public decimal? QuoteVolume { get; set; }

        [JsonProperty("isFrozen")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string IsFrozen { get; set; } = default!;

        [JsonProperty("high24hr")]
        public decimal? High24Hr { get; set; }

        [JsonProperty("low24hr")]
        public decimal? Low24Hr { get; set; }
    }
}