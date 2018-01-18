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

		[JsonProperty("coin_name", Required = Required.Always)]
		public string CoinName { get; set; }

		[JsonProperty("coin_symbol", Required = Required.Always)]
		public string CoinSymbol { get; set; }

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

		[JsonProperty("categories", Required = Required.Always)]
		public List<string> Categories { get; set; }

		[JsonProperty("tip_symbol", Required = Required.Default)]
		public string TipSymbol { get; set; }

		[JsonProperty("tip_adress", Required = Required.Default)]
		public string TipAdress { get; set; }

		[JsonProperty("twitter_account", Required = Required.Default)]
		public string TwitterAccount { get; set; }

		[JsonProperty("event_is_deadline", Required = Required.Always)]
		public bool EventIsDeadline { get; set; }
	}
}
