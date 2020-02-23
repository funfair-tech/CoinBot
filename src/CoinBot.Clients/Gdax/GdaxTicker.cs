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

        [JsonProperty(propertyName: "ask")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string Ask { get; set; } = default!;

        [JsonProperty(propertyName: "bid")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string Bid { get; set; } = default!;

        [JsonProperty(propertyName: "price")]
        public decimal? Price { get; set; }

        [JsonProperty(propertyName: "size")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string Size { get; set; } = default!;

        [JsonProperty(propertyName: "time")]
        public DateTime? Time { get; set; }

        [JsonProperty(propertyName: "trade_id")]
        public long TradeId { get; set; }

        [JsonProperty(propertyName: "volume")]
        public decimal? Volume { get; set; }
    }
}