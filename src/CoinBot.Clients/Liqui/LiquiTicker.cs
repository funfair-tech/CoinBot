using System;
using Newtonsoft.Json;

namespace CoinBot.Clients.Liqui
{
    public sealed class LiquiTicker
    {
        // ReSharper disable once RedundantDefaultMemberInitializer
        public string Pair { get; set; } = default!;

        [JsonProperty(propertyName: "high")]
        public decimal? High { get; set; }

        [JsonProperty(propertyName: "low")]
        public decimal? Low { get; set; }

        [JsonProperty(propertyName: "avg")]
        public decimal? Avg { get; set; }

        [JsonProperty(propertyName: "vol")]
        public decimal? Vol { get; set; }

        [JsonProperty(propertyName: "vol_cur")]
        public decimal? VolCur { get; set; }

        [JsonProperty(propertyName: "last")]
        public decimal? Last { get; set; }

        [JsonProperty(propertyName: "buy")]
        public decimal? Buy { get; set; }

        [JsonProperty(propertyName: "sell")]
        public decimal? Sell { get; set; }

        [JsonProperty(propertyName: "updated")]
        [JsonConverter(typeof(UnixTimeConverter))]
        public DateTime? Updated { get; set; }
    }
}