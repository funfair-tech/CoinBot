using System;
using System.Diagnostics.CodeAnalysis;
using CoinBot.Core;
using Newtonsoft.Json;

namespace CoinBot.Clients.CoinMarketCap
{
    [JsonObject]
    public sealed class CoinMarketCapCoin : ICoinInfo
    {
        [SuppressMessage(category: "Microsoft.Design", checkId: "CA1056:UriPropertiesShouldNotBeStrings", Justification = "Model for serialization")]
        public string Url => $"https://coinmarketcap.com/currencies/{this.Id}";

        [JsonProperty(propertyName: "available_supply")]
        public decimal? AvailableSupply { get; set; }

        [JsonProperty(propertyName: "total_supply")]
        public decimal? TotalSupply { get; set; }

        [JsonProperty(propertyName: "max_supply")]
        public decimal? MaxSupply { get; set; }

        [JsonProperty(propertyName: "id")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string Id { get; set; } = default!;

        [SuppressMessage(category: "Microsoft.Design", checkId: "CA1056:UriPropertiesShouldNotBeStrings", Justification = "Model for serialization")]
        public string ImageUrl => $"https://raw.githubusercontent.com/cjdowner/cryptocurrency-icons/master/128/color/{this.Symbol.ToLowerInvariant()}.png";

        [JsonProperty(propertyName: "name")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string Name { get; set; } = default!;

        [JsonProperty(propertyName: "symbol")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string Symbol { get; set; } = default!;

        [JsonProperty(propertyName: "rank")]
        public int? Rank { get; set; }

        [JsonProperty(propertyName: "price_usd")]
        public double? PriceUsd { get; set; }

        [JsonProperty(propertyName: "price_btc")]
        public decimal? PriceBtc { get; set; }

        [JsonProperty(propertyName: "price_eth")]
        public decimal? PriceEth { get; set; }

        [JsonProperty(propertyName: "24h_volume_usd")]
        public double? Volume { get; set; }

        [JsonProperty(propertyName: "market_cap_usd")]
        public double? MarketCap { get; set; }

        [JsonProperty(propertyName: "percent_change_1h")]
        public double? HourChange { get; set; }

        [JsonProperty(propertyName: "percent_change_24h")]
        public double? DayChange { get; set; }

        [JsonProperty(propertyName: "percent_change_7d")]
        public double? WeekChange { get; set; }

        [JsonProperty(propertyName: "last_updated")]
        [JsonConverter(typeof(UnixTimeConverter))]
        public DateTime? LastUpdated { get; set; }
    }
}