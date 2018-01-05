using Newtonsoft.Json;

namespace CoinBot.Clients.Kraken
{
	public class KrakenTicker
	{
		public string BaseCurrency { get; set; }
		public string QuoteCurrency { get; set; }

		/// <summary>
		/// last trade closed array(price, lot volume)
		/// </summary>
		[JsonProperty("c")]
		public decimal?[] Last { get; set; }

		/// <summary>
		/// 24 Hour volume array(today, last 24 hours)
		/// </summary>
		[JsonProperty("v")]
		public decimal?[] Volume { get; set; }
	}
}
