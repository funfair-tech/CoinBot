using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using CoinBot.Core;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CoinBot.Clients.CoinMarketCal
{
	public class CoinMarketCalClient : IEventClient
	{
		/// <summary>
		/// The <see cref="Uri"/> of the CoinMarketCal endpoint.
		/// </summary>
		private readonly Uri _baseUri = new Uri("https://coinmarketcal.com/api/", UriKind.Absolute);

		/// <summary>
		/// The <see cref="CurrencyManager"/>.
		/// </summary>
		private readonly CurrencyManager _currencyManager;

		/// <summary>
		/// The <see cref="Uri"/> of the CoinMarketCal endpoint.
		/// </summary>
		private readonly Uri _eventsUri = new Uri("events", UriKind.Relative);

		/// <summary>
		/// The <see cref="HttpClient"/>.
		/// </summary>
		private readonly HttpClient _httpClient;

		/// <summary>
		/// The <see cref="ILogger"/>.
		/// </summary>
		private readonly ILogger _logger;

		/// <summary>
		/// The <see cref="JsonSerializerSettings"/>.
		/// </summary>
		private readonly JsonSerializerSettings _serializerSettings;

		/// <summary>
		/// Constructs an <see cref="CoinMarketCalClient"/>.
		/// </summary>
		/// <param name="currencyManager"></param>
		/// <param name="logger"></param>
		public CoinMarketCalClient(CurrencyManager currencyManager, ILogger logger)
		{
			this._currencyManager = currencyManager ?? throw new ArgumentNullException(nameof(currencyManager));
			this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
			this._httpClient = new HttpClient
			{
				BaseAddress = this._baseUri
			};

			this._serializerSettings = new JsonSerializerSettings
			{
				Error = (sender, args) =>
				{
					var ex = args.ErrorContext.Error.GetBaseException();
					this._logger.LogError(new EventId(args.ErrorContext.Error.HResult), ex, ex.Message);
				}
			};
		}

		/// <summary>
		/// Get events for a given <see cref="Currency"/>.
		/// </summary>
		/// <returns></returns>
		public async Task<EventsDto> GetEvents(Currency currency)
		{
			const int max = 5;
			var uriBuilder = new UriBuilder(new Uri(this._baseUri, this._eventsUri));
			var query = HttpUtility.ParseQueryString(uriBuilder.Query);
			query["showPastEvent"] = "false";
			query["max"] = max.ToString();
			query["coins"] = $"{currency.Name} ({currency.Symbol})";
			uriBuilder.Query = query.ToString();

			using (var response = await this._httpClient.GetAsync(uriBuilder.Uri))
			{
				string json = await response.Content.ReadAsStringAsync();
				List<EventDto> events = JsonConvert.DeserializeObject<List<CoinMarketCalEventDto>>(json, this._serializerSettings)
					.Select(e => new EventDto
					{
						Title = e.Title,
						Description = e.Description,
						Currency = this._currencyManager.Get(e.CoinSymbol),
						Date = e.DateEvent,
						ProofSource = e.Source != null ? new Uri(e.Source) : null,
						ProofImage = e.Proof != null ? new Uri(e.Proof) : null,
						IsDeadline = e.EventIsDeadline,
						Reliability = e.Percentage
					}).ToList();

				return new EventsDto
				{
					LastUpdated = DateTime.Now,
					Events = events
				};
			}
		}
	}
}
