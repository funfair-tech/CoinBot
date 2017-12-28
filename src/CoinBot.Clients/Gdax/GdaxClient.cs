using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CoinBot.Core;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CoinBot.Clients.Gdax
{
	public class GdaxClient : IMarketClient
	{
		/// <summary>
		/// The <see cref="CurrencyManager"/>.
		/// </summary>
		private readonly CurrencyManager _currencyManager;

		public string Name => "GDAX";

		/// <summary>
		/// The <see cref="Uri"/> of the CoinMarketCap endpoint.
		/// </summary>
		private readonly Uri _endpoint = new Uri("https://api.gdax.com/", UriKind.Absolute);

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

		public GdaxClient(ILogger logger, CurrencyManager currencyManager)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_currencyManager = currencyManager ?? throw new ArgumentNullException(nameof(currencyManager));

			_httpClient = new HttpClient
			{
				BaseAddress = _endpoint
			};
			_httpClient.DefaultRequestHeaders.Add("User-Agent", "CoinBot");

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
				var tickers = new List<GdaxTicker>();
				foreach (var product in products)
					tickers.Add(await GetTicker(product.Id));

				return tickers.Select(t => new MarketSummaryDto
				{
					BaseCurrrency = _currencyManager.Get(t.ProductId.Substring(0, t.ProductId.IndexOf('-'))),
					MarketCurrency = _currencyManager.Get(t.ProductId.Substring(t.ProductId.IndexOf('-') + 1)),
					Market = "GDAX",
					Volume = t.Volume,
					Last = t.Price,
					LastUpdated = t.Time,
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
		/// Get the products.
		/// </summary>
		/// <returns></returns>
		private async Task<GdaxTicker> GetTicker(string productId)
		{
			using (var response = await _httpClient.GetAsync(new Uri($"products/{productId}/ticker", UriKind.Relative)))
			{
				var ticker = JsonConvert.DeserializeObject<GdaxTicker>(await response.Content.ReadAsStringAsync(), _serializerSettings);
				ticker.ProductId = productId;
				return ticker;
			}
		}

		/// <summary>
		/// Get the products.
		/// </summary>
		/// <returns></returns>
		private async Task<List<GdaxProduct>> GetProducts()
		{
			using (var response = await _httpClient.GetAsync(new Uri("products/", UriKind.Relative)))
			{
				var json = await response.Content.ReadAsStringAsync();
				var products = JsonConvert.DeserializeObject<List<GdaxProduct>>(json, _serializerSettings);
				return products;
			}
		}
	}
}
