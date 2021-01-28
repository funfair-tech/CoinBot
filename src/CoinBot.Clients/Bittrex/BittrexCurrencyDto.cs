using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace CoinBot.Clients.Bittrex
{
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

        // ReSharper disable once UnusedMember.Local
        private int MinConfirmations { get; set; }

        [JsonPropertyName(name: @"IsActive")]

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public bool IsActive { get; set; }

        [JsonPropertyName(name: @"TxFee")]

        // ReSharper disable once UnusedMember.Global
        public decimal TxFee { get; set; }

        [JsonPropertyName(name: @"IsRestricted")]

        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        public bool IsRestricted { get; init; }

        [JsonPropertyName(name: @"CoinType")]

        // ReSharper disable once UnusedMember.Local
        private string CoinType { get; set; } = default!;

        [JsonPropertyName(name: @"BaseAddress")]

        // ReSharper disable once UnusedMember.Local
        private string? BaseAddress { get; set; }

        [JsonPropertyName(name: @"BaseAddress")]

        // ReSharper disable once UnusedMember.Local
        private string? Notice { get; set; }
    }
}