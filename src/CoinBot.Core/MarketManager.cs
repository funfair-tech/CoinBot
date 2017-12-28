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
		/// The <see cref="IMarketClient"/>s.
		/// </summary>
		private readonly List<IMarketClient> _clients;

		private readonly IReadOnlyDictionary<string, Exchange> _exchanges;

		/// <summary>
		/// The <see cref="ILogger"/>.
		/// </summary>
		private readonly ILogger _logger;

		/// <summary>
		/// The tick interval <see cref="TimeSpan"/>.
		/// </summary>
		private readonly TimeSpan _tickInterval;

		/// <summary>
		/// The <see cref="Timer"/>.
		/// </summary>
		private Timer _timer;

		public MarketManager(IOptions<MarketManagerSettings> settings, IEnumerable<IMarketClient> clients, ILogger logger)
		{
			_clients = clients.ToList() ?? throw new ArgumentNullException(nameof(clients));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_tickInterval = TimeSpan.FromMinutes(settings.Value.RefreshInterval);
			_exchanges = new ReadOnlyDictionary<string, Exchange>(_clients.ToDictionary(client => client.Name, client => new Exchange
			{
				Lock = new ReaderWriterLockSlim()
			}));
		}

		public IEnumerable<MarketSummaryDto> Get(Currency currency)
		{
			var results = new List<MarketSummaryDto>();
			foreach (var row in _exchanges)
			{
				var name = row.Key;
				var exchange = row.Value;

				// Enter read lock with a timeout of 3 seconds to continue to the next exchange.
				if (!exchange.Lock.TryEnterReadLock(TimeSpan.FromSeconds(3)))
				{
					_logger.LogWarning(0, $"The '{name}' exchange was locked for more than 3 seconds.");
					continue;
				}

				try
				{
					if (exchange.Markets == null)
						continue;

					var markets = exchange.Markets.Where(m =>
					{
						// TODO FIX EMPTY CURRENCIES
						if (m.BaseCurrrency == null || m.MarketCurrency == null)
							return false;

						if (m.BaseCurrrency?.Symbol.Equals(currency.Symbol, StringComparison.OrdinalIgnoreCase) != false ||
						    m.MarketCurrency?.Symbol.Equals(currency.Symbol, StringComparison.OrdinalIgnoreCase) != false)
							return true;
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
			var results = new List<MarketSummaryDto>();
			foreach (var row in _exchanges)
			{
				var name = row.Key;
				var exchange = row.Value;

				// Enter read lock with a timeout of 3 seconds to continue to the next exchange.
				if (!exchange.Lock.TryEnterReadLock(TimeSpan.FromSeconds(3)))
				{
					_logger.LogWarning(0, $"The '{name}' exchange was locked for more than 3 seconds.");
					continue;
				}

				try
				{
					if (exchange.Markets == null)
						continue;
					
					var markets = exchange.Markets.Where(m =>
					{
						// TODO FIX EMPTY CURRENCIES
						if (m.BaseCurrrency == null || m.MarketCurrency == null)
							return false;

						if ((m.BaseCurrrency?.Symbol.Equals(currency1.Symbol, StringComparison.OrdinalIgnoreCase) != false &&
						     m.MarketCurrency?.Symbol.Equals(currency2.Symbol, StringComparison.OrdinalIgnoreCase) != false) ||
						    (m.BaseCurrrency?.Symbol.Equals(currency2.Symbol, StringComparison.OrdinalIgnoreCase) != false &&
						     m.MarketCurrency?.Symbol.Equals(currency1.Symbol, StringComparison.OrdinalIgnoreCase) != false))
							return true;
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
			_timer = new Timer(
				async state => await Tick(),
				null,
				TimeSpan.FromSeconds(0),
				Timeout.InfiniteTimeSpan);
		}

		public void Stop()
		{
			// stop the timer
			_timer.Dispose();
			_timer = null;
		}

		private Task Tick()
		{
			try
			{
				Update();
			}
			catch (Exception e)
			{
				_logger.LogError(new EventId(e.HResult), e, e.Message);
			}
			finally
			{
				// and reset the timer
				_timer.Change(_tickInterval, TimeSpan.Zero);
			}
			return Task.CompletedTask;
		}

		/// <summary>
		/// Updates the markets.
		/// </summary>
		/// <returns></returns>
		private void Update()
		{
			Parallel.ForEach(_clients, client =>
			{
				if (_exchanges.TryGetValue(client.Name, out var exchange))
				{
					_logger.LogInformation($"Start updating exchange '{client.Name}'.");
					var watch = new Stopwatch();
					watch.Start();

					IReadOnlyCollection<MarketSummaryDto> markets;
					try
					{
						markets = client.Get().Result;
					}
					catch (Exception e)
					{
						_logger.LogError(0, e, $"An error occurred while fetching results from the exchange '{client.Name}'.");
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
						_logger.LogInformation($"Finished updating exchange '{client.Name}' in {watch.ElapsedMilliseconds}ms.");
					}
					finally
					{
						exchange.Lock.ExitWriteLock();
					}
				}
				else
					_logger.LogWarning(0, $"Couldn't find exchange {client.Name}.");
			});
		}

		private class Exchange
		{
			public ReaderWriterLockSlim Lock;
			public IReadOnlyCollection<MarketSummaryDto> Markets;
		}
	}
}
