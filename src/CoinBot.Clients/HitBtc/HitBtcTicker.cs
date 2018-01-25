using System;
using Newtonsoft.Json;

namespace CoinBot.Clients.HitBtc
{
	[JsonObject]
	internal sealed class HitBtcTicker
	{
		[JsonProperty("ask")]
		public decimal? Ask { get; set; }

		[JsonProperty("bid")]
		public decimal? Bid { get; set; }

		[JsonProperty("last")]
		public decimal? Last { get; set; }

		[JsonProperty("open")]
		public decimal? Open { get; set; }

		[JsonProperty("low")]
		public decimal? Low { get; set; }

		[JsonProperty("high")]
		public decimal? High { get; set; }

		[JsonProperty("volume")]
		public decimal? Volume { get; set; }

		[JsonProperty("volumeQuote")]
		public decimal? VolumeQuote { get; set; }

		[JsonProperty("timestamp")]
		public DateTime? Timestamp { get; set; }

		[JsonProperty("symbol")]
		public string Symbol { get; set; }

		public string BaseCurrency { get; set; }

		public string QuoteCurrency { get; set; }
	}
}
