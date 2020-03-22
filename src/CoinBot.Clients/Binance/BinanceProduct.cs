using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace CoinBot.Clients.Binance
{
    [SuppressMessage(category: "Microsoft.Performance", checkId: "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Used as data packet")]
    internal sealed class BinanceProduct
    {
        [JsonPropertyName(name: @"symbol")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string Symbol { get; set; } = default!;

        [JsonPropertyName(name: @"quoteAssetName")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string QuoteAssetName { get; set; } = default!;

        [JsonPropertyName(name: @"tradedMoney")]
        public double TradedMoney { get; set; }

        [JsonPropertyName(name: @"baseAssetUnit")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string BaseAssetUnit { get; set; } = default!;

        [JsonPropertyName(name: @"baseAssetName")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string BaseAssetName { get; set; } = default!;

        [JsonPropertyName(name: @"baseAsset")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string BaseAsset { get; set; } = default!;

        [JsonPropertyName(name: @"tickSize")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string TickSize { get; set; } = default!;

        [JsonPropertyName(name: @"prevClose")]
        public decimal? PrevClose { get; set; }

        [JsonPropertyName(name: @"activeBuy")]
        public decimal ActiveBuy { get; set; }

        [JsonPropertyName(name: @"high")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string High { get; set; } = default!;

        [JsonPropertyName(name: @"lastAggTradeId")]
        public long LastAggTradeId { get; set; }

        [JsonPropertyName(name: @"low")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string Low { get; set; } = default!;

        [JsonPropertyName(name: @"matchingUnitType")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string MatchingUnitType { get; set; } = default!;

        [JsonPropertyName(name: @"close")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string Close { get; set; } = default!;

        [JsonPropertyName(name: @"quoteAsset")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string QuoteAsset { get; set; } = default!;

        [JsonPropertyName(name: @"active")]
        public bool Active { get; set; }

        [JsonPropertyName(name: @"minTrade")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public decimal MinTrade { get; set; }

        [JsonPropertyName(name: @"activeSell")]
        public double ActiveSell { get; set; }

        [JsonPropertyName(name: @"withdrawFee")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string WithdrawFee { get; set; } = default!;

        [JsonPropertyName(name: @"volume")]
        public decimal Volume { get; set; }

        [JsonPropertyName(name: @"decimalPlaces")]
        public long DecimalPlaces { get; set; }

        [JsonPropertyName(name: @"quoteAssetUnit")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string QuoteAssetUnit { get; set; } = default!;

        [JsonPropertyName(name: @"open")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string Open { get; set; } = default!;

        [JsonPropertyName(name: @"status")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string Status { get; set; } = default!;

        [JsonPropertyName(name: @"minQty")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public double MinQty { get; set; } = default!;
    }
}