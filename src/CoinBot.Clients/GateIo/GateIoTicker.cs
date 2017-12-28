using Newtonsoft.Json;

namespace CoinBot.Clients.GateIo
{
	public class GateIoTicker
	{
		public string Pair { get; set; }

		[JsonProperty("result")]
		public string Result { get; set; }

		[JsonProperty("last")]
		public decimal? Last { get; set; }

		[JsonProperty("lowestAsk")]
		public decimal? LowestAsk { get; set; }

		[JsonProperty("highestBid")]
		public decimal? HighestBid { get; set; }

		[JsonProperty("percentChange")]
		public decimal? PercentChange { get; set; }

		[JsonProperty("baseVolume")]
		public decimal? BaseVolume { get; set; }

		[JsonProperty("quoteVolume")]
		public decimal? QuoteVolume { get; set; }

		[JsonProperty("high24hr")]
		public decimal? High24Hr { get; set; }

		[JsonProperty("low24hr")]
		public decimal? Low24Hr { get; set; }
	}
}
