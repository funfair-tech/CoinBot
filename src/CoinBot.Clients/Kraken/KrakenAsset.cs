using Newtonsoft.Json;

namespace CoinBot.Clients.Kraken
{
	public class KrakenAsset
	{
		public string Id;

		[JsonProperty("altname")]
		public string Altname { get; set; }
	}
}
