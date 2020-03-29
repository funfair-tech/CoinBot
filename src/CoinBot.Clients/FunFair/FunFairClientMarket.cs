using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CoinBot.Core;
using CoinBot.Core.Extensions;
using Microsoft.Extensions.Logging;

namespace CoinBot.Clients.FunFair
{
    public sealed class FunFairClientMarket : FunFairClientBase, IMarketClient
    {
        private readonly CurrencyManager _currencyManager;

        public FunFairClientMarket(IHttpClientFactory httpClientFactory, ILogger<FunFairClientMarket> logger, CurrencyManager currencyManager)
            : base(httpClientFactory, logger)
        {
            this._currencyManager = currencyManager ?? throw new ArgumentNullException(nameof(currencyManager));
        }

        /// <inheritdoc />
        public string Name { get; } = @"FunFair Wallet";

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<MarketSummaryDto>> GetAsync()
        {
            IReadOnlyCollection<FunFairWalletPriceResultPairDto?> products = await this.GetBasePricesAsync();

            return products.RemoveNulls()
                           .Select(this.CreateMarketSummaryDto)
                           .RemoveNulls()
                           .ToList();
        }

        private MarketSummaryDto? CreateMarketSummaryDto(FunFairWalletPriceResultPairDto pkt)
        {
            Currency? baseCurrency = this._currencyManager.Get(pkt.TokenSymbol);

            if (baseCurrency == null)
            {
                return null;
            }

            Currency? marketCurrency = this._currencyManager.Get(pkt.FiatCurrencySymbol);

            if (marketCurrency == null)
            {
                return null;
            }

            return new MarketSummaryDto(market: this.Name, baseCurrency: baseCurrency, marketCurrency: marketCurrency, volume: 0m, last: pkt.Price, lastUpdated: pkt.LastUpdated);
        }
    }
}