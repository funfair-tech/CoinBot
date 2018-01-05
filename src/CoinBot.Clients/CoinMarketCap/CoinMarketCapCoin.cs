using System;
using CoinBot.Core;
using Newtonsoft.Json;

namespace CoinBot.Clients.CoinMarketCap
{
	[JsonObject]
	public class CoinMarketCapCoin : ICoinInfo
	{
		[JsonProperty("id")]
		public string Id { get; set; }

		public string ImageUrl => $"https://files.coinmarketcap.com/static/img/coins/64x64/{this.Id}.png";

		public string Url => $"https://coinmarketcap.com/currencies/{this.Id}";

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("symbol")]
		public string Symbol { get; set; }

		[JsonProperty("rank")]
		public int? Rank { get; set; }

		[JsonProperty("price_usd")]
		public double? PriceUsd { get; set; }

		[JsonProperty("price_btc")]
		public decimal? PriceBtc { get; set; }

		[JsonProperty("price_eth")]
		public decimal? PriceEth { get; set; }

		[JsonProperty("24h_volume_usd")]
		public double? Volume { get; set; }

		[JsonProperty("market_cap_usd")]
		public double? MarketCap { get; set; }

		[JsonProperty("available_supply")]
		public decimal? AvailableSupply { get; set; }

		[JsonProperty("total_supply")]
		public decimal? TotalSupply { get; set; }

		[JsonProperty("max_supply")]
		public decimal? MaxSupply { get; set; }

		[JsonProperty("percent_change_1h")]
		public double? HourChange { get; set; }

		[JsonProperty("percent_change_24h")]
		public double? DayChange { get; set; }

		[JsonProperty("percent_change_7d")]
		public double? WeekChange { get; set; }

		[JsonProperty("last_updated")]
		[JsonConverter(typeof(UnixTimeConverter))]
		public DateTime? LastUpdated { get; set; }
	}
}
