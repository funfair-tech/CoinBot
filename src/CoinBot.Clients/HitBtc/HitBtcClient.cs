using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CoinBot.Core;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CoinBot.Clients.HitBtc
{
	public class HitBtcClient : IMarketClient
	{
		/// <summary>
		/// The <see cref="CurrencyManager"/>.
		/// </summary>
		private readonly CurrencyManager _currencyManager;

		public string Name => "HitBtc";

		/// <summary>
		/// The <see cref="Uri"/> of the CoinMarketCap endpoint.
		/// </summary>
		private readonly Uri _endpoint = new Uri("https://api.hitbtc.com/api/2/public/", UriKind.Absolute);

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

		public HitBtcClient(ILogger logger, CurrencyManager currencyManager)
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
					var ex = args.ErrorContext.Error.GetBaseException();
					this._logger.LogError(new EventId(args.ErrorContext.Error.HResult), ex, ex.Message);
				}
			};
		}

		/// <inheritdoc/>
		public async Task<IReadOnlyCollection<MarketSummaryDto>> Get()
		{
			try
			{
				List<HitBtcTicker> tickers = await this.GetTicker();

				return tickers.Select(t => new MarketSummaryDto
				{
					BaseCurrrency = this._currencyManager.Get(t.BaseCurrency),
					MarketCurrency = this._currencyManager.Get(t.QuoteCurrency),
					Market = "HitBTC",
					Volume = t.Volume,
					Last = t.Last,
					LastUpdated = t.Timestamp,
				}).ToList();
			}
			catch (Exception e)
			{
				this._logger.LogError(new EventId(e.HResult), e, e.Message);
				throw;
			}
		}

		/// <summary>
		/// Get the tickers.
		/// </summary>
		/// <returns></returns>
		private async Task<List<HitBtcTicker>> GetTicker()
		{
			List<HitBtcSymbol> symbols;
			using (var response = await this._httpClient.GetAsync(new Uri("symbol", UriKind.Relative)))
				symbols = JsonConvert.DeserializeObject<List<HitBtcSymbol>>(await response.Content.ReadAsStringAsync(), this._serializerSettings);

			using (var response = await this._httpClient.GetAsync(new Uri("ticker", UriKind.Relative)))
			{
				List<HitBtcTicker> tickers = JsonConvert.DeserializeObject<List<HitBtcTicker>>(await response.Content.ReadAsStringAsync(), this._serializerSettings);
				foreach (var ticker in tickers)
				{
					var symbol = symbols.FirstOrDefault(s => s.Id == ticker.Symbol);
					if (symbol == null)
						continue;

					ticker.BaseCurrency = symbol.BaseCurrency;
					ticker.QuoteCurrency = symbol.QuoteCurrency;
				}
				return tickers
					.Where(t => !string.IsNullOrEmpty(t.BaseCurrency) && !string.IsNullOrEmpty(t.QuoteCurrency))
					.ToList();
			}
		}
	}
}
