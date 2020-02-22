using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using CoinBot.Core;
using CoinBot.Core.Extensions;
using CoinBot.Core.JsonConverters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CoinBot.Clients.FunFair
{
    public sealed class FunFairClient : CoinClientBase, IMarketClient
    {
        private const string HTTP_CLIENT_NAME = @"FunFair";

        private readonly IReadOnlyList<string> _cryptoSymbols = new[] {@"ETH", @"FUN"};
        private readonly CurrencyManager _currencyManager;
        private readonly IReadOnlyList<string> _fiatSymbols = new[] {@"USD"};
        private readonly JsonSerializerOptions _jsonSerializerSettings;

        public FunFairClient(IHttpClientFactory httpClientFactory, ILogger<FunFairClient> logger, CurrencyManager currencyManager)
            : base(httpClientFactory, HTTP_CLIENT_NAME, logger)
        {
            this._currencyManager = currencyManager ?? throw new ArgumentNullException(nameof(currencyManager));
            this._jsonSerializerSettings = new JsonSerializerOptions
                                           {
                                               IgnoreNullValues = true,
                                               PropertyNameCaseInsensitive = false,
                                               PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                                               Converters = {new DecimalAsStringConverter()}
                                           };
        }

        /// <inheritdoc />
        public string Name { get; } = @"FunFair Wallet";

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<MarketSummaryDto>> GetAsync()
        {
            List<(string cryptoSymbol, string fiatSymbol)> symbols =
                (from cryptoSymbol in this._cryptoSymbols from fiatSymbol in this._fiatSymbols select (cryptoSymbol, fiatSymbol)).ToList();

            try
            {
                MarketSummaryDto[] products = await Task.WhenAll(symbols.Select(this.GetCurrentPriceCommonAsync));

                return products.RemoveNulls()
                               .ToList();
            }
            catch (Exception exception)
            {
                EventId eventId = new EventId(exception.HResult);
                this.Logger.LogError(eventId, exception, exception.Message);

                throw;
            }
        }

        public static void Register(IServiceCollection services, FunFairClientConfiguration? options)
        {
            if (options == null)
            {
                return;
            }

            if (!Uri.TryCreate(options.Endpoint ?? string.Empty, UriKind.Absolute, out Uri? endpoint))
            {
                return;
            }

            services.AddSingleton<IMarketClient, FunFairClient>();
            services.AddSingleton(options);

            AddHttpClientFactorySupport(services, HTTP_CLIENT_NAME, endpoint);
        }

        private Task<MarketSummaryDto?> GetCurrentPriceCommonAsync((string tokenSymbol, string fiatCurrencySymbol) pair)
        {
            return this.GetCurrentPriceCommonAsync(pair.tokenSymbol, pair.fiatCurrencySymbol);
        }

        private async Task<MarketSummaryDto?> GetCurrentPriceCommonAsync(string tokenSymbol, string fiatCurrencySymbol)
        {
            HttpClient client = this.CreateHttpClient();

            Uri uri = BuildUri(tokenSymbol, fiatCurrencySymbol);

            using (HttpResponseMessage response = await client.GetAsync(uri))
            {
                if (!response.IsSuccessStatusCode)
                {
                    this.Logger.LogError($"Failed to retrieve prices for {tokenSymbol} in {fiatCurrencySymbol}: Http Error: {response.StatusCode}");

                    return null;
                }

                string msg = await response.Content.ReadAsStringAsync();

                if (string.IsNullOrWhiteSpace(msg))
                {
                    this.Logger.LogError($"Failed to retrieve prices for {tokenSymbol} in {fiatCurrencySymbol}: No Content returned (1)");

                    return null;
                }

                FunFairWalletPriceResultDto pkt = JsonSerializer.Deserialize<FunFairWalletPriceResultDto>(msg, this._jsonSerializerSettings);

                if (pkt.Symbol == null)
                {
                    this.Logger.LogError($"Failed to retrieve prices for {tokenSymbol} in {fiatCurrencySymbol}: No Content returned (2)");

                    return null;
                }

                this.Logger.LogDebug($"Retrieved price for {tokenSymbol}.  Currently {pkt.Price} {fiatCurrencySymbol}");

                Currency? baseCurrency = this._currencyManager.Get(fiatCurrencySymbol);

                if (baseCurrency == null)
                {
                    return null;
                }

                Currency? marketCurrency = this._currencyManager.Get(tokenSymbol);

                if (marketCurrency == null)
                {
                    return null;
                }

                return new MarketSummaryDto(market: this.Name, baseCurrency: baseCurrency, marketCurrency: marketCurrency, volume: 0m, last: pkt.Price, lastUpdated: pkt.Date);
            }
        }

        private static Uri BuildUri(string tokenSymbol, string fiatCurrencySymbol)
        {
            return new Uri($"/Dev/token/{tokenSymbol}/{fiatCurrencySymbol}", UriKind.Relative);
        }
    }
}