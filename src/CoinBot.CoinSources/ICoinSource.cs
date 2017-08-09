namespace CoinBot.CoinSources
{
    public interface ICoinSource
    {
        string Name { get; }
        ICoin GetCoinBySymbol(string symbol);
        ICoin GetCoinByName(string name);
        ICoin Get(string nameOrSymbol);

        void Start();
        void Stop();
    }
}
