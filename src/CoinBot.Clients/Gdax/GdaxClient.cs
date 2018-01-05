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
			this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
		    this._currencyManager = currencyManager ?? throw new ArgumentNullException(nameof(currencyManager));

		    this._httpClient = new HttpClient
			{
				BaseAddress = this._endpoint
			};
		    this._httpClient.DefaultRequestHeaders.Add("User-Agent", "CoinBot");

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
				List<GdaxProduct> products = await this.GetProducts();
				List<GdaxTicker> tickers = new List<GdaxTicker>();

			    foreach (GdaxProduct product in products)
			    {
			        tickers.Add(await this.GetTicker(product.Id));
			    }

			    return tickers.Select(t => new MarketSummaryDto
				{
					BaseCurrrency = this._currencyManager.Get(t.ProductId.Substring(0, t.ProductId.IndexOf('-'))),
					MarketCurrency = this._currencyManager.Get(t.ProductId.Substring(t.ProductId.IndexOf('-') + 1)),
					Market = "GDAX",
					Volume = t.Volume,
					Last = t.Price,
					LastUpdated = t.Time,
				}).ToList();
			}
			catch (Exception e)
			{
				this._logger.LogError(new EventId(e.HResult), e, e.Message);
				throw;
			}
		}

		/// <summary>
		/// Get the products.
		/// </summary>
		/// <returns></returns>
		private async Task<GdaxTicker> GetTicker(string productId)
		{
			using (HttpResponseMessage response = await this._httpClient.GetAsync(new Uri($"products/{productId}/ticker", UriKind.Relative)))
			{
				GdaxTicker ticker = JsonConvert.DeserializeObject<GdaxTicker>(await response.Content.ReadAsStringAsync(), this._serializerSettings);
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
			using (HttpResponseMessage response = await this._httpClient.GetAsync(new Uri("products/", UriKind.Relative)))
			{
				return JsonConvert.DeserializeObject<List<GdaxProduct>>(await response.Content.ReadAsStringAsync(), this._serializerSettings);
			}
		}
	}
}
