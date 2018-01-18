using Newtonsoft.Json;

namespace CoinBot.Clients.Binance
{
	internal sealed class BinanceTickerDto
	{
		[JsonProperty("symbol")]
		public string Symbol { get; set; }

		[JsonProperty("priceChange")]
		public string PriceChange { get; set; }

		[JsonProperty("priceChangePercent")]
		public string PriceChangePercent { get; set; }

		[JsonProperty("weightedAvgPrice")]
		public string WeightedAvgPrice { get; set; }

		[JsonProperty("prevClosePrice")]
		public string PrevClosePrice { get; set; }

		[JsonProperty("lastPrice")]
		public decimal? LastPrice { get; set; }

		[JsonProperty("lastQty")]
		public string LastQty { get; set; }

		[JsonProperty("bidPrice")]
		public string BidPrice { get; set; }

		[JsonProperty("bidQty")]
		public string BidQty { get; set; }

		[JsonProperty("askPrice")]
		public string AskPrice { get; set; }

		[JsonProperty("askQty")]
		public string AskQty { get; set; }

		[JsonProperty("openPrice")]
		public string OpenPrice { get; set; }

		[JsonProperty("highPrice")]
		public string HighPrice { get; set; }

		[JsonProperty("lowPrice")]
		public string LowPrice { get; set; }

		[JsonProperty("volume")]
		public decimal? Volume { get; set; }

		[JsonProperty("quoteVolume")]
		public string QuoteVolume { get; set; }

		[JsonProperty("openTime")]
		public long OpenTime { get; set; }

		[JsonProperty("closeTime")]
		public long CloseTime { get; set; }

		[JsonProperty("firstId")]
		public int FirstId { get; set; }

		[JsonProperty("lastId")]
		public int LastId { get; set; }

		[JsonProperty("count")]
		public int Count { get; set; }
	}
}