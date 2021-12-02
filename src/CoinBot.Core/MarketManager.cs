using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CoinBot.Core.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CoinBot.Core;

public sealed class MarketManager : TickingService
{
    private readonly IReadOnlyList<ICoinClient> _coinClients;
    private readonly ICurrencyListUpdater _currencyListUpdater;

    private readonly IReadOnlyDictionary<string, Exchange> _exchanges;

    /// <summary>
    ///     The <see cref="IMarketClient" />s.
    /// </summary>
    private readonly IReadOnlyList<IMarketClient> _marketClients;

    /// <summary>
    ///     Constructor.
    /// </summary>
    /// <param name="settings">Settings.</param>
    /// <param name="coinClients"></param>
    /// <param name="marketClients">Market Clients.</param>
    /// <param name="currencyListUpdater">Currency list updater.</param>
    /// <param name="logger">Logging</param>
    public MarketManager(IOptions<MarketManagerSettings> settings,
                         IEnumerable<ICoinClient> coinClients,
                         IEnumerable<IMarketClient> marketClients,
                         ICurrencyListUpdater currencyListUpdater,
                         ILogger<MarketManager> logger)
        : base(TimeSpan.FromMinutes(settings.Value.RefreshInterval), logger: logger)
    {
        this._currencyListUpdater = currencyListUpdater;
        this._coinClients = coinClients.ToList() ?? throw new ArgumentNullException(nameof(coinClients));
        this._marketClients = marketClients.ToList() ?? throw new ArgumentNullException(nameof(marketClients));
        this._exchanges = new ReadOnlyDictionary<string, Exchange>(this._marketClients.ToDictionary(keySelector: client => client.Name, elementSelector: _ => new Exchange()));
    }

