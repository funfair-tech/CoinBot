using System.Runtime.Serialization;

namespace CoinBot.CoinSources.CoinMarketCap
{
    [DataContract]
    public class CoinMarketCapCoin : ICoin
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "symbol")]
        public string Symbol { get; set; }

        [DataMember(Name = "rank")]
        public int Rank { get; set; }

        [DataMember(Name = "price_usd")]
        public string PriceUsd { get; set; }

        [DataMember(Name = "price_btc")]
        public string PriceBtc { get; set; }

        [DataMember(Name = "price_eth")]
        public string PriceEth { get; set; }

        [DataMember(Name = "24h_volume_usd")]
        public string Volume { get; set; }

        [DataMember(Name = "market_cap_usd")]
        public string MarketCap { get; set; }

        [DataMember(Name = "available_supply")]
        public string AvailableSupply { get; set; }

        [DataMember(Name = "total_supply")]
        public string TotalSupply { get; set; }

        [DataMember(Name = "percent_change_1h")]
        public string HourChange { get; set; }

        [DataMember(Name = "percent_change_24h")]
        public string DayChange { get; set; }

        [DataMember(Name = "percent_change_7d")]
        public string WeekChange { get; set; }

        [DataMember(Name = "last_updated")]
        public long? LastUpdated { get; set; }
    }
}
