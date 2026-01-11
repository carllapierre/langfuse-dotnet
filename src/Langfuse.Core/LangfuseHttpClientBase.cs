using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Langfuse.Core;

/// <summary>
/// Base HTTP client for Langfuse API interactions.
/// Provides authentication, JSON serialization, and error handling.
/// </summary>
public abstract class LangfuseHttpClientBase : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly bool _ownsHttpClient;
    private bool _disposed;

    /// <summary>
    /// Gets the configured options.
    /// </summary>
    protected LangfuseOptions Options { get; }

    /// <summary>
    /// Gets the logger instance.
    /// </summary>
    protected ILogger Logger { get; }

    /// <summary>
    /// JSON serializer options for API requests/responses.
    /// </summary>
    protected static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        WriteIndented = false
    };

    /// <summary>
    /// Creates a new instance with the specified options.
    /// </summary>
    /// <param name="options">Langfuse options.</param>
    /// <param name="httpClient">Optional HttpClient instance. If not provided, one will be created.</param>
    /// <param name="logger">Optional logger instance.</param>
    protected LangfuseHttpClientBase(
        LangfuseOptions options,
        HttpClient? httpClient = null,
        ILogger? logger = null)
    {
        Options = options ?? throw new ArgumentNullException(nameof(options));
        Options.Validate();

        Logger = logger ?? NullLogger.Instance;

        if (httpClient != null)
        {
            _httpClient = httpClient;
            _ownsHttpClient = false;
        }
        else
        {
            _httpClient = CreateDefaultHttpClient(options);
            _ownsHttpClient = true;
        }

        ConfigureHttpClient();
    }

    private static HttpClient CreateDefaultHttpClient(LangfuseOptions options)
    {
        var handler = new HttpClientHandler
        {
            AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate
        };

        return new HttpClient(handler)
        {
            Timeout = options.Timeout
        };
    }

    private void ConfigureHttpClient()
    {
        _httpClient.BaseAddress = new Uri(Options.BaseUrl!);
        _httpClient.DefaultRequestHeaders.Authorization =
            AuthenticationHeaderValue.Parse(AuthenticationHelper.BuildBasicAuthHeader(Options));
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("langfuse-dotnet/1.0.0");
    }

    /// <summary>
    /// Sends a GET request and deserializes the response.
    /// </summary>
    protected async Task<T> GetAsync<T>(string path, CancellationToken cancellationToken = default)
    {
        Logger.LogDebug("GET {Path}", path);

        using var response = await _httpClient.GetAsync(path, cancellationToken);
        return await HandleResponseAsync<T>(response, cancellationToken);
    }

    /// <summary>
    /// Sends a POST request with JSON body and deserializes the response.
    /// </summary>
    protected async Task<TResponse> PostAsync<TRequest, TResponse>(
        string path,
        TRequest body,
        CancellationToken cancellationToken = default)
    {
        Logger.LogDebug("POST {Path}", path);

        var json = JsonSerializer.Serialize(body, JsonOptions);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        using var response = await _httpClient.PostAsync(path, content, cancellationToken);
        return await HandleResponseAsync<TResponse>(response, cancellationToken);
    }

    /// <summary>
    /// Sends a POST request with JSON body (no response body expected).
    /// </summary>
    protected async Task PostAsync<TRequest>(
        string path,
        TRequest body,
        CancellationToken cancellationToken = default)
    {
        Logger.LogDebug("POST {Path}", path);

        var json = JsonSerializer.Serialize(body, JsonOptions);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        using var response = await _httpClient.PostAsync(path, content, cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
    }

    /// <summary>
    /// Sends a DELETE request (no response body expected).
    /// </summary>
    protected async Task DeleteAsync(string path, CancellationToken cancellationToken = default)
    {
        Logger.LogDebug("DELETE {Path}", path);

        using var response = await _httpClient.DeleteAsync(path, cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
    }

    /// <summary>
    /// Sends a DELETE request with JSON body and deserializes the response.
    /// </summary>
    protected async Task<TResponse> DeleteWithBodyAsync<TRequest, TResponse>(
        string path,
        TRequest body,
        CancellationToken cancellationToken = default)
    {
        Logger.LogDebug("DELETE {Path}", path);

        var json = JsonSerializer.Serialize(body, JsonOptions);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        using var request = new HttpRequestMessage(HttpMethod.Delete, path) { Content = content };
        using var response = await _httpClient.SendAsync(request, cancellationToken);
        return await HandleResponseAsync<TResponse>(response, cancellationToken);
    }

    private async Task<T> HandleResponseAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        await EnsureSuccessAsync(response, cancellationToken);

        var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var result = await JsonSerializer.DeserializeAsync<T>(stream, JsonOptions, cancellationToken);

        if (result == null)
        {
            throw new LangfuseApiException("Failed to deserialize response", (int)response.StatusCode);
        }

        return result;
    }

    private async Task EnsureSuccessAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
            return;

        var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
        Logger.LogError("API request failed: {StatusCode} - {Content}", response.StatusCode, errorContent);

        throw new LangfuseApiException(
            $"Langfuse API request failed: {response.StatusCode} - {errorContent}",
            (int)response.StatusCode,
            errorContent);
    }

    /// <summary>
    /// Disposes the HTTP client if owned by this instance.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes resources.
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing && _ownsHttpClient)
        {
            _httpClient.Dispose();
        }

        _disposed = true;
    }
}

