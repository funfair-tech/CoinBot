using Newtonsoft.Json;

namespace CoinBot.Clients.Kraken
{
    public sealed class KrakenAsset
    {
        // ReSharper disable once RedundantDefaultMemberInitializer
        public string Id { get; set; } = default!;

        [JsonProperty(propertyName: "altname")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string Altname { get; set; } = default!;
    }
}