using System.Text.Json.Serialization;

namespace CoinBot.Clients.Kraken
{
    public sealed class KrakenTicker
    {
        // ReSharper disable once RedundantDefaultMemberInitializer
        public string BaseCurrency { get; set; } = default!;

        // ReSharper disable once RedundantDefaultMemberInitializer
        public string QuoteCurrency { get; set; } = default!;

        /// <summary>
        ///     last trade closed array(price, lot volume)
        /// </summary>
        [JsonPropertyName(name: @"c")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        public decimal[]? Last { get; set; } = default!;

        /// <summary>
        ///     24 Hour volume array(today, last 24 hours)
        /// </summary>
        [JsonPropertyName(name: @"v")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        public decimal[]? Volume { get; set; } = default!;
    }
}