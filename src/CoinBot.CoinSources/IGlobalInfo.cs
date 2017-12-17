namespace CoinBot.CoinSources
{
    public interface IGlobalInfo
    {
        string MarketCap { get; }
        string Volume { get; }
        string BTCDominance { get; }
        string Currencies { get; }
        string Assets { get; }
        string Markets { get; }
        long? LastUpdated { get; set; }
    }
}
