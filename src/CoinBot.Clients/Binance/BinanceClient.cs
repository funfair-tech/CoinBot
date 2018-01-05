using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CoinBot.Core;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CoinBot.Clients.Binance
{
	public class BinanceClient : IMarketClient
	{
		/// <summary>
		/// The <see cref="CurrencyManager"/>.
		/// </summary>
		private readonly CurrencyManager _currencyManager;

		/// <summary>
		/// The Exchange name.
		/// </summary>
		public string Name => "Binance";

		/// <summary>
		/// The <see cref="Uri"/> of the CoinMarketCap endpoint.
		/// </summary>
		private readonly Uri _endpoint = new Uri("https://www.binance.com/exchange/public/", UriKind.Absolute);

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

		public BinanceClient(ILogger logger, CurrencyManager currencyManager)
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
				var products = await GetProducts();
				return products.Select(p => new MarketSummaryDto
				{
					BaseCurrrency = _currencyManager.Get(p.BaseAsset),
					MarketCurrency = _currencyManager.Get(p.QuoteAsset),
					Market = "Binance",
					Volume = p.Volume,
					Last = p.PrevClose,
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
		private async Task<List<BinanceProduct>> GetProducts()
		{
			using (var response = await _httpClient.GetAsync(new Uri("product", UriKind.Relative)))
			{
				var json = await response.Content.ReadAsStringAsync();
				var jObject = JObject.Parse(json);
				var products = JsonConvert.DeserializeObject<List<BinanceProduct>>(jObject["data"].ToString(), _serializerSettings);
				return products;
			}
		}
	}
}
