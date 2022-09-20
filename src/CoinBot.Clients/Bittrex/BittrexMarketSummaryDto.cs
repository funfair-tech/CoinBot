using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace CoinBot.Clients.Bittrex;

[SuppressMessage(category: "Microsoft.Performance", checkId: "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Used as data packet")]
internal sealed class BittrexMarketSummaryDto
{
    [JsonConstructor]
    public BittrexMarketSummaryDto(string marketName,
                                   decimal? high,
                                   decimal? low,
                                   decimal? volume,
                                   decimal? last,
                                   decimal? baseVolume,
                                   DateTime? timeStamp,
                                   decimal? bid,
                                   decimal? ask)
    {
        this.MarketName = marketName;
        this.High = high;
        this.Low = low;
        this.Volume = volume;
        this.Last = last;
        this.BaseVolume = baseVolume;
        this.TimeStamp = timeStamp;
        this.Bid = bid;
        this.Ask = ask;
    }

    [JsonPropertyName(name: @"MarketName")]
    public string MarketName { get; }

    [JsonPropertyName(name: @"High")]
    [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Global", Justification = "For expansion")]
    public decimal? High { get; }

    [JsonPropertyName(name: @"Low")]
    [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Global", Justification = "For expansion")]
    public decimal? Low { get; }

    [JsonPropertyName(name: @"Volume")]
    [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Global", Justification = "For expansion")]
    public decimal? Volume { get; }

    [JsonPropertyName(name: @"Last")]
    public decimal? Last { get; }

    [JsonPropertyName(name: @"BaseVolume")]
    public decimal? BaseVolume { get; }

    [JsonPropertyName(name: @"TimeStamp")]
    public DateTime? TimeStamp { get; }

    [JsonPropertyName(name: @"Bid")]
    [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Global", Justification = "For expansion")]
    public decimal? Bid { get; }

    [JsonPropertyName(name: @"Ask")]
    [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Global", Justification = "For expansion")]
    public decimal? Ask { get; }
}