using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace CoinBot.Clients.Bittrex;

[DebuggerDisplay(value: "Symbol: {Symbol} Name: {Name}")]
[SuppressMessage(category: "Microsoft.Performance", checkId: "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Used as data packet")]
internal sealed class BittrexCurrencyDto
{
    [JsonConstructor]
    public BittrexCurrencyDto(string symbol,
                              string name,
                              int minConfirmations,
                              bool isActive,
                              decimal txFee,
                              bool isRestricted,
                              string coinType,
                              string? baseAddress,
                              string? notice)
    {
        this.Symbol = symbol;
        this.Name = name;
        this.MinConfirmations = minConfirmations;
        this.IsActive = isActive;
        this.TxFee = txFee;
        this.CoinType = coinType;
        this.BaseAddress = baseAddress;
        this.Notice = notice;
        this.IsRestricted = isRestricted;
    }

    [JsonPropertyName(name: @"Currency")]
    public string Symbol { get; }

    [JsonPropertyName(name: @"CurrencyLong")]
    public string Name { get; }

    [JsonPropertyName(name: @"MinConfirmation")]
    [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Global", Justification = "For expansion")]
    public int MinConfirmations { get; }

    [JsonPropertyName(name: @"IsActive")]
    public bool IsActive { get; }

    [JsonPropertyName(name: @"TxFee")]
    [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Global", Justification = "For expansion")]
    public decimal TxFee { get; }

    [JsonPropertyName(name: @"IsRestricted")]
    public bool IsRestricted { get; }

    [JsonPropertyName(name: @"CoinType")]
    [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Global", Justification = "For expansion")]
    public string CoinType { get; }

    [JsonPropertyName(name: @"BaseAddress")]
    [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Global", Justification = "For expansion")]
    public string? BaseAddress { get; }

    [JsonPropertyName(name: @"BaseAddress")]
    [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Global", Justification = "For expansion")]
    public string? Notice { get; }
}