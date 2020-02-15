using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace CoinBot.Clients.Binance
{
    [SuppressMessage(category: "Microsoft.Performance", checkId: "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Used as data packet")]
    internal sealed class BinanceProduct
    {
        [JsonProperty("symbol")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string Symbol { get; set; } = default!;

        [JsonProperty("quoteAssetName")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string QuoteAssetName { get; set; } = default!;

        [JsonProperty("tradedMoney")]
        public double TradedMoney { get; set; }

        [JsonProperty("baseAssetUnit")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string BaseAssetUnit { get; set; } = default!;

        [JsonProperty("baseAssetName")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string BaseAssetName { get; set; } = default!;

        [JsonProperty("baseAsset")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string BaseAsset { get; set; } = default!;

        [JsonProperty("tickSize")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string TickSize { get; set; } = default!;

        [JsonProperty("prevClose")]
        public decimal? PrevClose { get; set; }

        [JsonProperty("activeBuy")]
        public long ActiveBuy { get; set; }

        [JsonProperty("high")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string High { get; set; } = default!;

        [JsonProperty("lastAggTradeId")]
        public long LastAggTradeId { get; set; }

        [JsonProperty("low")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string Low { get; set; } = default!;

        [JsonProperty("matchingUnitType")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string MatchingUnitType { get; set; } = default!;

        [JsonProperty("close")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string Close { get; set; } = default!;

        [JsonProperty("quoteAsset")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string QuoteAsset { get; set; } = default!;

        [JsonProperty("active")]
        public bool Active { get; set; }

        [JsonProperty("minTrade")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string MinTrade { get; set; } = default!;

        [JsonProperty("activeSell")]
        public double ActiveSell { get; set; }

        [JsonProperty("withdrawFee")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string WithdrawFee { get; set; } = default!;

        [JsonProperty("volume")]
        public decimal? Volume { get; set; }

        [JsonProperty("decimalPlaces")]
        public long DecimalPlaces { get; set; }

        [JsonProperty("quoteAssetUnit")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string QuoteAssetUnit { get; set; } = default!;

        [JsonProperty("open")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string Open { get; set; } = default!;

        [JsonProperty("status")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string Status { get; set; } = default!;

        [JsonProperty("minQty")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string MinQty { get; set; } = default!;
    }
}