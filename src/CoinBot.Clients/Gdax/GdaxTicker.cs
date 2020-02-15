using System;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace CoinBot.Clients.Gdax
{
    [SuppressMessage(category: "Microsoft.Performance", checkId: "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Used as data packet")]
    internal sealed class GdaxTicker
    {
        // ReSharper disable once RedundantDefaultMemberInitializer
        public string ProductId { get; set; } = default!;

        [JsonProperty("ask")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string Ask { get; set; } = default!;

        [JsonProperty("bid")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string Bid { get; set; } = default!;

        [JsonProperty("price")]
        public decimal? Price { get; set; }

        [JsonProperty("size")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string Size { get; set; } = default!;

        [JsonProperty("time")]
        public DateTime? Time { get; set; }

        [JsonProperty("trade_id")]
        public long TradeId { get; set; }

        [JsonProperty("volume")]
        public decimal? Volume { get; set; }
    }
}