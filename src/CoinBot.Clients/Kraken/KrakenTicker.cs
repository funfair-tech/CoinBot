using Newtonsoft.Json;

namespace CoinBot.Clients.Kraken
{
    public class KrakenTicker
    {
        // ReSharper disable once RedundantDefaultMemberInitializer
        public string BaseCurrency { get; set; } = default!;

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string QuoteCurrency { get; set; } = default!;

        /// <summary>
        /// last trade closed array(price, lot volume)
        /// </summary>
        [JsonProperty("c")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public decimal?[] Last { get; set; } = default!;

        /// <summary>
        /// 24 Hour volume array(today, last 24 hours)
        /// </summary>
        [JsonProperty("v")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public decimal?[] Volume { get; set; } = default!;
    }
}