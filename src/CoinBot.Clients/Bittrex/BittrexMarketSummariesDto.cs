using System.Collections.Generic;
using Newtonsoft.Json;

namespace CoinBot.Clients.Bittrex
{
	internal sealed class BittrexMarketSummariesDto
	{
		[JsonProperty("result")]
		public List<BittrexMarketSummaryDto> Result { get; set; }
	}
}