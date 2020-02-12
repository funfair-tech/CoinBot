using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace CoinBot.Clients.Binance
{
    [SuppressMessage(category: "Microsoft.Performance", checkId: "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Used as data packet")]
    internal sealed class BinanceProduct
    {
        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("quoteAssetName")]
        public string QuoteAssetName { get; set; }

        [JsonProperty("tradedMoney")]
        public double TradedMoney { get; set; }

        [JsonProperty("baseAssetUnit")]
        public string BaseAssetUnit { get; set; }

        [JsonProperty("baseAssetName")]
        public string BaseAssetName { get; set; }

        [JsonProperty("baseAsset")]
        public string BaseAsset { get; set; }

        [JsonProperty("tickSize")]
        public string TickSize { get; set; }

        [JsonProperty("prevClose")]
        public decimal? PrevClose { get; set; }

        [JsonProperty("activeBuy")]
        public long ActiveBuy { get; set; }

        [JsonProperty("high")]
        public string High { get; set; }

        [JsonProperty("lastAggTradeId")]
        public long LastAggTradeId { get; set; }

        [JsonProperty("low")]
        public string Low { get; set; }

        [JsonProperty("matchingUnitType")]
        public string MatchingUnitType { get; set; }

        [JsonProperty("close")]
        public string Close { get; set; }

        [JsonProperty("quoteAsset")]
        public string QuoteAsset { get; set; }

        [JsonProperty("active")]
        public bool Active { get; set; }

        [JsonProperty("minTrade")]
        public string MinTrade { get; set; }

        [JsonProperty("activeSell")]
        public double ActiveSell { get; set; }

        [JsonProperty("withdrawFee")]
        public string WithdrawFee { get; set; }

        [JsonProperty("volume")]
        public decimal? Volume { get; set; }

        [JsonProperty("decimalPlaces")]
        public long DecimalPlaces { get; set; }

        [JsonProperty("quoteAssetUnit")]
        public string QuoteAssetUnit { get; set; }

        [JsonProperty("open")]
        public string Open { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("minQty")]
        public string MinQty { get; set; }
    }
}