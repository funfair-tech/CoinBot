using System;
using Newtonsoft.Json;

namespace CoinBot.Clients.Bittrex
{
	internal sealed class BittrexMarketSummaryDto
	{
		[JsonProperty("MarketName")]
		public string MarketName { get; set; }

		[JsonProperty("High")]
		public decimal? High { get; set; }

		[JsonProperty("Low")]
		public decimal? Low { get; set; }

		[JsonProperty("Volume")]
		public decimal? Volume { get; set; }

		[JsonProperty("Last")]
		public decimal? Last { get; set; }

		[JsonProperty("BaseVolume")]
		public decimal? BaseVolume { get; set; }

		[JsonProperty("TimeStamp")]
		public DateTime? TimeStamp { get; set; }

		[JsonProperty("Bid")]
		public decimal? Bid { get; set; }

		[JsonProperty("Ask")]
		public decimal? Ask { get; set; }

		//[JsonProperty("OpenBuyOrders")]
		//public long OpenBuyOrders { get; set; }

		//[JsonProperty("OpenSellOrders")]
		//public long OpenSellOrders { get; set; }

		//[JsonProperty("PrevDay")]
		//public double PrevDay { get; set; }

		//[JsonProperty("Created")]
		//public string Created { get; set; }
	}
}