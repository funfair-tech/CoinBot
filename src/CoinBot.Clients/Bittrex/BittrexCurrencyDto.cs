using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace CoinBot.Clients.Bittrex;

[DebuggerDisplay(value: "Symbol: {Symbol} Name: {Name}")]
[SuppressMessage(category: "Microsoft.Performance", checkId: "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Used as data packet")]
internal sealed class BittrexCurrencyDto
{
    public BittrexCurrencyDto(bool isRestricted)
    {
        this.IsRestricted = isRestricted;
    }

    [JsonPropertyName(name: @"Currency")]
    [SuppressMessage(category: "ReSharper", checkId: "RedundantDefaultMemberInitializer", Justification = "TODO: Review")]
    [SuppressMessage(category: "ReSharper", checkId: "AutoPropertyCanBeMadeGetOnly.Global", Justification = "TODO: Review")]
    public string Symbol { get; init; } = default!;

    [JsonPropertyName(name: @"CurrencyLong")]
    [SuppressMessage(category: "ReSharper", checkId: "RedundantDefaultMemberInitializer", Justification = "TODO: Review")]
    [SuppressMessage(category: "ReSharper", checkId: "AutoPropertyCanBeMadeGetOnly.Global", Justification = "TODO: Review")]
    public string Name { get; init; } = default!;

    [JsonPropertyName(name: @"MinConfirmation")]
    [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Local", Justification = "TODO: Review")]
    private int MinConfirmations { get; init; }

    [JsonPropertyName(name: @"IsActive")]
    [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Global", Justification = "TODO: Review")]
    public bool IsActive { get; init; }

    [JsonPropertyName(name: @"TxFee")]
    [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "TODO: Review")]
    public decimal TxFee { get; init; }

    [JsonPropertyName(name: @"IsRestricted")]
    [SuppressMessage(category: "ReSharper", checkId: "MemberCanBePrivate.Global", Justification = "TODO: Review")]
    [SuppressMessage(category: "ReSharper", checkId: "AutoPropertyCanBeMadeGetOnly.Global", Justification = "TODO: Review")]
    public bool IsRestricted { get; init; }

    [JsonPropertyName(name: @"CoinType")]
    [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Local", Justification = "TODO: Review")]
    private string CoinType { get; init; } = default!;

    [JsonPropertyName(name: @"BaseAddress")]
    [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Local", Justification = "TODO: Review")]
    private string? BaseAddress { get; init; }

    [JsonPropertyName(name: @"BaseAddress")]
    [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Local", Justification = "TODO: Review")]
    private string? Notice { get; init; }
}