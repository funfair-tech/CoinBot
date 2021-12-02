using System.Collections.Generic;

namespace CoinBot.Core;

public interface ICurrencyListUpdater
{
    void Update(IReadOnlyList<Currency> currencies, IGlobalInfo? globalInfo);
}