using System.Text.Json.Serialization;

namespace CoinBot.Clients.Kraken;

public sealed class KrakenTicker
{
    [JsonConstructor]
    public KrakenTicker(string baseCurrency, string quoteCurrency, decimal[]? last, decimal[]? volume)
    {
        this.BaseCurrency = baseCurrency;
        this.QuoteCurrency = quoteCurrency;
        this.Last = last;
        this.Volume = volume;
    }

    public string BaseCurrency { get; set; }

    public string QuoteCurrency { get; set; }

    /// <summary>
    ///     last trade closed array(price, lot volume)
    /// </summary>
    [JsonPropertyName(name: @"c")]
    public decimal[]? Last { get; }

    /// <summary>
    ///     24 Hour volume array(today, last 24 hours)
    /// </summary>
    [JsonPropertyName(name: @"v")]
    public decimal[]? Volume { get; }
}