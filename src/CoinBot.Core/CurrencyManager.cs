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
		//private readonly ReaderWriterLockSlim _lock;

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
		    this._coinClients = coinClients;
		    this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
		    this._tickInterval = TimeSpan.FromSeconds(10);
		    //this._lock = new ReaderWriterLockSlim();
		}

		private async Task Tick()
		{
			try
			{
				await Task.WhenAll(this.UpdateCoins(), this.UpdateGlobalInfo());
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

		public IGlobalInfo GetGlobalInfo()
		{
			return this._globalInfo;
		}

		public Currency Get(string nameOrSymbol) => this.GetCoinBySymbol(nameOrSymbol) ?? this.GetCoinByName(nameOrSymbol);

		private Currency GetCoinBySymbol(string symbol)
		{
		    //this._lock.EnterReadLock();
			try
			{
				return this._coinInfoCollection.FirstOrDefault(c => string.Compare(c.Symbol, symbol, StringComparison.OrdinalIgnoreCase) == 0);
			}
			finally
			{
			  //  this._lock.ExitReadLock();
			}
		}

		private Currency GetCoinByName(string name)
		{
		    //this._lock.EnterReadLock();
			try
			{
				return this._coinInfoCollection.FirstOrDefault(c => string.Compare(c.Name, name, StringComparison.OrdinalIgnoreCase) == 0);
			}
			finally
			{
			    //this._lock.ExitReadLock();
			}
		}

		public IEnumerable<Currency> Get(Func<Currency, bool> predicate)
		{
		    //this._lock.EnterReadLock();
			try
			{
				return this._coinInfoCollection.Where(predicate);
			}
			finally
			{
			    //this._lock.ExitReadLock();
			}
		}

		private Task UpdateCoins()
		{
			ICoinClient client = this._coinClients.First();

		    //this._lock.EnterWriteLock();
			try
			{
				List<ICoinInfo> coinInfos = client.GetCoinInfo().Result.ToList();

				List<Currency> currencies = new List<Currency>();
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
					Currency currency = new Currency
					{
						Symbol = cryptoInfo.Symbol,
						Name = cryptoInfo.Name,
						ImageUrl = cryptoInfo.ImageUrl
					};
					currency.AddDetails(cryptoInfo);
					return currency;
				}));

			    this._coinInfoCollection = new ReadOnlyCollection<Currency>(currencies);

				return Task.CompletedTask;
			}
			finally
			{
			    //this._lock.ExitWriteLock();
			}
		}

		private Task UpdateGlobalInfo()
		{
			ICoinClient client = this._coinClients.First();

		    //this._lock.EnterWriteLock();
			try
			{
			    this._globalInfo = client.GetGlobalInfo().Result;
				return Task.CompletedTask;
			}
			finally
			{
			    //this._lock.ExitWriteLock();
			}
		}
	}
}
