using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace CoinBot.Core
{
	public class EventManager
	{
		/// <summary>
		/// The <see cref="IEventClient"/>s.
		/// </summary>
		private readonly IEnumerable<IEventClient> _eventClients;

		/// <summary>
		/// The <see cref="IMemoryCache"/>.
		/// </summary>
		private readonly IMemoryCache _cache;

		/// <summary>
		/// Constructs an <see cref="EventManager"/>.
		/// </summary>
		/// <param name="eventClients">The <see cref="IEventClient"/>s.</param>
		/// <param name="cache">The <see cref="IMemoryCache"/>.</param>
		/// <param name="logger">The <see cref="ILogger"/>.</param>
		public EventManager(IEnumerable<IEventClient> eventClients, IMemoryCache cache, ILogger logger)
		{
			this._eventClients = eventClients ?? throw new ArgumentNullException(nameof(eventClients));
			this._cache = cache ?? throw new ArgumentNullException(nameof(cache));
		}

		/// <summary>
		/// Get events for the given <paramref name="currency"/>.
		/// </summary>
		/// <param name="currency">The <see cref="Currency"/>.</param>
		/// <returns></returns>
		public async Task<EventsDto> Get(Currency currency)
		{
			string cacheKey = $"events-{currency.Name}+{currency.Symbol}";
			if (this._cache.TryGetValue(cacheKey, out EventsDto events))
				return events;

			var eventClient = this._eventClients.First();
			events = await eventClient.GetEvents(currency);

			using (var entry = this._cache.CreateEntry(cacheKey))
			{
				entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(30));
				entry.SetValue(events);
			}

			return events;
		}
	}
}
