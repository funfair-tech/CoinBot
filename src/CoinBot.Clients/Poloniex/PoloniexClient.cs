using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CoinBot.Core;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CoinBot.Clients.Poloniex
{
	public class PoloniexClient : IMarketClient
	{
		/// <summary>
		/// The <see cref="CurrencyManager"/>.
		/// </summary>
		private readonly CurrencyManager _currencyManager;

		/// <summary>
		/// The Exchange name.
		/// </summary>
		public string Name => "Poloniex";

		/// <summary>
		/// The <see cref="Uri"/> of the CoinMarketCap endpoint.
		/// </summary>
		private readonly Uri _endpoint = new Uri("https://poloniex.com/", UriKind.Absolute);

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

		public PoloniexClient(ILogger logger, CurrencyManager currencyManager)
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
				var tickers = await GetTickers();
				return tickers.Select(t => new MarketSummaryDto
				{
					BaseCurrrency = _currencyManager.Get(t.Pair.Substring(0, t.Pair.IndexOf('_'))),
					MarketCurrency = _currencyManager.Get(t.Pair.Substring(t.Pair.IndexOf('_') + 1)),
					Market = "Poloniex",
					Volume = t.BaseVolume,
					Last = t.Last,
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
		private async Task<List<PoloniexTicker>> GetTickers()
		{
			using (var response = await _httpClient.GetAsync(new Uri("public?command=returnTicker", UriKind.Relative)))
			{
				var json = await response.Content.ReadAsStringAsync();
				var jResponse = JObject.Parse(json);
				var tickers = new List<PoloniexTicker>();
				foreach (var jToken in jResponse)
				{
					var obj = JObject.Parse(jToken.Value.ToString());
					var ticker = JsonConvert.DeserializeObject<PoloniexTicker>(obj.ToString(), _serializerSettings);
					ticker.Pair = jToken.Key;
					tickers.Add(ticker);
				}

				return tickers;
			}
		}
	}
}
