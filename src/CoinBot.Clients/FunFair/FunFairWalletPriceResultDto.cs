using System;
using System.Diagnostics.CodeAnalysis;

namespace CoinBot.Clients.FunFair
{
    /// <summary>
    ///     The price source response packet
    /// </summary>
    [SuppressMessage(category: "Microsoft.Performance", checkId: "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Used as data packet")]
    internal sealed class FunFairWalletPriceResultDto
    {
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        public string? Status { get; set; } = default!;

        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        public decimal Price { get; set; } = default!;

        // ReSharper disable once UnusedMember.Local
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        public string? Symbol { get; set; } = default!;

        public DateTime Date { get; set; }
    }
}