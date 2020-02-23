namespace CoinBot.Clients.FunFair
{
    internal sealed class FunFairWalletPriceResultPairDto
    {
        public FunFairWalletPriceResultPairDto(string fiatCurrencySymbol, FunFairWalletPriceResultDto price)
        {
            this.FiatCurrencySymbol = fiatCurrencySymbol;
            this.Price = price;
        }

        public string FiatCurrencySymbol { get; }

        public FunFairWalletPriceResultDto Price { get; }
    }
}