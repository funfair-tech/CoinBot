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
        private TimeSpan _tickInterval;
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;
        private readonly DataContractJsonSerializer _serializer;
        private readonly DataContractJsonSerializer _globalSerializer;
        private List<ICoin> _coins;
        private IGlobalInfo _globalInfo;
        private readonly ReaderWriterLockSlim _readerWriterLock;

        public CoinMarketCap(ILogger logger)
        {
            this._logger = logger;
            this._httpClient = new HttpClient();
            this._coins = new List<ICoin>();
            this._serializer = new DataContractJsonSerializer(typeof(List<CoinMarketCapCoin>));
            this._globalSerializer = new DataContractJsonSerializer(typeof(CoinMarketCapGlobalInfo));
            this._tickInterval = TimeSpan.FromSeconds(10);
            this._readerWriterLock = new ReaderWriterLockSlim();
        }

        public IGlobalInfo GetGlobalInfo()
        {
            return this._globalInfo;
        }

        public ICoin Get(string nameOrSymbol)
        {
            return this.GetCoinBySymbol(nameOrSymbol) ?? this.GetCoinByName(nameOrSymbol);
        }

        public ICoin GetCoinBySymbol(string symbol)
        {
            this._readerWriterLock.EnterReadLock();
            try
            {
                return this._coins.FirstOrDefault(c => string.Compare(c.Symbol, symbol, StringComparison.OrdinalIgnoreCase) == 0);
            }
            finally
            {
                this._readerWriterLock.ExitReadLock();
            }
        }

        public ICoin GetCoinByName(string name)
        {
            this._readerWriterLock.EnterReadLock();
            try
            {
                return this._coins.FirstOrDefault(c => string.Compare(c.Name, name, StringComparison.OrdinalIgnoreCase) == 0);
            }
            finally
            {
                this._readerWriterLock.ExitReadLock();
            }
        }

        public List<ICoin> GetTop100()
        {
           this._readerWriterLock.EnterReadLock();
           try
           {
               return this._coins.Where(x => x.Rank <= 100).OrderByDescending(x => Convert.ToDouble(x.DayChange)).ToList();
           } 
           finally 
           {
               this._readerWriterLock.ExitReadLock();
           }
        }

        private async Task UpdateCoins()
        {
            // get the list of coin info from coinmarketcap
            Task<Stream> streamTask = _httpClient.GetStreamAsync("https://api.coinmarketcap.com/v1/ticker/?convert=ETH&limit=1000");
            List<CoinMarketCapCoin> coinMarketCapCoins = _serializer.ReadObject(await streamTask) as List<CoinMarketCapCoin>;

            this._readerWriterLock.EnterWriteLock();
            try
            {
                // update our local list
                this._coins = coinMarketCapCoins.Select(c => (ICoin)c).ToList();
            }
            finally
            {
                this._readerWriterLock.ExitWriteLock();
            }
        }

        private async Task UpdateGlobalInfo()
        {
            // update the global info from coinmarketcap
            Task<Stream> streamTask = _httpClient.GetStreamAsync("https://api.coinmarketcap.com/v1/global/");
            CoinMarketCapGlobalInfo coinMarketCapGlobalInfo = _globalSerializer.ReadObject(await streamTask) as CoinMarketCapGlobalInfo;

            this._readerWriterLock.EnterWriteLock();
            try
            {
                // update our local list
                this._globalInfo = coinMarketCapGlobalInfo;
            }
            finally
            {
                this._readerWriterLock.ExitWriteLock();
            }
        }

        private async Task Tick()
        {
            try
            {
                await Task.WhenAll(UpdateCoins(), UpdateGlobalInfo());
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
