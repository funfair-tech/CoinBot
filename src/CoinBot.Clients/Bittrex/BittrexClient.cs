using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CoinBot.Core;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CoinBot.Clients.Bittrex
{
	public class BittrexClient : IMarketClient
	{
		/// <summary>
		/// The <see cref="CurrencyManager"/>.
		/// </summary>
		private readonly CurrencyManager _currencyManager;

		/// <summary>
		/// The Exchange name.
		/// </summary>
		public string Name => "Bittrex";

		/// <summary>
		/// The <see cref="Uri"/> of the CoinMarketCap endpoint.
		/// </summary>
		private readonly Uri _endpoint = new Uri("https://bittrex.com/api/v1.1/public/", UriKind.Absolute);

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

		public BittrexClient(ILogger logger, CurrencyManager currencyManager)
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
				var summaries = await GetMarketSummaries();
				return summaries.Select(m => new MarketSummaryDto
				{
					BaseCurrrency = _currencyManager.Get(m.MarketName.Substring(0, m.MarketName.IndexOf('-'))),
					MarketCurrency = _currencyManager.Get(m.MarketName.Substring(m.MarketName.IndexOf('-') + 1)),
					Market = "Bittrex",
					Volume = m.BaseVolume,
					Last = m.Last,
					LastUpdated = m.TimeStamp
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
		private async Task<List<BittrexMarketSummaryDto>> GetMarketSummaries()
		{
			using (var response = await _httpClient.GetAsync(new Uri("getmarketsummaries", UriKind.Relative)))
			{
				var summaries = JsonConvert.DeserializeObject<BittrexMarketSummariesDto>(await response.Content.ReadAsStringAsync(), _serializerSettings);
				return summaries.Result;
			}
		}
	}
}