    public IEnumerable<MarketSummaryDto> Get(Currency currency)
    {
        List<MarketSummaryDto> results = new();

        foreach ((string name, Exchange exchange) in this._exchanges)
        {
            // Enter read lock with a timeout of 3 seconds to continue to the next exchange.
            if (!exchange.Lock.TryEnterReadLock(TimeSpan.FromSeconds(value: 3)))
            {
                this.Logger.LogWarning(eventId: 0, $"The '{name}' exchange was locked for more than 3 seconds.");

                continue;
            }

            try
            {
                IEnumerable<MarketSummaryDto> markets = exchange.Markets.Where(predicate: m =>
                                                                                          {
                                                                                              if (m.BaseCurrency.Symbol.Equals(value: currency.Symbol,
                                                                                                      comparisonType: StringComparison.OrdinalIgnoreCase) ||
                                                                                                  m.MarketCurrency.Symbol.Equals(value: currency.Symbol,
                                                                                                      comparisonType: StringComparison.OrdinalIgnoreCase))
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
        List<MarketSummaryDto> results = new();

        foreach ((string name, Exchange exchange) in this._exchanges)
        {
            // Enter read lock with a timeout of 3 seconds to continue to the next exchange.
            if (!exchange.Lock.TryEnterReadLock(TimeSpan.FromSeconds(value: 3)))
            {
                this.Logger.LogWarning(eventId: 0, $"The '{name}' exchange was locked for more than 3 seconds.");

                continue;
            }

            try
            {
                IEnumerable<MarketSummaryDto> markets = exchange.Markets.Where(predicate: m =>
                                                                                          {
                                                                                              if (m.BaseCurrency.Symbol.Equals(value: currency1.Symbol,
                                                                                                      comparisonType: StringComparison.OrdinalIgnoreCase) &&
                                                                                                  m.MarketCurrency.Symbol.Equals(value: currency2.Symbol,
                                                                                                      comparisonType: StringComparison.OrdinalIgnoreCase) ||
                                                                                                  m.BaseCurrency.Symbol.Equals(value: currency2.Symbol,
                                                                                                      comparisonType: StringComparison.OrdinalIgnoreCase) &&
                                                                                                  m.MarketCurrency.Symbol.Equals(value: currency1.Symbol,
                                                                                                      comparisonType: StringComparison.OrdinalIgnoreCase))
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

    protected override async Task TickAsync()
    {
        try
        {
            await this.UpdateAsync();
        }
        catch (Exception e)
        {
            this.Logger.LogError(new(e.HResult), exception: e, message: e.Message);
        }
    }

    /// <summary>
    ///     Updates the markets.
    /// </summary>
    /// <returns></returns>
    private async Task UpdateAsync()
    {
        CoinBuilder builder = new();

        await this.UpdateCoinsAsync(builder);

        await Task.WhenAll(this._marketClients.Select(selector: client => this.UpdateOneClientAsync(client: client, builder: builder)));

        IGlobalInfo? globalInfo = await this.UpdateGlobalInfoAsync();

        this._currencyListUpdater.Update(builder.AllCurrencies(), globalInfo: globalInfo);
    }

    private async Task UpdateCoinsAsync(ICoinBuilder builder)
    {
        this.Logger.LogInformation(message: "Updating All CoinInfos");

        IReadOnlyCollection<ICoinInfo>[] allCoinInfos = await Task.WhenAll(this._coinClients.Select(this.GetCoinInfoAsync));

        var cryptoInfos = allCoinInfos.SelectMany(selector: ci => ci)
                                      .GroupBy(keySelector: c => c.Symbol)
                                      .Select(selector: c => new { Symbol = c.Key, Coins = c.ToArray() });

        foreach (var cryptoInfo in cryptoInfos)
        {
            ICoinInfo name = cryptoInfo.Coins[0];

            Currency currency = builder.Get(symbol: cryptoInfo.Symbol, name: name.Name);

            foreach (ICoinInfo info in cryptoInfo.Coins)
            {
                currency.AddDetails(info);
            }
        }
    }

    private async Task<IReadOnlyCollection<ICoinInfo>> GetCoinInfoAsync(ICoinClient client)
    {
        this.Logger.LogInformation($"Updating {client.GetType().Name} CoinInfo");

        try
        {
            return await client.GetCoinInfoAsync();
        }
        catch (Exception exception)
        {
            this.Logger.LogError(new(exception.HResult), exception: exception, $"Failed to update {client.GetType().Name} CoinInfo: {exception.Message}");

            return Array.Empty<ICoinInfo>();
        }
    }

    private async Task<IGlobalInfo?> UpdateGlobalInfoAsync()
    {
        IGlobalInfo?[] results = await Task.WhenAll(this._coinClients.Select(selector: this.GetGlobalInfoAsync));

        return results.RemoveNulls()
                      .FirstOrDefault();
    }

    private async Task UpdateOneClientAsync(IMarketClient client, ICoinBuilder builder)
    {
        if (this._exchanges.TryGetValue(key: client.Name, out Exchange? exchange))
        {
            this.Logger.LogInformation($"Start updating exchange '{client.Name}'.");
            Stopwatch watch = new();
            watch.Start();

            IReadOnlyCollection<MarketSummaryDto> markets;

            try
            {
                markets = await client.GetAsync(builder);
            }
            catch (Exception e)
            {
                this.Logger.LogError(eventId: 0, exception: e, $"An error occurred while fetching results from the exchange '{client.Name}'.");
                exchange.Lock.EnterWriteLock();

                try
                {
                    // Remove out-of-date market summaries
                    exchange.Markets = Array.Empty<MarketSummaryDto>();
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
                this.Logger.LogInformation($"Finished updating exchange '{client.Name}' in {watch.ElapsedMilliseconds}ms.");
            }
            finally
            {
                exchange.Lock.ExitWriteLock();
            }
        }
        else
        {
            this.Logger.LogWarning(eventId: 0, $"Couldn't find exchange {client.Name}.");
        }
    }

    private async Task<IGlobalInfo?> GetGlobalInfoAsync(ICoinClient client)
    {
        try
        {
            return await client.GetGlobalInfoAsync();
        }
        catch (Exception exception)
        {
            this.Logger.LogError(new(exception.HResult), exception: exception, $"Failed to update {client.GetType().Name} GlobalInfo: {exception.Message}");

            return null;
        }
    }

    private sealed class Exchange
    {
        public Exchange()
        {
            this.Lock = new();
            this.Markets = Array.Empty<MarketSummaryDto>();
        }

        public ReaderWriterLockSlim Lock { get; }

        public IReadOnlyCollection<MarketSummaryDto> Markets { get; set; }
    }
}