using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace CoinBot.Clients.Binance
{
    [SuppressMessage(category: "Microsoft.Performance", checkId: "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Used as data packet")]
    internal sealed class BinanceProduct
    {
        [JsonProperty(propertyName: "symbol")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string Symbol { get; set; } = default!;

        [JsonProperty(propertyName: "quoteAssetName")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string QuoteAssetName { get; set; } = default!;

        [JsonProperty(propertyName: "tradedMoney")]
        public double TradedMoney { get; set; }

        [JsonProperty(propertyName: "baseAssetUnit")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string BaseAssetUnit { get; set; } = default!;

        [JsonProperty(propertyName: "baseAssetName")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string BaseAssetName { get; set; } = default!;

        [JsonProperty(propertyName: "baseAsset")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string BaseAsset { get; set; } = default!;

        [JsonProperty(propertyName: "tickSize")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string TickSize { get; set; } = default!;

        [JsonProperty(propertyName: "prevClose")]
        public decimal? PrevClose { get; set; }

        [JsonProperty(propertyName: "activeBuy")]
        public long ActiveBuy { get; set; }

        [JsonProperty(propertyName: "high")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string High { get; set; } = default!;

        [JsonProperty(propertyName: "lastAggTradeId")]
        public long LastAggTradeId { get; set; }

        [JsonProperty(propertyName: "low")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string Low { get; set; } = default!;

        [JsonProperty(propertyName: "matchingUnitType")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string MatchingUnitType { get; set; } = default!;

        [JsonProperty(propertyName: "close")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string Close { get; set; } = default!;

        [JsonProperty(propertyName: "quoteAsset")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string QuoteAsset { get; set; } = default!;

        [JsonProperty(propertyName: "active")]
        public bool Active { get; set; }

        [JsonProperty(propertyName: "minTrade")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string MinTrade { get; set; } = default!;

        [JsonProperty(propertyName: "activeSell")]
        public double ActiveSell { get; set; }

        [JsonProperty(propertyName: "withdrawFee")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string WithdrawFee { get; set; } = default!;

        [JsonProperty(propertyName: "volume")]
        public decimal? Volume { get; set; }

        [JsonProperty(propertyName: "decimalPlaces")]
        public long DecimalPlaces { get; set; }

        [JsonProperty(propertyName: "quoteAssetUnit")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string QuoteAssetUnit { get; set; } = default!;

        [JsonProperty(propertyName: "open")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string Open { get; set; } = default!;

        [JsonProperty(propertyName: "status")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string Status { get; set; } = default!;

        [JsonProperty(propertyName: "minQty")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string MinQty { get; set; } = default!;
    }
}