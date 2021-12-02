using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace CoinBot.Core;

public sealed class CurrencyManager : ICurrencyListUpdater
{
    private readonly ILogger<CurrencyManager> _logger;

    /// <summary>
    ///     The <see cref="Currency" /> list.
    /// </summary>
    private IReadOnlyDictionary<string, Currency> _coinInfoByName;

    /// <summary>
    ///     The <see cref="Currency" /> list.
    /// </summary>
    private IReadOnlyDictionary<string, Currency> _coinInfoBySymbol;

    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="logger">Logging</param>
    public CurrencyManager(ILogger<CurrencyManager> logger)
    {
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));

        this._coinInfoByName = ImmutableDictionary<string, Currency>.Empty;
        this._coinInfoBySymbol = ImmutableDictionary<string, Currency>.Empty;
    }

    /// <summary>
    ///     The <see cref="IGlobalInfo" />.
    /// </summary>
    public IGlobalInfo? GlobalInfo { get; private set; }

    void ICurrencyListUpdater.Update(IReadOnlyList<Currency> currencies, IGlobalInfo? globalInfo)
    {
        this._logger.LogInformation(message: "Currencies updated");
        this._coinInfoBySymbol = currencies.ToDictionary(keySelector: key => key.Symbol, elementSelector: value => value);
        this._coinInfoByName = currencies.ToDictionary(keySelector: key => key.Name, elementSelector: value => value);
        this.GlobalInfo = globalInfo;
    }

    public Currency? Get(string nameOrSymbol)
    {
        return this.GetCoinBySymbol(nameOrSymbol) ?? this.GetCoinByName(nameOrSymbol);
    }

    private Currency? GetCoinBySymbol(string symbol)
    {
        if (this._coinInfoBySymbol.TryGetValue(symbol.ToUpperInvariant(), out Currency? currency))
        {
            return currency;
        }

        return null;
    }

    private Currency? GetCoinByName(string name)
    {
        if (this._coinInfoByName.TryGetValue(name.ToUpperInvariant(), out Currency? currency))
        {
            return currency;
        }

        return null;
    }

    public IEnumerable<Currency> Get(Func<Currency, bool> predicate)
    {
        return this._coinInfoBySymbol.Values.Where(predicate);
    }
}