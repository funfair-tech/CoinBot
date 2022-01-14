using System;
using System.Diagnostics.CodeAnalysis;
using CoinBot.Core;
using CoinBot.Core.Helpers;

namespace CoinBot.Clients.FunFair;

public sealed class FunFairWalletCoin : ICoinInfo
{
    public FunFairWalletCoin(string symbol, in DateTime lastUpdated, decimal priceUsd)
    {
        this.Id = symbol;
        this.Symbol = symbol;
        this.PriceUsd = priceUsd;
        this.Name = symbol == @"FUN"
            ? "FunFair"
            : symbol;
        this.LastUpdated = lastUpdated;
    }

    public string Id { get; }

    [SuppressMessage(category: "Microsoft.Design", checkId: "CA1056:UriPropertiesShouldNotBeStrings", Justification = "Model for serialization")]
    public string ImageUrl =>
        Images.CurrencyImageUrl(this.Symbol)
              .ToString();

    public string Name { get; }

    public string Symbol { get; }

    public int? Rank { get; }

    public decimal? PriceUsd { get; }

    public decimal? PriceBtc { get; }

    public decimal? PriceEth { get; }

    public double? Volume { get; }

    public double? MarketCap { get; }

    public double? HourChange { get; }

    public double? DayChange { get; }

    public double? WeekChange { get; }

    public DateTime? LastUpdated { get; }
}