using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CoinBot.Core;
using CoinBot.Core.Extensions;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.Logging;

namespace CoinBot.Discord.Commands
{
	public class EventCommands : CommandBase
	{
		private readonly CurrencyManager _currencyManager;
		private readonly EventManager _eventManager;
		private readonly ILogger _logger;

		public EventCommands(CurrencyManager currencyManager, EventManager eventManager, ILogger logger)
		{
			this._currencyManager = currencyManager ?? throw new ArgumentNullException(nameof(currencyManager));
			this._eventManager = eventManager ?? throw new ArgumentNullException(nameof(eventManager));
			this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		[Command("events")]
		[Summary("get events for a coin, e.g. `!events FUN`.")]
		public async Task Events([Remainder] [Summary("The input as a single coin symbol")]
			string input)
		{
			using (this.Context.Channel.EnterTypingState())
				try
				{
					var currency = this._currencyManager.Get(input);
					var builder = new EmbedBuilder();
					builder
						.WithTitle($"{currency.GetTitle()} events:")
						.WithThumbnailUrl(currency.ImageUrl);

					var result = await this._eventManager.Get(currency);
					foreach (var e in result.Events)
					{
						var eventDetails = new StringBuilder();
						
						if (e.ProofImage != null)
							eventDetails.AppendLine(this.GetEventFooter(e));
						eventDetails.AppendLine($"```{e.Description}```");
						string title = e.IsDeadline
							? $"(deadline) {e.Date:dddd dd MMMM} - {e.Title}"
							: $"{e.Date:dddd dd MMMM} - {e.Title}";
						
						builder.AddField(title, eventDetails);
					}

					builder.AddField("More events", "Visit https://www.coinmarketcal.com for more events!");
					AddFooter(builder, result.LastUpdated);
					await this.ReplyAsync($"events for `{currency.Name}`:", false, builder.Build());
				}
				catch (Exception e)
				{
					this._logger.LogError(0, e, $"Something went wrong while processing the !events command with input '{input}'");
					await this.ReplyAsync("oops, something went wrong, sorry!");
				}
		}

		private string GetEventFooter(EventDto e)
		{
			List<string> parts = new List<string>();
			if (e.ProofImage != null)
				parts.Add($"[proof]({e.ProofImage})");

			if (e.ProofSource != null)
				parts.Add($"[source]({e.ProofSource})");

			parts.Add($"Reliability: {e.Reliability}%");

			string footer = string.Join(" | ", parts);
			return footer;
		}
	}
}
