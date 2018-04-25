using Newtonsoft.Json;

namespace CoinBot.Clients.CoinMarketCal
{
	public class CoinDto
	{
		[JsonProperty("id", Required = Required.Always)]
		public string Id { get; set; }

		[JsonProperty("name", Required = Required.Always)]
		public string Name { get; set; }

		[JsonProperty("symbol", Required = Required.Always)]
		public string Symbol { get; set; }
	}
}
