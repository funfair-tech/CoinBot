using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace CoinBot.Clients.GateIo;

public sealed class GateIoTicker
{
    public GateIoTicker(string pair,
                        string result,
                        decimal? last,
                        decimal? lowestAsk,
                        decimal? highestBid,
                        decimal? percentChange,
                        decimal? baseVolume,
                        decimal? quoteVolume,
                        decimal? high24Hr,
                        decimal? low24Hr)
    {
        this.Pair = pair;
        this.Result = result;
        this.Last = last;
        this.LowestAsk = lowestAsk;
        this.HighestBid = highestBid;
        this.PercentChange = percentChange;
        this.BaseVolume = baseVolume;
        this.QuoteVolume = quoteVolume;
        this.High24Hr = high24Hr;
        this.Low24Hr = low24Hr;
    }

    public string Pair { get; set; }

    [JsonPropertyName(name: @"result")]
    [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Global", Justification = "For expansion")]

    public string Result { get; }

    [JsonPropertyName(name: @"last")]
    public decimal? Last { get; }

    [JsonPropertyName(name: @"lowestAsk")]
    [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Global", Justification = "For expansion")]
    public decimal? LowestAsk { get; }

    [JsonPropertyName(name: @"highestBid")]
    [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Global", Justification = "For expansion")]
    public decimal? HighestBid { get; }

    [JsonPropertyName(name: @"percentChange")]
    [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Global", Justification = "For expansion")]
    public decimal? PercentChange { get; }

    [JsonPropertyName(name: @"baseVolume")]
    public decimal? BaseVolume { get; }

    [JsonPropertyName(name: @"quoteVolume")]
    [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Global", Justification = "For expansion")]
    public decimal? QuoteVolume { get; }

    [JsonPropertyName(name: @"high24hr")]
    [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Global", Justification = "For expansion")]
    public decimal? High24Hr { get; }

    [JsonPropertyName(name: @"low24hr")]
    [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Global", Justification = "For expansion")]
    public decimal? Low24Hr { get; }
}