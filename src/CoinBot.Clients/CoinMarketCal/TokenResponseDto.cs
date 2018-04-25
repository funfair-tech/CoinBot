using System;
using Newtonsoft.Json;

namespace CoinBot.Clients.CoinMarketCal
{
	public class TokenResponseDto
	{
		[JsonProperty("access_token", Required = Required.Always)]
		public string AccessToken { get; set; }

		[JsonProperty("expires_in", Required = Required.Always)]
		public int ExpiresIn { get; set; }

		[JsonProperty("scope", Required = Required.Default)]
		public object Scope { get; set; }

		[JsonProperty("token_type", Required = Required.Always)]
		public string TokenType { get; set; }
	}
}
