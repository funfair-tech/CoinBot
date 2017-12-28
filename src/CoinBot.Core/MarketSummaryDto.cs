using System;

namespace CoinBot.Core
{
	public sealed class MarketSummaryDto
	{
		public Currency BaseCurrrency { get; set; }

		public Currency MarketCurrency { get; set; }

		public string Market { get; set; }

		public decimal? Volume { get; set; }

		public decimal? Last { get; set; }

		public DateTime? LastUpdated { get; set; }
	}
}
