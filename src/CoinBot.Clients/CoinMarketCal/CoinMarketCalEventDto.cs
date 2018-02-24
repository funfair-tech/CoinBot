using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CoinBot.Clients.CoinMarketCal
{
	[JsonObject]
	internal sealed class CoinMarketCalEventDto
	{
		[JsonProperty("id", Required = Required.Always)]
		public int Id { get; set; }

		[JsonProperty("title", Required = Required.Always)]
		public string Title { get; set; }

		[JsonProperty("date_event", Required = Required.Always)]
		public DateTime DateEvent { get; set; }

		[JsonProperty("created_date", Required = Required.Always)]
		public DateTime CreatedDate { get; set; }

		[JsonProperty("description", Required = Required.Always)]
		public string Description { get; set; }

		[JsonProperty("proof", Required = Required.Always)]
		public string Proof { get; set; }

		[JsonProperty("source", Required = Required.Default)]
		public string Source { get; set; }

		[JsonProperty("is_hot", Required = Required.Always)]
		public bool IsHot { get; set; }

		[JsonProperty("vote_count", Required = Required.Always)]
		public int VoteCount { get; set; }

		[JsonProperty("positive_vote_count", Required = Required.Always)]
		public int PositiveVoteCount { get; set; }

		[JsonProperty("percentage", Required = Required.Always)]
		public int Percentage { get; set; }

		[JsonProperty("can_occur_before", Required = Required.Always)]
		public bool CanOccurBefore { get; set; }
	}
}
