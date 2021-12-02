using System.Collections.Generic;
using System.Linq;
using NonBlocking;

namespace CoinBot.Core;

internal sealed class CoinBuilder : ICoinBuilder
{
    private readonly ConcurrentDictionary<string, Currency> _coinsBySymbol;

    public CoinBuilder()
    {
        this._coinsBySymbol = new();

        this._coinsBySymbol.TryAdd(key: @"EUR", new(symbol: "EUR", name: "Euro") { IsFiat = true });
        this._coinsBySymbol.TryAdd(key: @"USD", new(symbol: "USD", name: "United States dollar") { IsFiat = true });
    }

    public Currency Get(string symbol, string name)
    {
        string upperSymbol = symbol.ToUpperInvariant();

        return this._coinsBySymbol.GetOrAdd(key: upperSymbol, new Currency(symbol: upperSymbol, name: name));
    }

    public Currency? Get(string symbol)
    {
        string upperSymbol = symbol.ToUpperInvariant();

        if (this._coinsBySymbol.TryGetValue(key: upperSymbol, out Currency? currency))
        {
            return currency;
        }

        return null;
    }

    public IReadOnlyList<Currency> AllCurrencies()
    {
        return this._coinsBySymbol.Values.ToArray();
    }
}