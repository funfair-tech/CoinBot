using Newtonsoft.Json;

namespace CoinBot.Clients.Kraken
{
    public class KrakenAsset
    {
        // ReSharper disable once RedundantDefaultMemberInitializer
        public string Id { get; set; } = default!;

        [JsonProperty("altname")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string Altname { get; set; } = default!;
    }
}