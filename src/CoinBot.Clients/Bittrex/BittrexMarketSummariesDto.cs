using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace CoinBot.Clients.Bittrex;

[SuppressMessage(category: "Microsoft.Performance", checkId: "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Used as data packet")]
internal sealed class BittrexMarketSummariesDto
{
    [JsonConstructor]
    public BittrexMarketSummariesDto(List<BittrexMarketSummaryDto> result)
    {
        this.Result = result;
    }

    [JsonPropertyName(name: @"result")]
    [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Global", Justification = "For expansion")]
    public List<BittrexMarketSummaryDto> Result { get; }
}