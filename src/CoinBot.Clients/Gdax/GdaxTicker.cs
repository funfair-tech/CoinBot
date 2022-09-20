using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace CoinBot.Clients.Gdax;

[SuppressMessage(category: "Microsoft.Performance", checkId: "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Used as data packet")]
internal sealed class GdaxTicker
{
    [JsonConstructor]
    public GdaxTicker(string productId, string ask, string bid, decimal? price, string size, DateTime? time, long tradeId, decimal? volume)
    {
        this.ProductId = productId;
        this.Ask = ask;
        this.Bid = bid;
        this.Price = price;
        this.Size = size;
        this.Time = time;
        this.TradeId = tradeId;
        this.Volume = volume;
    }

    public string ProductId { get; set; }

    [JsonPropertyName(name: @"ask")]
    [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Global", Justification = "For expansion")]
    public string Ask { get; }

    [JsonPropertyName(name: @"bid")]
    [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Global", Justification = "For expansion")]
    public string Bid { get; }

    [JsonPropertyName(name: @"price")]
    public decimal? Price { get; set; }

    [JsonPropertyName(name: @"size")]
    [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Global", Justification = "For expansion")]
    public string Size { get; }

    [JsonPropertyName(name: @"time")]
    public DateTime? Time { get; set; }

    [JsonPropertyName(name: @"trade_id")]
    public long TradeId { get; set; }

    [JsonPropertyName(name: @"volume")]
    public decimal? Volume { get; set; }
}