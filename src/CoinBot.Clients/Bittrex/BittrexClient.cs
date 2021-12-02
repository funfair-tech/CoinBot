using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CoinBot.Core;
using CoinBot.Core.Extensions;
using CoinBot.Core.JsonConverters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CoinBot.Clients.Bittrex;

public sealed class BittrexClient : CoinClientBase, IMarketClient
{
    private const string HTTP_CLIENT_NAME = @"Bittrex";

    /// <summary>
    ///     The <see cref="Uri" /> of the CoinMarketCap endpoint.
    /// </summary>
    private static readonly Uri Endpoint = new(uriString: "https://bittrex.com/api/v1.1/public/", uriKind: UriKind.Absolute);

    /// <summary>
    ///     The <see cref="JsonSerializerOptions" />.
    /// </summary>
    private readonly JsonSerializerOptions _serializerSettings;

    public BittrexClient(IHttpClientFactory httpClientFactory, ILogger<BittrexClient> logger)
        : base(httpClientFactory: httpClientFactory, clientName: HTTP_CLIENT_NAME, logger: logger)
    {
        this._serializerSettings = new()
                                   {
                                       DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                                       PropertyNameCaseInsensitive = false,
                                       PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                                       Converters = { new DecimalAsStringConverter() }
                                   };
    }

    /// <summary>
    ///     The Exchange name.
    /// </summary>
    public string Name => "Bittrex";

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<MarketSummaryDto>> GetAsync(ICoinBuilder builder)
    {
        try
        {
            IReadOnlyList<BittrexCurrencyDto> currencies = await this.GetCurrenciesAsync();

            if (currencies.Count == 0)
            {
                return Array.Empty<MarketSummaryDto>();
            }

            foreach (BittrexCurrencyDto currency in currencies.Where(predicate: c => c.IsActive && !c.IsRestricted))
            {
                builder.Get(symbol: currency.Symbol, name: currency.Name);
            }

            IReadOnlyList<BittrexMarketSummaryDto> summaries = await this.GetMarketSummariesAsync();

            return summaries.Select(selector: summary => this.CreateMarketSummaryDto(marketSummary: summary, builder: builder))
                            .RemoveNulls()
                            .ToList();
        }
        catch (Exception e)
        {
            this.Logger.LogError(new(e.HResult), exception: e, message: e.Message);

            throw;
        }
    }

    private async Task<IReadOnlyList<BittrexCurrencyDto>> GetCurrenciesAsync()
    {
        HttpClient httpClient = this.CreateHttpClient();

        using (HttpResponseMessage response = await httpClient.GetAsync(new Uri(uriString: "getcurrencies", uriKind: UriKind.Relative)))
        {
            response.EnsureSuccessStatusCode();

            string content = await response.Content.ReadAsStringAsync();

            try
            {
                BittrexCurrenciesDto? summaries = JsonSerializer.Deserialize<BittrexCurrenciesDto>(json: content, options: this._serializerSettings);

                IReadOnlyList<BittrexCurrencyDto>? items = summaries?.Result;

                return items ?? Array.Empty<BittrexCurrencyDto>();
            }
            catch (Exception exception)
            {
                this.Logger.LogError(new(exception.HResult), exception: exception, message: "Failed to deserialize");

                return Array.Empty<BittrexCurrencyDto>();
            }
        }
    }

    private MarketSummaryDto? CreateMarketSummaryDto(BittrexMarketSummaryDto marketSummary, ICoinBuilder builder)
    {
        // always look at the quoted currency first as if that does not exist, then no point creating doing any more
        Currency? marketCurrency = builder.Get(marketSummary.MarketName.Substring(marketSummary.MarketName.IndexOf(value: '-') + 1));

        if (marketCurrency == null)
        {
            return null;
        }

        Currency? baseCurrency = builder.Get(marketSummary.MarketName.Substring(startIndex: 0, marketSummary.MarketName.IndexOf(value: '-')));

        if (baseCurrency == null)
        {
            return null;
        }

        return new(market: this.Name,
                   baseCurrency: baseCurrency,
                   marketCurrency: marketCurrency,
                   volume: marketSummary.BaseVolume,
                   last: marketSummary.Last,
                   lastUpdated: marketSummary.TimeStamp);
    }

    /// <summary>
    ///     Get the market summaries.
    /// </summary>
    /// <returns></returns>
    private async Task<IReadOnlyList<BittrexMarketSummaryDto>> GetMarketSummariesAsync()
    {
        HttpClient httpClient = this.CreateHttpClient();

        using (HttpResponseMessage response = await httpClient.GetAsync(new Uri(uriString: "getmarketsummaries", uriKind: UriKind.Relative)))
        {
            response.EnsureSuccessStatusCode();

            string content = await response.Content.ReadAsStringAsync();

            try
            {
                BittrexMarketSummariesDto? summaries = JsonSerializer.Deserialize<BittrexMarketSummariesDto>(json: content, options: this._serializerSettings);

                IReadOnlyList<BittrexMarketSummaryDto>? items = summaries?.Result;

                return items ?? Array.Empty<BittrexMarketSummaryDto>();
            }
            catch (Exception exception)
            {
                this.Logger.LogError(new(exception.HResult), exception: exception, message: "Failed to deserialize");

                return Array.Empty<BittrexMarketSummaryDto>();
            }
        }
    }

    public static void Register(IServiceCollection services)
    {
        services.AddSingleton<IMarketClient, BittrexClient>();

        AddHttpClientFactorySupport(services: services, clientName: HTTP_CLIENT_NAME, endpoint: Endpoint);
    }
}