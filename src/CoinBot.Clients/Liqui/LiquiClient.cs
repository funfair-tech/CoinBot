using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CoinBot.Core;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CoinBot.Clients.Liqui
{
	public class LiquiClient : IMarketClient
	{
		/// <summary>
		/// The <see cref="CurrencyManager"/>.
		/// </summary>
		private readonly CurrencyManager _currencyManager;

		/// <summary>
		/// The pair separator character.
		/// </summary>
		private const char PairSeparator = '_';

		/// <summary>
		/// The Exchange name.
		/// </summary>
		public string Name => "Liqui";

		/// <summary>
		/// The <see cref="Uri"/> of the CoinMarketCap endpoint.
		/// </summary>
		private readonly Uri _endpoint = new Uri("https://api.liqui.io/api/3/", UriKind.Absolute);

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

		public LiquiClient(ILogger logger, CurrencyManager currencyManager)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_currencyManager = currencyManager ?? throw new ArgumentNullException(nameof(currencyManager));
			_httpClient = new HttpClient
			{
				BaseAddress = _endpoint
			};

			_serializerSettings = new JsonSerializerSettings
			{
				Error = (sender, args) =>
				{
					var eventId = new EventId(args.ErrorContext.Error.HResult);
					var ex = args.ErrorContext.Error.GetBaseException();
					_logger.LogError(eventId, ex, ex.Message);
				}
			};
		}

		/// <inheritdoc/>
		public async Task<IReadOnlyCollection<MarketSummaryDto>> Get()
		{
			try
			{
				var pairs = await GetPairs();
				var tickers = new List<LiquiTicker>();
				foreach(var pair in pairs)
					tickers.Add(await GetTicker(pair));
				
				return tickers.Select(m => new MarketSummaryDto
				{
					BaseCurrrency = _currencyManager.Get(m.Pair.Substring(0, m.Pair.IndexOf(PairSeparator))),
					MarketCurrency = _currencyManager.Get(m.Pair.Substring(m.Pair.IndexOf(PairSeparator) + 1)),
					Market = "Liqui",
					Volume = m.Vol,
					LastUpdated = m.Updated,
					Last = m.Last
				}).ToList();
			}
			catch (Exception e)
			{
				var eventId = new EventId(e.HResult);
				_logger.LogError(eventId, e, e.Message);
				throw;
			}
		}

		/// <summary>
		/// Get the market summaries.
		/// </summary>
		/// <returns></returns>
		private async Task<List<string>> GetPairs()
		{
			using (var response = await _httpClient.GetAsync(new Uri("info", UriKind.Relative)))
			{
				var json = await response.Content.ReadAsStringAsync();
				var jResponse = JObject.Parse(json);
				var pairs = jResponse.GetValue("pairs").Children().Cast<JProperty>().Select(property => property.Name).ToList();
				return pairs;
			}
		}

		/// <summary>
		/// Get the ticker.
		/// </summary>
		/// <returns></returns>
		private async Task<LiquiTicker> GetTicker(string pair)
		{
			using (var response = await _httpClient.GetAsync(new Uri($"ticker/{pair}", UriKind.Relative)))
			{
				var json = await response.Content.ReadAsStringAsync();
				var jObject = JObject.Parse(json);
				var ticker = JsonConvert.DeserializeObject<LiquiTicker>(jObject.GetValue(pair).ToString(), _serializerSettings);
				ticker.Pair = pair;
				return ticker;
			}
		}
	}
}
