using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using CoinBot.Core;
using CoinBot.Core.Helpers;
using CoinBot.Core.JsonConverters;

namespace CoinBot.Clients.CoinMarketCap;

public sealed class CoinMarketCapCoin : ICoinInfo
{
    [JsonConstructor]
    public CoinMarketCapCoin(decimal? availableSupply,
                             decimal? totalSupply,
                             decimal? maxSupply,
                             string id,
                             string name,
                             string symbol,
                             int? rank,
                             decimal? priceUsd,
                             decimal? priceBtc,
                             decimal? priceEth,
                             double? volume,
                             double? marketCap,
                             double? hourChange,
                             double? dayChange,
                             double? weekChange,
                             DateTime? lastUpdated)
    {
        this.AvailableSupply = availableSupply;
        this.TotalSupply = totalSupply;
        this.MaxSupply = maxSupply;
        this.Id = id;
        this.Name = name;
        this.Symbol = symbol;
        this.Rank = rank;
        this.PriceUsd = priceUsd;
        this.PriceBtc = priceBtc;
        this.PriceEth = priceEth;
        this.Volume = volume;
        this.MarketCap = marketCap;
        this.HourChange = hourChange;
        this.DayChange = dayChange;
        this.WeekChange = weekChange;
        this.LastUpdated = lastUpdated;
    }

    [SuppressMessage(category: "Microsoft.Design", checkId: "CA1056:UriPropertiesShouldNotBeStrings", Justification = "Model for serialization")]
    public string Url => $"https://coinmarketcap.com/currencies/{this.Id}";

    [JsonPropertyName(name: @"available_supply")]
    [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Global", Justification = "For expansion")]
    public decimal? AvailableSupply { get; }

    [JsonPropertyName(name: @"total_supply")]
    [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Global", Justification = "For expansion")]
    public decimal? TotalSupply { get; }

    [JsonPropertyName(name: @"max_supply")]
    [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Global", Justification = "For expansion")]
    public decimal? MaxSupply { get; }

    [JsonPropertyName(name: @"id")]
    public string Id { get; }

    [SuppressMessage(category: "Microsoft.Design", checkId: "CA1056:UriPropertiesShouldNotBeStrings", Justification = "Model for serialization")]
    public string ImageUrl =>
        Images.CurrencyImageUrl(this.Symbol)
              .ToString();

    [JsonPropertyName(name: @"name")]
    public string Name { get; }

    [JsonPropertyName(name: @"symbol")]
    public string Symbol { get; }

    [JsonPropertyName(name: @"rank")]
    public int? Rank { get; }

    [JsonPropertyName(name: @"price_usd")]
    public decimal? PriceUsd { get; }

    [JsonPropertyName(name: @"price_btc")]
    public decimal? PriceBtc { get; }

    [JsonPropertyName(name: @"price_eth")]
    public decimal? PriceEth { get; }

    [JsonPropertyName(name: @"24h_volume_usd")]
    public double? Volume { get; }

    [JsonPropertyName(name: @"market_cap_usd")]
    public double? MarketCap { get; }

    [JsonPropertyName(name: @"percent_change_1h")]
    public double? HourChange { get; }

    [JsonPropertyName(name: @"percent_change_24h")]
    public double? DayChange { get; }

    [JsonPropertyName(name: @"percent_change_7d")]
    public double? WeekChange { get; }

    [JsonPropertyName(name: @"last_updated")]
    [JsonConverter(typeof(UnixTimeConverter))]
    public DateTime? LastUpdated { get; }
}