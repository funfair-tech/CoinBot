using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CoinBot.Core;
using CoinBot.Core.Helpers;
using CoinBot.Core.JsonConverters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CoinBot.Clients.FunFair;

public abstract class FunFairClientBase : CoinClientBase
{
    private const string HTTP_CLIENT_NAME = @"FunFair";

    /// <summary>
    ///     List of crypto currencies hard coded for now.
    /// </summary>
    private readonly IReadOnlyList<string> _cryptoSymbols = new[]
                                                            {
                                                                @"ETH",
                                                                @"FUN"
                                                            };

    /// <summary>
    ///     List of fiat currencies hard coded for now.
    /// </summary>
    private readonly IReadOnlyList<string> _fiatSymbols = new[]
                                                          {
                                                              @"USD"
                                                          };

    private readonly JsonSerializerOptions _jsonSerializerSettings;

    protected FunFairClientBase(IHttpClientFactory httpClientFactory, ILogger logger)
        : base(httpClientFactory: httpClientFactory, clientName: HTTP_CLIENT_NAME, logger: logger)
    {
        this._jsonSerializerSettings = new()
                                       {
                                           DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                                           PropertyNameCaseInsensitive = false,
                                           PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                                           Converters = { new DecimalAsStringConverter() }
                                       };
    }

    internal async Task<IReadOnlyCollection<FunFairWalletPriceResultPairDto?>> GetBasePricesAsync()
    {
        List<(string cryptoSymbol, string fiatSymbol)> symbols = (from cryptoSymbol in this._cryptoSymbols
                                                                  from fiatSymbol in this._fiatSymbols
                                                                  select (cryptoSymbol, fiatSymbol)).ToList();

        try
        {
            return await Batched.WhenAllAsync(concurrent: 10, symbols.Select(this.GetCurrentPriceCommonAsync));
        }
        catch (Exception exception)
        {
            EventId eventId = new(exception.HResult);
            this.Logger.LogError(eventId: eventId, exception: exception, message: exception.Message);

            throw;
        }
    }

    public static void Register(IServiceCollection services, FunFairClientConfiguration? options)
    {
        if (options == null)
        {
            return;
        }

        if (!Uri.TryCreate(options.Endpoint ?? string.Empty, uriKind: UriKind.Absolute, out Uri? endpoint))
        {
            return;
        }

        services.AddSingleton<IMarketClient, FunFairClientMarket>();
        services.AddSingleton<ICoinClient, FunFairClientCoin>();
        services.AddSingleton(options);

        AddHttpClientFactorySupport(services: services, clientName: HTTP_CLIENT_NAME, endpoint: endpoint);
    }

    private Task<FunFairWalletPriceResultPairDto?> GetCurrentPriceCommonAsync((string tokenSymbol, string fiatCurrencySymbol) pair)
    {
        return this.GetCurrentPriceCommonAsync(tokenSymbol: pair.tokenSymbol, fiatCurrencySymbol: pair.fiatCurrencySymbol);
    }

    private async Task<FunFairWalletPriceResultPairDto?> GetCurrentPriceCommonAsync(string tokenSymbol, string fiatCurrencySymbol)
    {
        try
        {
            HttpClient client = this.CreateHttpClient();

            Uri uri = BuildUri(tokenSymbol: tokenSymbol, fiatCurrencySymbol: fiatCurrencySymbol);

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

                FunFairWalletPriceResultDto? pkt = JsonSerializer.Deserialize<FunFairWalletPriceResultDto>(json: msg, options: this._jsonSerializerSettings);

                if (pkt?.Symbol == null)
                {
                    this.Logger.LogError($"Failed to retrieve prices for {tokenSymbol} in {fiatCurrencySymbol}: No Content returned (2)");

                    return null;
                }

                this.Logger.LogDebug($"Retrieved price for {tokenSymbol}.  Currently {pkt.Price} {fiatCurrencySymbol}");

                return new(fiatCurrencySymbol: fiatCurrencySymbol, tokenSymbol: pkt.Symbol, price: pkt.Price, lastUpdated: pkt.Date);
            }
        }
        catch (Exception exception)
        {
            this.Logger.LogError(new(exception.HResult), exception: exception, $"Failed to retrieve prices for {tokenSymbol} in {fiatCurrencySymbol}: Error: {exception.Message}");

            return null;
        }
    }

    private static Uri BuildUri(string tokenSymbol, string fiatCurrencySymbol)
    {
        return new($"/Dev/token/{tokenSymbol}/{fiatCurrencySymbol}", uriKind: UriKind.Relative);
    }
}