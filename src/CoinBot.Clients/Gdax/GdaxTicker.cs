using System;
using Newtonsoft.Json;

namespace CoinBot.Clients.Gdax
{
	internal sealed class GdaxTicker
	{
		public string ProductId;

		[JsonProperty("ask")]
		public string Ask { get; set; }

		[JsonProperty("bid")]
		public string Bid { get; set; }

		[JsonProperty("price")]
		public decimal? Price { get; set; }

		[JsonProperty("size")]
		public string Size { get; set; }

		[JsonProperty("time")]
		public DateTime? Time { get; set; }

		[JsonProperty("trade_id")]
		public long TradeId { get; set; }

		[JsonProperty("volume")]
		public decimal? Volume { get; set; }
	}
}
