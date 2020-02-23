using System;
using CoinBot.Core;
using Newtonsoft.Json;

namespace CoinBot.Clients.CoinMarketCap
{
    /// <inheritdoc />
    [JsonObject]
    public sealed class CoinMarketCapGlobalInfo : IGlobalInfo
    {
        /// <inheritdoc />
        [JsonProperty(propertyName: "total_market_cap_usd")]
        public double? MarketCap { get; set; }

        /// <inheritdoc />
        [JsonProperty(propertyName: "total_24h_volume_usd")]
        public double? Volume { get; set; }

        /// <inheritdoc />
        [JsonProperty(propertyName: "bitcoin_percentage_of_market_cap")]
        public double? BtcDominance { get; set; }

        /// <inheritdoc />
        [JsonProperty(propertyName: "active_currencies")]
        public int? Currencies { get; set; }

        /// <inheritdoc />
        [JsonProperty(propertyName: "active_assets")]
        public int? Assets { get; set; }

        /// <inheritdoc />
        [JsonProperty(propertyName: "active_markets")]
        public int? Markets { get; set; }

        /// <inheritdoc />
        [JsonProperty(propertyName: "last_updated")]
        [JsonConverter(typeof(UnixTimeConverter))]
        public DateTime? LastUpdated { get; set; }
    }
}