using System;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;

namespace CoinBot.Clients;

/// <summary>
///     Base class for Coin clients.
/// </summary>
public abstract class CoinClientBase
{
    private readonly string _clientName;

    private readonly IHttpClientFactory _httpClientFactory;

    /// <summary>
    ///     Constructor.
    /// </summary>
    /// <param name="httpClientFactory">Http client factory.</param>
    /// <param name="clientName">Name of the client.</param>
    /// <param name="logger">Logging.</param>
    protected CoinClientBase(IHttpClientFactory httpClientFactory, string clientName, ILogger logger)
    {
        this.Logger = logger;
        this._httpClientFactory = httpClientFactory;
        this._clientName = clientName;
    }

    /// <summary>
    ///     Logging.
    /// </summary>
    protected ILogger Logger { get; }

    /// <summary>
    ///     Creates the <see cref="HttpClient" />.
    /// </summary>
    /// <returns>The <see cref="HttpClient" />.</returns>
    protected HttpClient CreateHttpClient()
    {
        return this._httpClientFactory.CreateClient(this._clientName);
    }

    /// <summary>
    ///     Registers the HTTP Client with dependency injection.
    /// </summary>
    /// <param name="services">The Dependency Injection collection to register the service with.</param>
    /// <param name="clientName">The HTTP Client name.</param>
    /// <param name="endpoint">The endpoint to connect to.</param>
    protected static void AddHttpClientFactorySupport(IServiceCollection services, string clientName, Uri endpoint)
    {
        const int maxRetries = 3;

        void ConfigureClient(HttpClient httpClient)
        {
            httpClient.BaseAddress = endpoint;
            httpClient.DefaultRequestHeaders.Accept.Add(new(mediaType: @"application/json"));
            httpClient.DefaultRequestHeaders.Add(name: "User-Agent", value: "CoinBot");
        }

        services.AddHttpClient(clientName)
                .ConfigureHttpClient(ConfigureClient)
                .ConfigurePrimaryHttpMessageHandler(
                    configureHandler: _ => new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate })
                .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(value: 30)))
                .AddTransientHttpErrorPolicy(configurePolicy: p => p.WaitAndRetryAsync(retryCount: maxRetries, sleepDurationProvider: Calculate));
    }

    private static TimeSpan Calculate(int attempts)
    {
        return attempts > 1
            ? TimeSpan.FromSeconds(Math.Pow(x: 2.0, y: attempts))
            : TimeSpan.Zero;
    }
}