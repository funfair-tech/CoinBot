using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CoinBot.Core;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CoinBot.Clients.GateIo
{
	public class GateIoClient : IMarketClient
	{
		private const char PairSeparator = '_';

		/// <summary>
		/// The Exchange name.
		/// </summary>
		public string Name => "Gate.io";

		/// <summary>
		/// The <see cref="CurrencyManager"/>.
		/// </summary>
		private readonly CurrencyManager _currencyManager;

		/// <summary>
		/// The <see cref="Uri"/> of the CoinMarketCap endpoint.
		/// </summary>
		private readonly Uri _endpoint = new Uri("http://data.gate.io/api2/1/", UriKind.Absolute);

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

		public GateIoClient(ILogger logger, CurrencyManager currencyManager)
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
				return tickers.Select(m => new MarketSummaryDto
				{
					BaseCurrrency = _currencyManager.Get(m.Pair.Substring(0, m.Pair.IndexOf(PairSeparator))),
					MarketCurrency = _currencyManager.Get(m.Pair.Substring(m.Pair.IndexOf(PairSeparator) + 1)),
					Market = "Gate.io",
					Volume = m.BaseVolume,
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
		private async Task<List<GateIoTicker>> GetTickers()
		{
			using (var response = await _httpClient.GetAsync(new Uri("tickers", UriKind.Relative)))
			{
				var json = await response.Content.ReadAsStringAsync();
				var jResponse = JObject.Parse(json);
				var tickers = new List<GateIoTicker>();
				foreach (var jToken in jResponse)
				{
					var obj = JObject.Parse(jToken.Value.ToString());
					var ticker = JsonConvert.DeserializeObject<GateIoTicker>(obj.ToString(), _serializerSettings);
					ticker.Pair = jToken.Key;
					tickers.Add(ticker);
				}

				return tickers;
			}
		}
	}
}
