using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.Logging;

namespace CoinBot.Core
{
    public class CurrencyManager
    {
        private readonly BufferBlock<Func<Task>> _bufferBlock;

        /// <summary>
        ///     The <see cref="ICoinClient" />s.
        /// </summary>
        private readonly IEnumerable<ICoinClient> _coinClients;

        /// <summary>
        ///     The <see cref="ILogger" />.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        ///     The <see cref="TimeSpan" />.
        /// </summary>
        private readonly TimeSpan _tickInterval;

        /// <summary>
        ///     The <see cref="Currency" /> list.
        /// </summary>
        private IReadOnlyCollection<Currency> _coinInfoCollection = Array.Empty<Currency>();

        /// <summary>
        ///     The <see cref="IGlobalInfo" />.
        /// </summary>
        private IGlobalInfo _globalInfo;

        /// <summary>
        ///     The <see cref="Timer" />.
        /// </summary>
        private Timer _timer;

        public CurrencyManager(ILogger logger, IEnumerable<ICoinClient> coinClients)
        {
            this._coinClients = coinClients;
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._tickInterval = TimeSpan.FromSeconds(value: 10);
            this._bufferBlock = CreateBufferBlock();
        }

        private static BufferBlock<Func<Task>> CreateBufferBlock()
        {
            BufferBlock<Func<Task>> bufferBlock = new BufferBlock<Func<Task>>();

            // link the buffer block to an action block to process the actions submitted to the buffer.
            // restrict the number of parallel tasks executing to 1, and only allow 1 messages per task to prevent
            // tasks submitted here from consuming all the available CPU time.
            bufferBlock.LinkTo(new ActionBlock<Func<Task>>(action: action => action(),
                                                           new ExecutionDataflowBlockOptions {MaxDegreeOfParallelism = 1, MaxMessagesPerTask = 1, BoundedCapacity = 1}));

            return bufferBlock;
        }

        private async Task TickAsync()
        {
            try
            {
                await Task.WhenAll(this.UpdateCoinsAsync(), this.UpdateGlobalInfoAsync());
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
            this._timer = new Timer(this.QueueTick, state: null, TimeSpan.FromSeconds(value: 0), Timeout.InfiniteTimeSpan);
        }

        public void QueueTick(object state)
        {
            this._bufferBlock.Post(this.TickAsync);
        }

        public void Stop()
        {
            // stop the timer
            this._timer.Dispose();
            this._timer = null;
        }

        public IGlobalInfo GetGlobalInfo()
        {
            return this._globalInfo;
        }

        public Currency Get(string nameOrSymbol)
        {
            return this.GetCoinBySymbol(nameOrSymbol) ?? this.GetCoinByName(nameOrSymbol);
        }

        private Currency GetCoinBySymbol(string symbol)
        {
            //this._lock.EnterReadLock();
            return this._coinInfoCollection.FirstOrDefault(predicate: c => string.Compare(c.Symbol, symbol, StringComparison.OrdinalIgnoreCase) == 0);
        }

        private Currency GetCoinByName(string name)
        {
            //this._lock.EnterReadLock();
            return this._coinInfoCollection.FirstOrDefault(predicate: c => string.Compare(c.Name, name, StringComparison.OrdinalIgnoreCase) == 0);
        }

        public IEnumerable<Currency> Get(Func<Currency, bool> predicate)
        {
            //this._lock.EnterReadLock();
            return this._coinInfoCollection.Where(predicate);
        }

        private async Task UpdateCoinsAsync()
        {
            ICoinClient client = this._coinClients.First();

            //this._lock.EnterWriteLock();
            List<ICoinInfo> coinInfos = (await client.GetCoinInfoAsync()).ToList();

            List<Currency> currencies = new List<Currency>();
            currencies.AddRange(new[] {new Currency {Symbol = "EUR", Name = "Euro"}, new Currency {Symbol = "USD", Name = "United States dollar"}});

            currencies.AddRange(coinInfos.Select(selector: cryptoInfo =>
                                                           {
                                                               Currency currency = new Currency
                                                                                   {
                                                                                       Symbol = cryptoInfo.Symbol, Name = cryptoInfo.Name, ImageUrl = cryptoInfo.ImageUrl
                                                                                   };
                                                               currency.AddDetails(cryptoInfo);

                                                               return currency;
                                                           }));

            this._coinInfoCollection = new ReadOnlyCollection<Currency>(currencies);
        }

        private async Task UpdateGlobalInfoAsync()
        {
            ICoinClient client = this._coinClients.First();

            this._globalInfo = await client.GetGlobalInfoAsync();
        }
    }
}