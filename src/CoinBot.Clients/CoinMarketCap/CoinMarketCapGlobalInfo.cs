using System;
using CoinBot.Core;
using Newtonsoft.Json;

namespace CoinBot.Clients.CoinMarketCap
{
	/// <inheritdoc />
	[JsonObject]
	public class CoinMarketCapGlobalInfo : IGlobalInfo
	{
		/// <inheritdoc />
		[JsonProperty("total_market_cap_usd")]
		public double? MarketCap { get; set; }

		/// <inheritdoc />
		[JsonProperty("total_24h_volume_usd")]
		public double? Volume { get; set; }

		/// <inheritdoc />
		[JsonProperty("bitcoin_percentage_of_market_cap")]
		public double? BtcDominance { get; set; }

		/// <inheritdoc />
		[JsonProperty("active_currencies")]
		public int? Currencies { get; set; }

		/// <inheritdoc />
		[JsonProperty("active_assets")]
		public int? Assets { get; set; }

		/// <inheritdoc />
		[JsonProperty("active_markets")]
		public int? Markets { get; set; }

		/// <inheritdoc />
		[JsonProperty("last_updated")]
		[JsonConverter(typeof(UnixTimeConverter))]
		public DateTime? LastUpdated { get; set; }
	}
}
