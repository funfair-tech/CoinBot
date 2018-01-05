using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CoinBot.Core;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CoinBot.Clients.Kraken
{
	public class KrakenClient : IMarketClient
	{
		/// <summary>
		/// The <see cref="CurrencyManager"/>.
		/// </summary>
		private readonly CurrencyManager _currencyManager;

		/// <summary>
		/// The Exchange name.
		/// </summary>
		public string Name => "Kraken";

		/// <summary>
		/// The <see cref="Uri"/> of the CoinMarketCap endpoint.
		/// </summary>
		private readonly Uri _endpoint = new Uri("https://api.kraken.com/0/public/", UriKind.Absolute);

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

		public KrakenClient(ILogger logger, CurrencyManager currencyManager)
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
				var assets = await GetAssets();
				var pairs = await GetPairs();
				var tickers = new List<KrakenTicker>();
				foreach (var pair in pairs)
				{
					// todo: can't get kraken details on these markets
					if (pair.PairId.EndsWith(".d"))
						continue;

					tickers.Add(await GetTicker(pair));
				}
				
				return tickers.Select(m =>
				{
					var baseCurrency = assets.Find(a => a.Id.Equals(m.BaseCurrency)).Altname;
					var quoteCurrency = assets.Find(a => a.Id.Equals(m.QuoteCurrency)).Altname;

					// Workaround for kraken
					if (baseCurrency.Equals("xbt", StringComparison.OrdinalIgnoreCase))
						baseCurrency = "btc";
					if (quoteCurrency.Equals("xbt", StringComparison.OrdinalIgnoreCase))
						quoteCurrency = "btc";

					return new MarketSummaryDto
					{
						BaseCurrrency = _currencyManager.Get(baseCurrency),
						MarketCurrency = _currencyManager.Get(quoteCurrency),
						Market = "Kraken",
						Volume = m.Volume[1],
						Last = m.Last[0]
					};
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
		/// Get the ticker.
		/// </summary>
		/// <returns></returns>
		private async Task<List<KrakenAsset>> GetAssets()
		{
			using (var response = await _httpClient.GetAsync(new Uri("Assets", UriKind.Relative)))
			{
				var json = await response.Content.ReadAsStringAsync();
				var jObject = JObject.Parse(json);
				var assets = jObject.GetValue("result").Children().Cast<JProperty>().Select(property =>
				{
					var asset = JsonConvert.DeserializeObject<KrakenAsset>(property.Value.ToString(), _serializerSettings);
					asset.Id = property.Name;
					return asset;
				}).ToList();
				return assets;
			}
		}

		/// <summary>
		/// Get the market summaries.
		/// </summary>
		/// <returns></returns>
		private async Task<List<KrakenPair>> GetPairs()
		{
			using (var response = await _httpClient.GetAsync(new Uri("AssetPairs", UriKind.Relative)))
			{
				var json = await response.Content.ReadAsStringAsync();
				var jResponse = JObject.Parse(json);
				var pairs = jResponse.GetValue("result").Children().Cast<JProperty>().Select(property =>
				{
					var pair = JsonConvert.DeserializeObject<KrakenPair>(property.Value.ToString());
					pair.PairId = property.Name;
					return pair;
				}).ToList();
				return pairs;
			}
		}

		/// <summary>
		/// Get the ticker.
		/// </summary>
		/// <returns></returns>
		private async Task<KrakenTicker> GetTicker(KrakenPair pair)
		{
			using (var response = await _httpClient.GetAsync(new Uri($"Ticker?pair={pair.PairId}", UriKind.Relative)))
			{
				var json = await response.Content.ReadAsStringAsync();
				var jObject = JObject.Parse(json);
				var ticker = JsonConvert.DeserializeObject<KrakenTicker>(jObject["result"][pair.PairId].ToString(), _serializerSettings);
				ticker.BaseCurrency = pair.BaseCurrency;
				ticker.QuoteCurrency = pair.QuoteCurrency;
				return ticker;
			}
		}
	}
}
