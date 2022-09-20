using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace CoinBot.Clients.FunFair;

/// <summary>
///     The price source response packet
/// </summary>
[SuppressMessage(category: "Microsoft.Performance", checkId: "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Used as data packet")]
internal sealed class FunFairWalletPriceResultDto
{
    [JsonConstructor]
    public FunFairWalletPriceResultDto(string? status,
                                       decimal price,
                                       string? symbol,
                                       [SuppressMessage(category: "Roslynator.Analyzers", checkId: "RCS1231: make ref read-only", Justification = "Can't be for json serialisatio")] DateTime date)
    {
        this.Status = status;
        this.Price = price;
        this.Symbol = symbol;
        this.Date = date;
    }

    [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Global", Justification = "For expansion")]
    public string? Status { get; }

    [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Global", Justification = "For expansion")]
    public decimal Price { get; }

    [SuppressMessage(category: "ReSharper", checkId: "UnusedAutoPropertyAccessor.Global", Justification = "For expansion")]
    public string? Symbol { get; }

    public DateTime Date { get; set; }
}