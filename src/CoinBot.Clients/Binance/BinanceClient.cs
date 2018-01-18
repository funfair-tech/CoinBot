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
		private readonly Uri _endpoint = new Uri("https://api.binance.com/api/v1/", UriKind.Absolute);

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
				List<BinanceTickerDto> products = await this.GetProducts();
				return products.Select(p =>
					{
						if (!this.FindCurrencies(p.Symbol, out var baseCur, out var quoteCur))
							return null;

						return new MarketSummaryDto
						{
							BaseCurrrency = baseCur,
							MarketCurrency = quoteCur,
							Market = "Binance",
							Volume = p.Volume,
							Last = p.LastPrice,
						};
					}).Where(p => p != null)
					.ToList();
			}
			catch (Exception e)
			{
				EventId eventId = new EventId(e.HResult);
				this._logger.LogError(eventId, e, e.Message);
				throw;
			}
		}

		/// <summary>
		/// Get the market summaries.
		/// </summary>
		/// <returns></returns>
		private async Task<List<BinanceTickerDto>> GetProducts()
		{
			using (HttpResponseMessage response = await this._httpClient.GetAsync(new Uri("ticker/24hr", UriKind.Relative)))
			{
				string json = await response.Content.ReadAsStringAsync();
				List<BinanceTickerDto> products = JsonConvert.DeserializeObject<List<BinanceTickerDto>>(json, this._serializerSettings);
				return products;
			}
		}

		/// <summary>
		/// Find the <paramref name="baseCur"/> and <paramref name="quoteCur"/> based on the given <paramref name="symbols"/>.
		/// </summary>
		/// <param name="symbols">The symbols.</param>
		/// <param name="baseCur"></param>
		/// <param name="quoteCur"></param>
		/// <returns></returns>
		private bool FindCurrencies(string symbols, out Currency baseCur, out Currency quoteCur)
		{
			baseCur = null;
			quoteCur = null;

			int i = 3;
			while (true)
			{
				if (i > symbols.Length)
					return false;

				baseCur = this._currencyManager.Get(symbols.Substring(0, i));
				quoteCur = this._currencyManager.Get(symbols.Substring(i));

				if (baseCur != null && quoteCur != null)
					return true;

				i++;
			}
		}
	}
}
