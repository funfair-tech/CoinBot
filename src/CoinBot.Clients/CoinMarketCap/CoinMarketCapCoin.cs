using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using CoinBot.Core;
using CoinBot.Core.Helpers;
using CoinBot.Core.JsonConverters;

namespace CoinBot.Clients.CoinMarketCap;

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
    [SuppressMessage(category: "ReSharper", checkId: "RedundantDefaultMemberInitializer", Justification = "TODO: Review")]
    public string Id { get; set; } = default!;

    [SuppressMessage(category: "Microsoft.Design", checkId: "CA1056:UriPropertiesShouldNotBeStrings", Justification = "Model for serialization")]
    public string ImageUrl =>
        Images.CurrencyImageUrl(this.Symbol)
              .ToString();

    [JsonPropertyName(name: @"name")]
    [SuppressMessage(category: "ReSharper", checkId: "RedundantDefaultMemberInitializer", Justification = "TODO: Review")]
    public string Name { get; set; } = default!;

    [JsonPropertyName(name: @"symbol")]
    [SuppressMessage(category: "ReSharper", checkId: "RedundantDefaultMemberInitializer", Justification = "TODO: Review")]
    public string Symbol { get; set; } = default!;

    [JsonPropertyName(name: @"rank")]
    public int? Rank { get; set; }

    [JsonPropertyName(name: @"price_usd")]
    public decimal? PriceUsd { get; set; }

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