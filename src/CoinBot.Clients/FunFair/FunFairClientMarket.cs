using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CoinBot.Core;
using CoinBot.Core.Extensions;
using Microsoft.Extensions.Logging;

namespace CoinBot.Clients.FunFair;

public sealed class FunFairClientMarket : FunFairClientBase, IMarketClient
{
    public FunFairClientMarket(IHttpClientFactory httpClientFactory, ILogger<FunFairClientMarket> logger)
        : base(httpClientFactory: httpClientFactory, logger: logger)
    {
    }

    /// <inheritdoc />
    public string Name { get; } = @"FunFair Wallet";

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<MarketSummaryDto>> GetAsync(ICoinBuilder builder)
    {
        IReadOnlyCollection<FunFairWalletPriceResultPairDto?> products = await this.GetBasePricesAsync();

        return products.RemoveNulls()
                       .Select(selector: product => this.CreateMarketSummaryDto(pkt: product, builder: builder))
                       .RemoveNulls()
                       .ToList();
    }

    private MarketSummaryDto? CreateMarketSummaryDto(FunFairWalletPriceResultPairDto pkt, ICoinBuilder builder)
    {
        // always look at the quoted currency first as if that does not exist, then no point creating doing any more
        Currency? marketCurrency = builder.Get(pkt.FiatCurrencySymbol);

        if (marketCurrency == null)
        {
            return null;
        }

        Currency? baseCurrency = builder.Get(pkt.TokenSymbol);

        if (baseCurrency == null)
        {
            return null;
        }

        return new(market: this.Name, baseCurrency: baseCurrency, marketCurrency: marketCurrency, volume: 0m, last: pkt.Price, lastUpdated: pkt.LastUpdated);
    }
}