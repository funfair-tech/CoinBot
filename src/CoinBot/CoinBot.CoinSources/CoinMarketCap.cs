using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Threading;
using System.Threading.Tasks;

namespace CoinBot.CoinSources.CoinMarketCap
{
    public class CoinMarketCap : ICoinSource
    {
        public string Name => "CoinMarketCap";
        private Timer _timer;
        private TimeSpan _tickInterval => TimeSpan.FromSeconds(10);
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;
        private readonly DataContractJsonSerializer _serializer;
        private List<ICoin> _coins;

        public CoinMarketCap(ILogger logger)
        {
            this._logger = logger;
            this._httpClient = new HttpClient();
            this._coins = new List<ICoin>();
            this._serializer = new DataContractJsonSerializer(typeof(List<CoinMarketCapCoin>));
        }

        public ICoin GetCoinBySymbol(string symbol)
        {
            return this._coins.FirstOrDefault(c => string.Compare(c.Symbol, symbol, StringComparison.OrdinalIgnoreCase) == 0);
        }

        public ICoin GetCoinByName(string name)
        {
            return this._coins.FirstOrDefault(c => string.Compare(c.Name, name, StringComparison.OrdinalIgnoreCase) == 0);
        }

        private async Task Tick()
        {
            try
            {
                Task<Stream> streamTask = _httpClient.GetStreamAsync("https://api.coinmarketcap.com/v1/ticker/?convert=ETH");
                List<CoinMarketCapCoin> coinMarketCapCoins = _serializer.ReadObject(await streamTask) as List<CoinMarketCapCoin>;

                this._coins = coinMarketCapCoins.Select(c => (ICoin)c).ToList();
            }
            catch (Exception e)
            {
                this._logger.LogError(new EventId(e.HResult), e, e.Message);
            }
            finally
            {
                // and reset the timer
                this._timer.Change(this._tickInterval, TimeSpan.Zero);
            }
        }

        public void Start()
        {
            // start a timer to fire the tickFunction
            this._timer = new Timer(
                async (state) => await this.Tick(),
                null,
                TimeSpan.FromSeconds(0),
                Timeout.InfiniteTimeSpan);
        }

        public void Stop()
        {
            // stop the timer
            this._timer.Dispose();
            this._timer = null;
        }
    }
}
