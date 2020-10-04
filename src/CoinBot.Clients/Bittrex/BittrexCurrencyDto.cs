using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace CoinBot.Clients.Bittrex
{
    [DebuggerDisplay(value: "Symbol: {Symbol} Name: {Name}")]
    [SuppressMessage(category: "Microsoft.Performance", checkId: "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Used as data packet")]
    internal sealed class BittrexCurrencyDto
    {
        [JsonPropertyName(name: @"Currency")]
        [SuppressMessage(category: "ReSharper", checkId: "RedundantDefaultMemberInitializer", Justification = "TODO: Review")]
        [SuppressMessage(category: "ReSharper", checkId: "AutoPropertyCanBeMadeGetOnly.Global", Justification = "TODO: Review")]
        public string Symbol { get; set; } = default!;

        [JsonPropertyName(name: @"CurrencyLong")]
        [SuppressMessage(category: "ReSharper", checkId: "RedundantDefaultMemberInitializer", Justification = "TODO: Review")]
        [SuppressMessage(category: "ReSharper", checkId: "AutoPropertyCanBeMadeGetOnly.Global", Justification = "TODO: Review")]
        public string Name { get; set; } = default!;

        [JsonPropertyName(name: @"MinConfirmation")]
        private int MinConfirmations { get; set; }

        [JsonPropertyName(name: @"IsActive")]
        public bool IsActive { get; set; }

        [JsonPropertyName(name: @"TxFee")]
        public decimal TxFee { get; set; }

        [JsonPropertyName(name: @"IsRestricted")]
        public bool IsRestricted { get; set; }

        [JsonPropertyName(name: @"CoinType")]
        private string CoinType { get; set; } = default!;

        [JsonPropertyName(name: @"BaseAddress")]
        private string? BaseAddress { get; set; }

        [JsonPropertyName(name: @"BaseAddress")]
        private string? Notice { get; set; }
    }
}