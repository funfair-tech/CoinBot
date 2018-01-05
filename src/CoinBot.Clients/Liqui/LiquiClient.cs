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
			this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
			this._currencyManager = currencyManager ?? throw new ArgumentNullException(nameof(currencyManager));
			this._httpClient = new HttpClient
			{
				BaseAddress = this._endpoint
			};

			this._serializerSettings = new JsonSerializerSettings
			{
				Error = (sender, args) =>
				{
					Exception ex = args.ErrorContext.Error.GetBaseException();
					this._logger.LogError(new EventId(args.ErrorContext.Error.HResult), ex, ex.Message);
				}
			};
		}

		/// <inheritdoc/>
		public async Task<IReadOnlyCollection<MarketSummaryDto>> Get()
		{
			try
			{
				List<string> pairs = await this.GetPairs();
				List<LiquiTicker> tickers = new List<LiquiTicker>();

			    foreach (string pair in pairs)
			    {
			        tickers.Add(await this.GetTicker(pair));
			    }

			    return tickers.Select(m => new MarketSummaryDto
				{
					BaseCurrrency = this._currencyManager.Get(m.Pair.Substring(0, m.Pair.IndexOf(PairSeparator))),
					MarketCurrency = this._currencyManager.Get(m.Pair.Substring(m.Pair.IndexOf(PairSeparator) + 1)),
					Market = "Liqui",
					Volume = m.Vol,
					LastUpdated = m.Updated,
					Last = m.Last
				}).ToList();
			}
			catch (Exception e)
			{
				this._logger.LogError(new EventId(e.HResult), e, e.Message);
				throw;
			}
		}

		/// <summary>
		/// Get the market summaries.
		/// </summary>
		/// <returns></returns>
		private async Task<List<string>> GetPairs()
		{
			using (HttpResponseMessage response = await this._httpClient.GetAsync(new Uri("info", UriKind.Relative)))
			{
				string json = await response.Content.ReadAsStringAsync();
				JObject jResponse = JObject.Parse(json);
				List<string> pairs = jResponse.GetValue("pairs").Children().Cast<JProperty>().Select(property => property.Name).ToList();
				return pairs;
			}
		}

		/// <summary>
		/// Get the ticker.
		/// </summary>
		/// <returns></returns>
		private async Task<LiquiTicker> GetTicker(string pair)
		{
			using (HttpResponseMessage response = await this._httpClient.GetAsync(new Uri($"ticker/{pair}", UriKind.Relative)))
			{
				string json = await response.Content.ReadAsStringAsync();
				JObject jObject = JObject.Parse(json);
				LiquiTicker ticker = JsonConvert.DeserializeObject<LiquiTicker>(jObject.GetValue(pair).ToString(), this._serializerSettings);
				ticker.Pair = pair;
				return ticker;
			}
		}
	}
}
