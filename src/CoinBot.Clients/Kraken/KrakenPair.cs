using Newtonsoft.Json;

namespace CoinBot.Clients.Kraken
{
	public class KrakenPair
	{
		public string PairId { get; set; }

		[JsonProperty("base")]
		public string BaseCurrency { get; set; }
		
		[JsonProperty("quote")]
		public string QuoteCurrency { get; set; }
	}
}
