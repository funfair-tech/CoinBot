using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace CoinBot.Clients.Gdax
{
    [SuppressMessage(category: "Microsoft.Performance", checkId: "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Used as data packet")]
    internal sealed class GdaxTicker
    {
        // ReSharper disable once RedundantDefaultMemberInitializer
        public string ProductId { get; set; } = default!;

        [JsonPropertyName(name: @"ask")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string Ask { get; set; } = default!;

        [JsonPropertyName(name: @"bid")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string Bid { get; set; } = default!;

        [JsonPropertyName(name: @"price")]
        public decimal? Price { get; set; }

        [JsonPropertyName(name: @"size")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string Size { get; set; } = default!;

        [JsonPropertyName(name: @"time")]
        public DateTime? Time { get; set; }

        [JsonPropertyName(name: @"trade_id")]
        public long TradeId { get; set; }

        [JsonPropertyName(name: @"volume")]
        public decimal? Volume { get; set; }
    }
}