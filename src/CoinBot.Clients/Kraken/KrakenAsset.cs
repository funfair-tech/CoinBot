using System.Diagnostics;
using System.Text.Json.Serialization;

namespace CoinBot.Clients.Kraken;

[DebuggerDisplay(value: "{Altname} : ID: {Id}")]
public sealed class KrakenAsset
{
    [JsonConstructor]
    public KrakenAsset(string id, string altname)
    {
        this.Id = id;
        this.Altname = altname;
    }

    public string Id { get; set; }

    [JsonPropertyName(name: @"altname")]
    public string Altname { get; }
}