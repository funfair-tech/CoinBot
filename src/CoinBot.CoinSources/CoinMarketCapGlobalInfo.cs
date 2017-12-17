using System.Runtime.Serialization;

namespace CoinBot.CoinSources
{
    [DataContract]
    public class CoinMarketCapGlobalInfo : IGlobalInfo
    {
        [DataMember(Name = "total_market_cap_usd")]
        public string MarketCap { get; set; }

        [DataMember(Name = "total_24h_volume_usd")]
        public string Volume { get; set; }

        [DataMember(Name = "bitcoin_percentage_of_market_cap")]
        public string BTCDominance { get; set; }

        [DataMember(Name = "active_currencies")]
        public string Currencies { get; set; }

        [DataMember(Name = "active_assets")]
        public string Assets { get; set; }

        [DataMember(Name = "active_markets")]
        public string Markets { get; set; }

        [DataMember(Name = "last_updated")]
        public long? LastUpdated { get; set; }
    }
}
