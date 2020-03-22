using System.Text.Json.Serialization;

namespace CoinBot.Clients.Kraken
{
    public sealed class KrakenAsset
    {
        // ReSharper disable once RedundantDefaultMemberInitializer
        public string Id { get; set; } = default!;

        [JsonPropertyName(name: @"altname")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        public string Altname { get; set; } = default!;
    }
}