using System;
using System.Collections.Generic;

namespace CoinBot.Core
{
	public sealed class EventsDto
	{
		/// <summary>
		/// Last <see cref="DateTime"/> of the last update.
		/// </summary>
		public DateTime LastUpdated { get; set; }

		/// <summary>
		/// The <see cref="EventDto"/>s.
		/// </summary>
		public List<EventDto> Events { get; set; }
	}
}