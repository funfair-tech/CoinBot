using System;
using System.Collections.Generic;
using System.Text;

namespace CoinBot.CoinSources
{
    public class EmptyCoin : ICoin
    {
        public string Id => "";

        public string Name => "";

        public string Symbol => "";

        public int Rank => 0;

        public string PriceUsd { get => ""; set => this.PriceUsd = ""; }
        public string PriceBtc { get => ""; set => this.PriceBtc = ""; }
        public string PriceEth { get => ""; set => this.PriceEth = ""; }
        public string Volume { get => ""; set => this.Volume = ""; }
        public string MarketCap { get => ""; set => this.MarketCap = ""; }
        public string AvailableSupply { get => ""; set => this.AvailableSupply = ""; }
        public string TotalSupply { get => ""; set => this.TotalSupply = ""; }
        public string HourChange { get => ""; set => this.HourChange = ""; }
        public string DayChange { get => ""; set => this.DayChange = ""; }
        public string WeekChange { get => ""; set => this.WeekChange = ""; }
        public long? LastUpdated { get => 0; set => this.LastUpdated = 0; }
    }
}
