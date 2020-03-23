using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace CoinBot.Clients.Bittrex
{
    [SuppressMessage(category: "Microsoft.Performance", checkId: "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Used as data packet")]
    internal sealed class BittrexMarketSummariesDto
    {
        [JsonPropertyName(name: @"result")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        public List<BittrexMarketSummaryDto> Result { get; set; } = default!;
    }
}