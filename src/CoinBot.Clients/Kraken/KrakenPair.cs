using System.Diagnostics;
using System.Text.Json.Serialization;

namespace CoinBot.Clients.Kraken
{
    [DebuggerDisplay(value: "{BaseCurrency} > {QuoteCurrency} : ID: {PairId}")]
    public sealed class KrakenPair
    {
        // ReSharper disable once RedundantDefaultMemberInitializer
        public string PairId { get; set; } = default!;

        [JsonPropertyName(name: @"base")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        public string BaseCurrency { get; set; } = default!;

        [JsonPropertyName(name: @"quote")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        public string QuoteCurrency { get; set; } = default!;
    }
}