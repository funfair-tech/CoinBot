using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using CoinBot.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace CoinBot.Clients.CoinMarketCal
{
	public class CoinMarketCalClient : IEventClient
	{
		/// <summary>
		/// The <see cref="Uri"/> of the CoinMarketCal endpoint.
		/// </summary>
		private readonly Uri _baseUri = new Uri("https://api.coinmarketcal.com/", UriKind.Absolute);

		/// <summary>
		/// The <see cref="Uri"/> of the CoinMarketCal coins endpoint.
		/// </summary>
		private readonly Uri _coinsUri = new Uri("v1/coins", UriKind.Relative);

		/// <summary>
		/// The <see cref="Uri"/> of the CoinMarketCal events endpoint.
		/// </summary>
		private readonly Uri _eventsUri = new Uri("v1/events", UriKind.Relative);

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
		/// The <see cref="CoinMarketCalSettings"/>.
		/// </summary>
		private readonly CoinMarketCalSettings _settings;

		/// <summary>
		/// The <see cref="Uri"/> of the CoinMarketCal oauth endpoint.
		/// </summary>
		private readonly Uri _tokenUri = new Uri("oauth/v2/token", UriKind.Relative);

		/// <summary>
		/// All the coins.
		/// </summary>
		private IEnumerable<CoinDto> _coins;

		/// <summary>
		/// The CoinMarketCal token.
		/// </summary>
		private string _token;

		/// <summary>
		/// The <see cref="DateTime"/> when the <see cref="_token"/> expires.
		/// </summary>
		private DateTime _tokenExpires;

		/// <summary>
		/// Constructs an <see cref="CoinMarketCalClient"/>.
		/// </summary>
		/// <param name="logger"></param>
		public CoinMarketCalClient(ILogger logger, IOptions<CoinMarketCalSettings> settings)
		{
			this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
			this._settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
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
			if (DateTime.Now > this._tokenExpires)
				await this.RenewAccessToken();

			// TODO: Interval update
			if (this._coins == null)
				this._coins = await this.GetCoins();

			const int max = 5;
			var coin = this._coins.SingleOrDefault(c => c.Symbol.Equals(currency.Symbol, StringComparison.OrdinalIgnoreCase));
			if (coin == null)
				return null;

			var uriBuilder = new UriBuilder(new Uri(this._baseUri, this._eventsUri));
			var query = HttpUtility.ParseQueryString(uriBuilder.Query);
			query["access_token"] = this._token;
			query["showPastEvent"] = "false";
			query["max"] = max.ToString();
			query["coins"] = $"{coin.Id}";
			uriBuilder.Query = query.ToString();

			using (var response = await this._httpClient.GetAsync(uriBuilder.Uri))
			{
				string json = await response.Content.ReadAsStringAsync();
				List<EventDto> events = JsonConvert.DeserializeObject<List<CoinMarketCalEventDto>>(json, this._serializerSettings)
					.Select(e => new EventDto
					{
						Title = e.Title,
						Description = e.Description,
						Currency = currency,
						Date = e.DateEvent,
						ProofSource = e.Source != null ? new Uri(e.Source) : null,
						ProofImage = e.Proof != null ? new Uri(e.Proof) : null,
						CanOccurBefore = e.CanOccurBefore,
						Reliability = e.Percentage
					}).ToList();

				return new EventsDto
				{
					LastUpdated = DateTime.Now,
					Events = events
				};
			}
		}

		public async Task Initialize()
		{
			await this.RenewAccessToken();
			this._coins = await this.GetCoins();
		}

		/// <summary>
		/// Gets an access token.
		/// </summary>
		/// <returns></returns>
		private async Task RenewAccessToken()
		{
			var uriBuilder = new UriBuilder(new Uri(this._baseUri, this._tokenUri));
			var query = HttpUtility.ParseQueryString(uriBuilder.Query);
			query["grant_type"] = "client_credentials";
			query["client_id"] = this._settings.ClientId;
			query["client_secret"] = this._settings.ClientSecret;
			uriBuilder.Query = query.ToString();

			using (var response = await this._httpClient.GetAsync(uriBuilder.Uri))
			{
				string json = await response.Content.ReadAsStringAsync();
				TokenResponseDto tokenResult = JsonConvert.DeserializeObject<TokenResponseDto>(json, this._serializerSettings);
				this._token = tokenResult.AccessToken;
				this._tokenExpires = DateTime.Now.AddSeconds(tokenResult.ExpiresIn);
			}
		}

		/// <summary>
		/// Gets all the coins.
		/// </summary>
		/// <returns></returns>
		private async Task<IEnumerable<CoinDto>> GetCoins()
		{
			var uriBuilder = new UriBuilder(new Uri(this._baseUri, this._coinsUri));
			var query = HttpUtility.ParseQueryString(uriBuilder.Query);
			query["access_token"] = this._token;
			uriBuilder.Query = query.ToString();

			using (var response = await this._httpClient.GetAsync(uriBuilder.Uri))
			{
				string json = await response.Content.ReadAsStringAsync();
				IEnumerable<CoinDto> coins = JsonConvert.DeserializeObject<IEnumerable<CoinDto>>(json, this._serializerSettings);
				return coins;
			}
		}
	}
}
