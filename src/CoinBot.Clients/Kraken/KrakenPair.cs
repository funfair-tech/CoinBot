using System.Diagnostics;
using System.Text.Json.Serialization;

namespace CoinBot.Clients.Kraken;

[DebuggerDisplay(value: "{BaseCurrency} > {QuoteCurrency} : ID: {PairId}")]
public sealed class KrakenPair
{
    [JsonConstructor]
    public KrakenPair(string pairId, string baseCurrency, string quoteCurrency)
    {
        this.PairId = pairId;
        this.BaseCurrency = baseCurrency;
        this.QuoteCurrency = quoteCurrency;
    }

    public string PairId { get; set; }

    [JsonPropertyName(name: @"base")]
    public string BaseCurrency { get; }

    [JsonPropertyName(name: @"quote")]
    public string QuoteCurrency { get; }
}