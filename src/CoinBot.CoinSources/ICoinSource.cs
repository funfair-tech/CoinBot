using System.Collections.Generic;

namespace CoinBot.CoinSources
{
    public interface ICoinSource
    {
        string Name { get; }
        ICoin GetCoinBySymbol(string symbol);
        ICoin GetCoinByName(string name);
        ICoin Get(string nameOrSymbol);
        List<ICoin> GetTop100();

        void Start();
        void Stop();
    }
}
