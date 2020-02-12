using Newtonsoft.Json;

namespace CoinBot.Clients.Kraken
{
    public class KrakenAsset
    {
        public string Id { get; set; }

        [JsonProperty("altname")]
        public string Altname { get; set; }
    }
}