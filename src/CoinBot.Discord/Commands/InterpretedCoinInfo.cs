using System;
using System.Collections.Generic;
using System.Linq;
using CoinBot.Core;
using CoinBot.Core.Helpers;

namespace CoinBot.Discord.Commands;

internal sealed class InterpretedCoinInfo : ICoinInfo
{
    public InterpretedCoinInfo(Currency currency, MarketManager marketManager, Currency? usd, Currency? eth, Currency? btc)
    {
        this.Id = currency.Symbol;
        this.Symbol = currency.Symbol;
        this.Name = currency.Name;

        this.PriceUsd = GetPriceFromMarkets(currency: currency, marketManager: marketManager, quoteCurrency: usd);
        this.PriceEth = GetPriceFromMarkets(currency: currency, marketManager: marketManager, quoteCurrency: eth);
        this.PriceBtc = GetPriceFromMarkets(currency: currency, marketManager: marketManager, quoteCurrency: btc);
    }

    public string Id { get; }

    public string ImageUrl =>
        Images.CurrencyImageUrl(this.Symbol)
              .ToString();

    public string Name { get; }

    public string Symbol { get; }

    public int? Rank { get; }

    public decimal? PriceUsd { get; }

    public decimal? PriceBtc { get; }

    public decimal? PriceEth { get; }

    public double? Volume { get; }

    public double? MarketCap { get; }

    public double? HourChange { get; }

    public double? DayChange { get; }

    public double? WeekChange { get; }

    public DateTime? LastUpdated { get; }

    private static decimal? GetPriceFromMarkets(Currency currency, MarketManager marketManager, Currency? quoteCurrency)
    {
        if (quoteCurrency == null)
        {
            return null;
        }

        IEnumerable<MarketSummaryDto> markets = marketManager.GetPair(currency1: currency, currency2: quoteCurrency);

        return markets.Where(predicate: x => x.Last != null)
                      .OrderByDescending(keySelector: x => x.LastUpdated ?? DateTime.MinValue)
                      .FirstOrDefault()
                      ?.Last;
    }
}