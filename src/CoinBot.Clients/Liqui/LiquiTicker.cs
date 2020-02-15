using System;
using Newtonsoft.Json;

namespace CoinBot.Clients.Liqui
{
    public class LiquiTicker
    {
        // ReSharper disable once RedundantDefaultMemberInitializer
        public string Pair { get; set; } = default!;

        [JsonProperty("high")]
        public decimal? High { get; set; }

        [JsonProperty("low")]
        public decimal? Low { get; set; }

        [JsonProperty("avg")]
        public decimal? Avg { get; set; }

        [JsonProperty("vol")]
        public decimal? Vol { get; set; }

        [JsonProperty("vol_cur")]
        public decimal? VolCur { get; set; }

        [JsonProperty("last")]
        public decimal? Last { get; set; }

        [JsonProperty("buy")]
        public decimal? Buy { get; set; }

        [JsonProperty("sell")]
        public decimal? Sell { get; set; }

        [JsonProperty("updated")]
        [JsonConverter(typeof(UnixTimeConverter))]
        public DateTime? Updated { get; set; }
    }
}