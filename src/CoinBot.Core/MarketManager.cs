using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CoinBot.Core
{
    public class MarketManager
    {
        /// <summary>
        ///     The <see cref="IMarketClient" />s.
        /// </summary>
        private readonly List<IMarketClient> _clients;

        private readonly IReadOnlyDictionary<string, Exchange> _exchanges;

        /// <summary>
        ///     The <see cref="ILogger" />.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        ///     The tick interval <see cref="TimeSpan" />.
        /// </summary>
        private readonly TimeSpan _tickInterval;

        /// <summary>
        ///     The <see cref="Timer" />.
        /// </summary>
        private Timer _timer;

        public MarketManager(IOptions<MarketManagerSettings> settings, IEnumerable<IMarketClient> clients, ILogger logger)
        {
            this._clients = clients.ToList() ?? throw new ArgumentNullException(nameof(clients));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._tickInterval = TimeSpan.FromMinutes(settings.Value.RefreshInterval);
            this._exchanges = new ReadOnlyDictionary<string, Exchange>(
                this._clients.ToDictionary(keySelector: client => client.Name, elementSelector: client => new Exchange {Lock = new ReaderWriterLockSlim()}));
        }

        public IEnumerable<MarketSummaryDto> Get(Currency currency)
        {
            List<MarketSummaryDto> results = new List<MarketSummaryDto>();

            foreach (KeyValuePair<string, Exchange> row in this._exchanges)
            {
                string name = row.Key;
                Exchange exchange = row.Value;

                // Enter read lock with a timeout of 3 seconds to continue to the next exchange.
                if (!exchange.Lock.TryEnterReadLock(TimeSpan.FromSeconds(value: 3)))
                {
                    this._logger.LogWarning(eventId: 0, $"The '{name}' exchange was locked for more than 3 seconds.");

                    continue;
                }

                try
                {
                    if (exchange.Markets == null)
                    {
                        continue;
                    }

                    IEnumerable<MarketSummaryDto> markets = exchange.Markets.Where(predicate: m =>
                                                                                              {
                                                                                                  // TODO FIX EMPTY CURRENCIES
                                                                                                  if (m.BaseCurrrency == null || m.MarketCurrency == null)
                                                                                                  {
                                                                                                      return false;
                                                                                                  }

                                                                                                  if (m.BaseCurrrency?.Symbol.Equals(currency.Symbol,
                                                                                                                                     StringComparison.OrdinalIgnoreCase) != false ||
                                                                                                      m.MarketCurrency?.Symbol.Equals(currency.Symbol,
                                                                                                                                      StringComparison.OrdinalIgnoreCase) != false)
                                                                                                  {
                                                                                                      return true;
                                                                                                  }

                                                                                                  return false;
                                                                                              });
                    results.AddRange(markets);
                }
                finally
                {
                    exchange.Lock.ExitReadLock();
                }
            }

            return results;
        }

        public IEnumerable<MarketSummaryDto> GetPair(Currency currency1, Currency currency2)
        {
            List<MarketSummaryDto> results = new List<MarketSummaryDto>();

            foreach (KeyValuePair<string, Exchange> row in this._exchanges)
            {
                string name = row.Key;
                Exchange exchange = row.Value;

                // Enter read lock with a timeout of 3 seconds to continue to the next exchange.
                if (!exchange.Lock.TryEnterReadLock(TimeSpan.FromSeconds(value: 3)))
                {
                    this._logger.LogWarning(eventId: 0, $"The '{name}' exchange was locked for more than 3 seconds.");

                    continue;
                }

                try
                {
                    if (exchange.Markets == null)
                    {
                        continue;
                    }

                    IEnumerable<MarketSummaryDto> markets = exchange.Markets.Where(predicate: m =>
                                                                                              {
                                                                                                  // TODO FIX EMPTY CURRENCIES
                                                                                                  if (m.BaseCurrrency == null || m.MarketCurrency == null)
                                                                                                  {
                                                                                                      return false;
                                                                                                  }

                                                                                                  if (m.BaseCurrrency?.Symbol.Equals(currency1.Symbol,
                                                                                                                                     StringComparison.OrdinalIgnoreCase) != false &&
                                                                                                      m.MarketCurrency?.Symbol.Equals(currency2.Symbol,
                                                                                                                                      StringComparison.OrdinalIgnoreCase) !=
                                                                                                      false ||
                                                                                                      m.BaseCurrrency?.Symbol.Equals(currency2.Symbol,
                                                                                                                                     StringComparison.OrdinalIgnoreCase) != false &&
                                                                                                      m.MarketCurrency?.Symbol.Equals(currency1.Symbol,
                                                                                                                                      StringComparison.OrdinalIgnoreCase) != false)
                                                                                                  {
                                                                                                      return true;
                                                                                                  }

                                                                                                  return false;
                                                                                              });
                    results.AddRange(markets);
                }
                finally
                {
                    exchange.Lock.ExitReadLock();
                }
            }

            return results;
        }

        public void Start()
        {
            // start a timer to fire the tickFunction
            this._timer = new Timer(callback: async state => await this.TickAsync(), state: null, TimeSpan.FromSeconds(value: 0), Timeout.InfiniteTimeSpan);
        }

        public void Stop()
        {
            // stop the timer
            this._timer.Dispose();
            this._timer = null;
        }

        private async Task TickAsync()
        {
            try
            {
                await this.UpdateAsync();
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

        /// <summary>
        ///     Updates the markets.
        /// </summary>
        /// <returns></returns>
        private Task UpdateAsync()
        {
            return Task.WhenAll(this._clients.Select(this.UpdateOneClientAsync));
        }

        private async Task UpdateOneClientAsync(IMarketClient client)
        {
            if (this._exchanges.TryGetValue(client.Name, out Exchange exchange))
            {
                this._logger.LogInformation($"Start updating exchange '{client.Name}'.");
                Stopwatch watch = new Stopwatch();
                watch.Start();

                IReadOnlyCollection<MarketSummaryDto> markets;

                try
                {
                    markets = await client.GetAsync();
                }
                catch (Exception e)
                {
                    this._logger.LogError(eventId: 0, e, $"An error occurred while fetching results from the exchange '{client.Name}'.");
                    exchange.Lock.EnterWriteLock();

                    try
                    {
                        // Remove out-of-date market summaries
                        exchange.Markets = null;
                    }
                    finally
                    {
                        exchange.Lock.ExitWriteLock();
                    }

                    return;
                }

                // Update market summaries
                exchange.Lock.EnterWriteLock();

                try
                {
                    exchange.Markets = markets;
                    watch.Stop();
                    this._logger.LogInformation($"Finished updating exchange '{client.Name}' in {watch.ElapsedMilliseconds}ms.");
                }
                finally
                {
                    exchange.Lock.ExitWriteLock();
                }
            }
            else
            {
                this._logger.LogWarning(eventId: 0, $"Couldn't find exchange {client.Name}.");
            }
        }

        private class Exchange
        {
            public ReaderWriterLockSlim Lock;
            public IReadOnlyCollection<MarketSummaryDto> Markets;
        }
    }
}