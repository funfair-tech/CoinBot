using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace CoinBot.Clients.Binance;

[SuppressMessage(category: "Microsoft.Performance", checkId: "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Used as data packet")]
internal sealed class BinanceProduct
{
    public BinanceProduct(string symbol,
                          string quoteAssetName,
                          double tradedMoney,
                          string baseAssetUnit,
                          string baseAssetName,
                          string baseAsset,
                          string tickSize,
                          decimal? prevClose,
                          decimal activeBuy,
                          string high,
                          long lastAggTradeId,
                          string low,
                          string matchingUnitType,
                          string close,
                          string quoteAsset,
                          bool active,
                          decimal minTrade,
                          double activeSell,
                          string withdrawFee,
                          decimal volume,
                          long decimalPlaces,
                          string quoteAssetUnit,
                          string open,
                          string status,
                          double minQty)
    {
        this.Symbol = symbol;
        this.QuoteAssetName = quoteAssetName;
        this.TradedMoney = tradedMoney;
        this.BaseAssetUnit = baseAssetUnit;
        this.BaseAssetName = baseAssetName;
        this.BaseAsset = baseAsset;
        this.TickSize = tickSize;
        this.PrevClose = prevClose;
        this.ActiveBuy = activeBuy;
        this.High = high;
        this.LastAggTradeId = lastAggTradeId;
        this.Low = low;
        this.MatchingUnitType = matchingUnitType;
        this.Close = close;
        this.QuoteAsset = quoteAsset;
        this.Active = active;
        this.MinTrade = minTrade;
        this.ActiveSell = activeSell;
        this.WithdrawFee = withdrawFee;
        this.Volume = volume;
        this.DecimalPlaces = decimalPlaces;
        this.QuoteAssetUnit = quoteAssetUnit;
        this.Open = open;
        this.Status = status;
        this.MinQty = minQty;
    }

    [JsonPropertyName(name: @"symbol")]
    [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Global", Justification = "For expansion")]
    public string Symbol { get; }

    [JsonPropertyName(name: @"quoteAssetName")]
    [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Global", Justification = "For expansion")]
    public string QuoteAssetName { get; }

    [JsonPropertyName(name: @"tradedMoney")]
    [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Global", Justification = "For expansion")]
    public double TradedMoney { get; }

    [JsonPropertyName(name: @"baseAssetUnit")]
    [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Global", Justification = "For expansion")]
    public string BaseAssetUnit { get; }

    [JsonPropertyName(name: @"baseAssetName")]
    public string BaseAssetName { get; }

    [JsonPropertyName(name: @"baseAsset")]
    public string BaseAsset { get; }

    [JsonPropertyName(name: @"tickSize")]
    [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Global", Justification = "For expansion")]
    public string TickSize { get; }

    [JsonPropertyName(name: @"prevClose")]
    public decimal? PrevClose { get; }

    [JsonPropertyName(name: @"activeBuy")]
    [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Global", Justification = "For expansion")]
    public decimal ActiveBuy { get; }

    [JsonPropertyName(name: @"high")]
    [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Global", Justification = "For expansion")]
    public string High { get; }

    [JsonPropertyName(name: @"lastAggTradeId")]
    [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Global", Justification = "For expansion")]
    public long LastAggTradeId { get; }

    [JsonPropertyName(name: @"low")]
    [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Global", Justification = "For expansion")]
    public string Low { get; }

    [JsonPropertyName(name: @"matchingUnitType")]
    [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Global", Justification = "For expansion")]
    public string MatchingUnitType { get; }

    [JsonPropertyName(name: @"close")]
    [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Global", Justification = "For expansion")]
    public string Close { get; }

    [JsonPropertyName(name: @"quoteAsset")]
    public string QuoteAsset { get; }

    [JsonPropertyName(name: @"active")]
    [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Global", Justification = "For expansion")]
    public bool Active { get; }

    [JsonPropertyName(name: @"minTrade")]
    [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Global", Justification = "For expansion")]
    public decimal MinTrade { get; }

    [JsonPropertyName(name: @"activeSell")]
    [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Global", Justification = "For expansion")]
    public double ActiveSell { get; }

    [JsonPropertyName(name: @"withdrawFee")]
    [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Global", Justification = "For expansion")]
    public string WithdrawFee { get; }

    [JsonPropertyName(name: @"volume")]
    public decimal Volume { get; }

    [JsonPropertyName(name: @"decimalPlaces")]
    [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Global", Justification = "For expansion")]
    public long DecimalPlaces { get; }

    [JsonPropertyName(name: @"quoteAssetUnit")]
    [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Global", Justification = "For expansion")]
    public string QuoteAssetUnit { get; }

    [JsonPropertyName(name: @"open")]
    [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Global", Justification = "For expansion")]
    public string Open { get; }

    [JsonPropertyName(name: @"status")]
    [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Global", Justification = "For expansion")]
    public string Status { get; }

    [JsonPropertyName(name: @"minQty")]
    [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Global", Justification = "For expansion")]
    public double MinQty { get; }
}