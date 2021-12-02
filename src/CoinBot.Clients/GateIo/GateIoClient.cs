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

namespace CoinBot.Clients.GateIo;

public sealed class GateIoClient : CoinClientBase, IMarketClient
{
    private const string HTTP_CLIENT_NAME = @"GateIo";

    private const char PAIR_SEPARATOR = '_';

    /// <summary>
    ///     The <see cref="Uri" /> of the CoinMarketCap endpoint.
    /// </summary>
    private static readonly Uri Endpoint = new(uriString: "http://data.gate.io/api2/1/", uriKind: UriKind.Absolute);

    /// <summary>
    ///     The <see cref="JsonSerializerOptions" />.
    /// </summary>
    private readonly JsonSerializerOptions _serializerSettings;

    public GateIoClient(IHttpClientFactory httpClientFactory, ILogger<GateIoClient> logger)
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
    public string Name => @"Gate.io";

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<MarketSummaryDto>> GetAsync(ICoinBuilder builder)
    {
        try
        {
            IReadOnlyList<GateIoTicker> tickers = await this.GetTickersAsync();

            return tickers.Select(selector: ticker => this.CreateMarketSummaryDto(marketSummary: ticker, builder: builder))
                          .RemoveNulls()
                          .ToList();
        }
        catch (Exception e)
        {
            this.Logger.LogError(new(e.HResult), exception: e, message: e.Message);

            throw;
        }
    }

    private MarketSummaryDto? CreateMarketSummaryDto(GateIoTicker marketSummary, ICoinBuilder builder)
    {
        // always look at the quoted currency first as if that does not exist, then no point creating doing any more
        Currency? marketCurrency = builder.Get(marketSummary.Pair.Substring(marketSummary.Pair.IndexOf(PAIR_SEPARATOR) + 1));

        if (marketCurrency == null)
        {
            return null;
        }

        Currency? baseCurrency = builder.Get(marketSummary.Pair.Substring(startIndex: 0, marketSummary.Pair.IndexOf(PAIR_SEPARATOR)));

        if (baseCurrency == null)
        {
            return null;
        }

        return new(market: this.Name, baseCurrency: baseCurrency, marketCurrency: marketCurrency, volume: marketSummary.BaseVolume, last: marketSummary.Last, lastUpdated: null);
    }

    /// <summary>
    ///     Get the market summaries.
    /// </summary>
    /// <returns></returns>
    private async Task<IReadOnlyList<GateIoTicker>> GetTickersAsync()
    {
        HttpClient httpClient = this.CreateHttpClient();

        using (HttpResponseMessage response = await httpClient.GetAsync(new Uri(uriString: "tickers", uriKind: UriKind.Relative)))
        {
            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();

            try
            {
                Dictionary<string, GateIoTicker>? items = JsonSerializer.Deserialize<Dictionary<string, GateIoTicker>>(json: json, options: this._serializerSettings);

                if (items == null)
                {
                    return Array.Empty<GateIoTicker>();
                }

                foreach ((string pair, GateIoTicker ticker) in items)
                {
                    ticker.Pair = pair;
                }

                return items.Values.ToArray();
            }
            catch (Exception exception)
            {
                this.Logger.LogError(new(exception.HResult), exception: exception, message: "Could not deserialise");

                return Array.Empty<GateIoTicker>();
            }
        }
    }

    public static void Register(IServiceCollection services)
    {
        services.AddSingleton<IMarketClient, GateIoClient>();

        AddHttpClientFactorySupport(services: services, clientName: HTTP_CLIENT_NAME, endpoint: Endpoint);
    }
}