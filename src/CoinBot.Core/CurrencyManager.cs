using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace CoinBot.Core
{
	public class CurrencyManager
	{
		/// <summary>
		/// The <see cref="ICoinClient"/>s.
		/// </summary>
		private readonly IEnumerable<ICoinClient> _coinClients;

		/// <summary>
		/// The <see cref="TimeSpan"/>.
		/// </summary>
		private readonly TimeSpan _tickInterval;

		/// <summary>
		/// The <see cref="Timer"/>.
		/// </summary>
		private Timer _timer;

		/// <summary>
		/// The <see cref="ILogger"/>.
		/// </summary>
		private readonly ILogger _logger;

		/// <summary>
		/// The <see cref="ReaderWriterLockSlim"/>.
		/// </summary>
		private readonly ReaderWriterLockSlim _lock;

		/// <summary>
		/// The <see cref="Currency"/> list.
		/// </summary>
		private IReadOnlyCollection<Currency> _coinInfoCollection = new Currency[0];

		/// <summary>
		/// The <see cref="IGlobalInfo"/>.
		/// </summary>
		private IGlobalInfo _globalInfo;

		public CurrencyManager(ILogger logger, IEnumerable<ICoinClient> coinClients)
		{
			_coinClients = coinClients;
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_tickInterval = TimeSpan.FromSeconds(10);
			_lock = new ReaderWriterLockSlim();
		}

		private async Task Tick()
		{
			try
			{
				await Task.WhenAll(UpdateCoins(), UpdateGlobalInfo());
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
		}

		public void Start()
		{
			// start a timer to fire the tickFunction
			_timer = new Timer(
				async (state) => await Tick(),
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

		public IGlobalInfo GetGlobalInfo()
		{
			return _globalInfo;
		}

		public Currency Get(string nameOrSymbol) => GetCoinBySymbol(nameOrSymbol) ?? GetCoinByName(nameOrSymbol);

		private Currency GetCoinBySymbol(string symbol)
		{
			_lock.EnterReadLock();
			try
			{
				return _coinInfoCollection.FirstOrDefault(c => string.Compare(c.Symbol, symbol, StringComparison.OrdinalIgnoreCase) == 0);
			}
			finally
			{
				_lock.ExitReadLock();
			}
		}

		private Currency GetCoinByName(string name)
		{
			_lock.EnterReadLock();
			try
			{
				return _coinInfoCollection.FirstOrDefault(c => string.Compare(c.Name, name, StringComparison.OrdinalIgnoreCase) == 0);
			}
			finally
			{
				_lock.ExitReadLock();
			}
		}

		public IEnumerable<Currency> Get(Func<Currency, bool> predicate)
		{
			_lock.EnterReadLock();
			try
			{
				return _coinInfoCollection.Where(predicate);
			}
			finally
			{
				_lock.ExitReadLock();
			}
		}

		private Task UpdateCoins()
		{
			var client = _coinClients.First();

			_lock.EnterWriteLock();
			try
			{
				var coinInfos = client.GetCoinInfo().Result.ToList();

				var currencies = new List<Currency>();
				currencies.AddRange(new[]
				{
					new Currency
					{
						Symbol = "EUR",
						Name = "Euro"
					},
					new Currency
					{
						Symbol = "USD",
						Name = "United States dollar"
					}
				});

				currencies.AddRange(coinInfos.Select(cryptoInfo =>
				{
					var currency = new Currency
					{
						Symbol = cryptoInfo.Symbol,
						Name = cryptoInfo.Name,
						ImageUrl = cryptoInfo.ImageUrl
					};
					currency.AddDetails(cryptoInfo);
					return currency;
				}));

				_coinInfoCollection = new ReadOnlyCollection<Currency>(currencies);

				return Task.CompletedTask;
			}
			finally
			{
				_lock.ExitWriteLock();
			}
		}

		private Task UpdateGlobalInfo()
		{
			var client = _coinClients.First();

			_lock.EnterWriteLock();
			try
			{
				_globalInfo = client.GetGlobalInfo().Result;
				return Task.CompletedTask;
			}
			finally
			{
				_lock.ExitWriteLock();
			}
		}
	}
}
