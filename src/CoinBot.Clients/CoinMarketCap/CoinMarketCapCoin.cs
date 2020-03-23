﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using CoinBot.Core;
using CoinBot.Core.JsonConverters;

namespace CoinBot.Clients.CoinMarketCap
{
    public sealed class CoinMarketCapCoin : ICoinInfo
    {
        [SuppressMessage(category: "Microsoft.Design", checkId: "CA1056:UriPropertiesShouldNotBeStrings", Justification = "Model for serialization")]
        public string Url => $"https://coinmarketcap.com/currencies/{this.Id}";

        [JsonPropertyName(name: @"available_supply")]
        public decimal? AvailableSupply { get; set; }

        [JsonPropertyName(name: @"total_supply")]
        public decimal? TotalSupply { get; set; }

        [JsonPropertyName(name: @"max_supply")]
        public decimal? MaxSupply { get; set; }

        [JsonPropertyName(name: @"id")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string Id { get; set; } = default!;

        [SuppressMessage(category: "Microsoft.Design", checkId: "CA1056:UriPropertiesShouldNotBeStrings", Justification = "Model for serialization")]
        public string ImageUrl => $"https://raw.githubusercontent.com/cjdowner/cryptocurrency-icons/master/128/color/{this.Symbol.ToLowerInvariant()}.png";

        [JsonPropertyName(name: @"name")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string Name { get; set; } = default!;

        [JsonPropertyName(name: @"symbol")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string Symbol { get; set; } = default!;

        [JsonPropertyName(name: @"rank")]
        public int? Rank { get; set; }

        [JsonPropertyName(name: @"price_usd")]
        public double? PriceUsd { get; set; }

        [JsonPropertyName(name: @"price_btc")]
        public decimal? PriceBtc { get; set; }

        [JsonPropertyName(name: @"price_eth")]
        public decimal? PriceEth { get; set; }

        [JsonPropertyName(name: @"24h_volume_usd")]
        public double? Volume { get; set; }

        [JsonPropertyName(name: @"market_cap_usd")]
        public double? MarketCap { get; set; }

        [JsonPropertyName(name: @"percent_change_1h")]
        public double? HourChange { get; set; }

        [JsonPropertyName(name: @"percent_change_24h")]
        public double? DayChange { get; set; }

        [JsonPropertyName(name: @"percent_change_7d")]
        public double? WeekChange { get; set; }

        [JsonPropertyName(name: @"last_updated")]
        [JsonConverter(typeof(UnixTimeConverter))]
        public DateTime? LastUpdated { get; set; }
    }
}