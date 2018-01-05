using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using CoinBot.Core;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CoinBot.Clients.CoinMarketCap
{
	public class CoinMarketCapClient : ICoinClient
	{
		/// <summary>
		/// The <see cref="Uri"/> of the CoinMarketCap endpoint.
		/// </summary>
		private readonly Uri _endpoint = new Uri("https://api.coinmarketcap.com/v1/", UriKind.Absolute);

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

		public CoinMarketCapClient(ILogger logger)
		{
			_logger = logger;
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
		/// <summary>
		/// get the list of coin info from coinmarketcap
		/// </summary>
		/// <returns></returns>
		public async Task<IReadOnlyCollection<ICoinInfo>> GetCoinInfo()
		{
			try
			{
				using (var response = await _httpClient.GetAsync(new Uri("ticker/?convert=ETH&limit=1000", UriKind.Relative)))
				{
					var coins = JsonConvert.DeserializeObject<List<CoinMarketCapCoin>>(await response.Content.ReadAsStringAsync(), _serializerSettings);
					return coins;
				}
			}
			catch (Exception e)
			{
				var eventId = new EventId(e.HResult);
				_logger.LogError(eventId, e, e.Message);
				throw;
			}
		}

		/// <inheritdoc/>
		/// <summary>
		/// update the global info from coinmarketcap
		/// </summary>
		/// <returns></returns>
		public async Task<IGlobalInfo> GetGlobalInfo()
		{
			try
			{
				using (var response = await _httpClient.GetAsync(new Uri("global/", UriKind.Relative)))
				{
					var globalInfo = JsonConvert.DeserializeObject<CoinMarketCapGlobalInfo>(await response.Content.ReadAsStringAsync(), _serializerSettings);
					return globalInfo;
				}
			}
			catch (Exception e)
			{
				var eventId = new EventId(e.HResult);
				_logger.LogError(eventId, e, e.Message);
				throw;
			}
		}
	}
}
