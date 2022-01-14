using System;

namespace CoinBot.Clients.FunFair;

internal sealed class FunFairWalletPriceResultPairDto
{
    public FunFairWalletPriceResultPairDto(string fiatCurrencySymbol, string tokenSymbol, decimal price, in DateTime lastUpdated)
    {
        this.FiatCurrencySymbol = fiatCurrencySymbol;
        this.TokenSymbol = tokenSymbol;
        this.Price = price;
        this.LastUpdated = lastUpdated;
    }

    public string FiatCurrencySymbol { get; }

    public decimal Price { get; }

    public string TokenSymbol { get; }

    public DateTime LastUpdated { get; }
}