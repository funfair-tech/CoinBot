using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace CoinBot.Clients.Binance;

[SuppressMessage(category: "Microsoft.Performance", checkId: "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Used as data packet")]
internal sealed class BinanceProduct
{
    [JsonPropertyName(name: @"symbol")]
    [SuppressMessage(category: "ReSharper", checkId: "RedundantDefaultMemberInitializer", Justification = "TODO: Review")]
    public string Symbol { get; set; } = default!;

    [JsonPropertyName(name: @"quoteAssetName")]
    [SuppressMessage(category: "ReSharper", checkId: "RedundantDefaultMemberInitializer", Justification = "TODO: Review")]
    public string QuoteAssetName { get; set; } = default!;

    [JsonPropertyName(name: @"tradedMoney")]
    public double TradedMoney { get; set; }

    [JsonPropertyName(name: @"baseAssetUnit")]
    [SuppressMessage(category: "ReSharper", checkId: "RedundantDefaultMemberInitializer", Justification = "TODO: Review")]
    public string BaseAssetUnit { get; set; } = default!;

    [JsonPropertyName(name: @"baseAssetName")]
    [SuppressMessage(category: "ReSharper", checkId: "RedundantDefaultMemberInitializer", Justification = "TODO: Review")]
    public string BaseAssetName { get; set; } = default!;

    [JsonPropertyName(name: @"baseAsset")]
    [SuppressMessage(category: "ReSharper", checkId: "RedundantDefaultMemberInitializer", Justification = "TODO: Review")]
    public string BaseAsset { get; set; } = default!;

    [JsonPropertyName(name: @"tickSize")]
    [SuppressMessage(category: "ReSharper", checkId: "RedundantDefaultMemberInitializer", Justification = "TODO: Review")]
    public string TickSize { get; set; } = default!;

    [JsonPropertyName(name: @"prevClose")]
    public decimal? PrevClose { get; set; }

    [JsonPropertyName(name: @"activeBuy")]
    public decimal ActiveBuy { get; set; }

    [JsonPropertyName(name: @"high")]
    [SuppressMessage(category: "ReSharper", checkId: "RedundantDefaultMemberInitializer", Justification = "TODO: Review")]
    public string High { get; set; } = default!;

    [JsonPropertyName(name: @"lastAggTradeId")]
    public long LastAggTradeId { get; set; }

    [JsonPropertyName(name: @"low")]
    [SuppressMessage(category: "ReSharper", checkId: "RedundantDefaultMemberInitializer", Justification = "TODO: Review")]
    public string Low { get; set; } = default!;

    [JsonPropertyName(name: @"matchingUnitType")]
    [SuppressMessage(category: "ReSharper", checkId: "RedundantDefaultMemberInitializer", Justification = "TODO: Review")]
    public string MatchingUnitType { get; set; } = default!;

    [JsonPropertyName(name: @"close")]
    [SuppressMessage(category: "ReSharper", checkId: "RedundantDefaultMemberInitializer", Justification = "TODO: Review")]
    public string Close { get; set; } = default!;

    [JsonPropertyName(name: @"quoteAsset")]
    [SuppressMessage(category: "ReSharper", checkId: "RedundantDefaultMemberInitializer", Justification = "TODO: Review")]
    public string QuoteAsset { get; set; } = default!;

    [JsonPropertyName(name: @"active")]
    public bool Active { get; set; }

    [JsonPropertyName(name: @"minTrade")]
    [SuppressMessage(category: "ReSharper", checkId: "RedundantDefaultMemberInitializer", Justification = "TODO: Review")]
    public decimal MinTrade { get; set; }

    [JsonPropertyName(name: @"activeSell")]
    public double ActiveSell { get; set; }

    [JsonPropertyName(name: @"withdrawFee")]
    [SuppressMessage(category: "ReSharper", checkId: "RedundantDefaultMemberInitializer", Justification = "TODO: Review")]
    public string WithdrawFee { get; set; } = default!;

    [JsonPropertyName(name: @"volume")]
    public decimal Volume { get; set; }

    [JsonPropertyName(name: @"decimalPlaces")]
    public long DecimalPlaces { get; set; }

    [JsonPropertyName(name: @"quoteAssetUnit")]
    [SuppressMessage(category: "ReSharper", checkId: "RedundantDefaultMemberInitializer", Justification = "TODO: Review")]
    public string QuoteAssetUnit { get; set; } = default!;

    [JsonPropertyName(name: @"open")]
    [SuppressMessage(category: "ReSharper", checkId: "RedundantDefaultMemberInitializer", Justification = "TODO: Review")]
    public string Open { get; set; } = default!;

    [JsonPropertyName(name: @"status")]
    [SuppressMessage(category: "ReSharper", checkId: "RedundantDefaultMemberInitializer", Justification = "TODO: Review")]
    public string Status { get; set; } = default!;

    [JsonPropertyName(name: @"minQty")]
    [SuppressMessage(category: "ReSharper", checkId: "RedundantDefaultMemberInitializer", Justification = "TODO: Review")]
    public double MinQty { get; set; } = default!;
}