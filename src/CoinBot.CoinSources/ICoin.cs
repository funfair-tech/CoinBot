namespace CoinBot.CoinSources
{
    public interface ICoin
    {
        string Id { get; }
        string Name { get; }
        string Symbol { get; }
        int Rank { get; }
        string PriceUsd { get; set; }
        string PriceBtc { get; set; }
        string PriceEth { get; set; }
        string Volume { get; set; }
        string MarketCap { get; set; }
        string AvailableSupply { get; set; }
        string TotalSupply { get; set; }
        string HourChange { get; set; }
        string DayChange { get; set; }
        string WeekChange { get; set; }
        long? LastUpdated { get; set; }
    }
}
