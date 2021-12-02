using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CoinBot.Core;
using CoinBot.Core.Extensions;
using Microsoft.Extensions.Logging;

namespace CoinBot.Clients.FunFair;

public sealed class FunFairClientCoin : FunFairClientBase, ICoinClient
{
    public FunFairClientCoin(IHttpClientFactory httpClientFactory, ILogger<FunFairClientCoin> logger)
        : base(httpClientFactory: httpClientFactory, logger: logger)
    {
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<ICoinInfo>> GetCoinInfoAsync()
    {
        IReadOnlyCollection<FunFairWalletPriceResultPairDto?> source = await this.GetBasePricesAsync();

        return source.RemoveNulls()
                     .Where(predicate: price => price.FiatCurrencySymbol == @"USD")
                     .Select(this.CreateCoinInfo)
                     .ToList();
    }

    /// <inheritdoc />
    public Task<IGlobalInfo?> GetGlobalInfoAsync()
    {
        IGlobalInfo? none = null;

        return Task.FromResult(none);
    }

    private ICoinInfo CreateCoinInfo(FunFairWalletPriceResultPairDto item)
    {
        return new FunFairWalletCoin(priceUsd: item.Price, symbol: item.TokenSymbol, lastUpdated: item.LastUpdated);
    }
}