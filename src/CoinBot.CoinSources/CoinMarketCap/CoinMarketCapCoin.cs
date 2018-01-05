using System;
using Newtonsoft.Json;

namespace CoinBot.CoinSources.CoinMarketCap
{
	/// <inheritdoc />
	[JsonObject]
	public class CoinMarketCapCoin : ICoin
	{
		/// <inheritdoc />
		[JsonProperty("id")]
		public string Id { get; set; }

		/// <inheritdoc />
		[JsonProperty("name")]
		public string Name { get; set; }

		/// <inheritdoc />
		[JsonProperty("symbol")]
		public string Symbol { get; set; }

		/// <inheritdoc />
		[JsonProperty("rank")]
		public int? Rank { get; set; }

		/// <inheritdoc />
		[JsonProperty("price_usd")]
		public double? PriceUsd { get; set; }

		/// <inheritdoc />
		[JsonProperty("price_btc")]
		public decimal? PriceBtc { get; set; }

		/// <inheritdoc />
		[JsonProperty("price_eth")]
		public decimal? PriceEth { get; set; }

		/// <inheritdoc />
		[JsonProperty("24h_volume_usd")]
		public double? Volume { get; set; }

		/// <inheritdoc />
		[JsonProperty("market_cap_usd")]
		public double? MarketCap { get; set; }

		/// <inheritdoc />
		[JsonProperty("available_supply")]
		public decimal? AvailableSupply { get; set; }

		/// <inheritdoc />
		[JsonProperty("total_supply")]
		public decimal? TotalSupply { get; set; }

		/// <inheritdoc />
		[JsonProperty("max_supply")]
		public decimal? MaxSupply { get; set; }

		/// <inheritdoc />
		[JsonProperty("percent_change_1h")]
		public double? HourChange { get; set; }

		/// <inheritdoc />
		[JsonProperty("percent_change_24h")]
		public double? DayChange { get; set; }

		/// <inheritdoc />
		[JsonProperty("percent_change_7d")]
		public double? WeekChange { get; set; }

		/// <inheritdoc />
		[JsonProperty("last_updated")]
		[JsonConverter(typeof(CoinMarketCapEpochConverter))]
		public DateTime? LastUpdated { get; set; }
	}
}