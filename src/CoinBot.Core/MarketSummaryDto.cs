using System;

namespace CoinBot.Core;

public sealed class MarketSummaryDto
{
    public MarketSummaryDto(DateTime? lastUpdated, string market, Currency baseCurrency, Currency marketCurrency, decimal? volume, decimal? last)
    {
        this.BaseCurrency = baseCurrency;
        this.MarketCurrency = marketCurrency;
        this.Market = market;
        this.Volume = volume;
        this.Last = last;
        this.LastUpdated = lastUpdated;
    }

    public Currency BaseCurrency { get; }

    public Currency MarketCurrency { get; }

    public string Market { get; }

    public decimal? Volume { get; }

    public decimal? Last { get; }

    public DateTime? LastUpdated { get; }
}