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

        // ReSharper disable once RedundantDefaultMemberInitializer
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        public string Symbol { get; set; } = default!;

        [JsonPropertyName(name: @"CurrencyLong")]

        // ReSharper disable once RedundantDefaultMemberInitializer
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
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