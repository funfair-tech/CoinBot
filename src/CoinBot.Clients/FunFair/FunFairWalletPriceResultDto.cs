using System;
using System.Diagnostics.CodeAnalysis;

namespace CoinBot.Clients.FunFair;

/// <summary>
///     The price source response packet
/// </summary>
[SuppressMessage(category: "Microsoft.Performance", checkId: "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Used as data packet")]
internal sealed class FunFairWalletPriceResultDto
{
    [SuppressMessage(category: "ReSharper", checkId: "AutoPropertyCanBeMadeGetOnly.Global", Justification = "TODO: Review")]
    public string? Status { get; set; } = default!;

    [SuppressMessage(category: "ReSharper", checkId: "AutoPropertyCanBeMadeGetOnly.Global", Justification = "TODO: Review")]
    public decimal Price { get; set; } = default!;

    [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Local", Justification = "TODO: Review")]
    [SuppressMessage(category: "ReSharper", checkId: "AutoPropertyCanBeMadeGetOnly.Global", Justification = "TODO: Review")]
    public string? Symbol { get; set; } = default!;

    public DateTime Date { get; set; }
}