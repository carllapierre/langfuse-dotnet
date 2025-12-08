using Langfuse.Client.Caching;
using Langfuse.Client.Prompts;
using Langfuse.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Langfuse.Client;

/// <summary>
/// Client for interacting with the Langfuse API.
/// Provides access to Prompt Management features with caching support.
/// </summary>
public class LangfuseClient : LangfuseHttpClientBase
{
    private readonly LangfuseClientOptions _clientOptions;
    private readonly PromptCache? _promptCache;
    private readonly ILogger<LangfuseClient> _typedLogger;

    /// <summary>
    /// Creates a new Langfuse client with options loaded from environment variables.
    /// </summary>
    public LangfuseClient()
        : this(new LangfuseClientOptions())
    {
    }

    /// <summary>
    /// Creates a new Langfuse client with the specified options.
    /// </summary>
    /// <param name="options">Client configuration options.</param>
    /// <param name="httpClient">Optional HttpClient instance.</param>
    /// <param name="logger">Optional logger instance.</param>
    public LangfuseClient(
        LangfuseClientOptions? options = null,
        HttpClient? httpClient = null,
        ILogger<LangfuseClient>? logger = null)
        : base(
            MergeOptions(options),
            httpClient,
            logger)
    {
        _clientOptions = options ?? new LangfuseClientOptions();
        _typedLogger = logger ?? NullLogger<LangfuseClient>.Instance;

        if (_clientOptions.EnablePromptCache)
        {
            _promptCache = new PromptCache(_clientOptions.PromptCacheTtl);
        }
    }

    /// <summary>
    /// Creates a new Langfuse client using IConfiguration for settings.
    /// </summary>
    /// <param name="configuration">Configuration source.</param>
    /// <param name="httpClient">Optional HttpClient instance.</param>
    /// <param name="logger">Optional logger instance.</param>
    public LangfuseClient(
        IConfiguration configuration,
        HttpClient? httpClient = null,
        ILogger<LangfuseClient>? logger = null)
        : base(
            ConfigurationLoader.LoadAndValidate(configuration),
            httpClient,
            logger)
    {
        _clientOptions = new LangfuseClientOptions();
        _typedLogger = logger ?? NullLogger<LangfuseClient>.Instance;

        if (_clientOptions.EnablePromptCache)
        {
            _promptCache = new PromptCache(_clientOptions.PromptCacheTtl);
        }
    }

    private static LangfuseOptions MergeOptions(LangfuseClientOptions? options)
    {
        var merged = ConfigurationLoader.Merge(options);
        merged.Validate();
        return merged;
    }

    #region Prompts API

    /// <summary>
    /// Gets a text prompt by name.
    /// </summary>
    /// <param name="name">The name of the prompt. Supports names with spaces and special characters.</param>
    /// <param name="version">Optional specific version number.</param>
    /// <param name="label">Optional label (e.g., "production", "staging"). Defaults to "production" if neither version nor label is specified. Supports labels with spaces.</param>
    /// <param name="fallback">Optional fallback prompt to use if fetch fails.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The text prompt.</returns>
    /// <exception cref="LangfuseApiException">Thrown when the API request fails and no fallback is provided.</exception>
    public async Task<TextPrompt> GetPromptAsync(
        string name,
        int? version = null,
        string? label = null,
        TextPrompt? fallback = null,
        CancellationToken cancellationToken = default)
    {
        var cacheKey = PromptCache.GetCacheKey(name, version, label);

        // Check cache first
        if (_promptCache != null && _promptCache.TryGetTextPrompt(cacheKey, out var cached) && cached != null)
        {
            _typedLogger.LogDebug("Prompt cache hit for {Name}", name);
            return cached;
        }

        try
        {
            var response = await FetchPromptAsync(name, version, label, cancellationToken);

            if (response.Type != "text")
            {
                throw new InvalidOperationException(
                    $"Expected text prompt but got {response.Type}. Use GetChatPromptAsync for chat prompts.");
            }

            var prompt = PromptFactory.CreateTextPrompt(response);

            // Cache the result
            _promptCache?.SetTextPrompt(cacheKey, prompt);

            return prompt;
        }
        catch (Exception ex) when (fallback != null)
        {
            _typedLogger.LogWarning(ex, "Failed to fetch prompt {Name}, using fallback", name);
            return fallback;
        }
    }

    /// <summary>
    /// Gets a chat prompt by name.
    /// </summary>
    /// <param name="name">The name of the prompt. Supports names with spaces and special characters.</param>
    /// <param name="version">Optional specific version number.</param>
    /// <param name="label">Optional label (e.g., "production", "staging"). Defaults to "production" if neither version nor label is specified. Supports labels with spaces.</param>
    /// <param name="fallback">Optional fallback prompt to use if fetch fails.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The chat prompt.</returns>
    /// <exception cref="LangfuseApiException">Thrown when the API request fails and no fallback is provided.</exception>
    public async Task<ChatPrompt> GetChatPromptAsync(
        string name,
        int? version = null,
        string? label = null,
        ChatPrompt? fallback = null,
        CancellationToken cancellationToken = default)
    {
        var cacheKey = PromptCache.GetCacheKey(name, version, label);

        // Check cache first
        if (_promptCache != null && _promptCache.TryGetChatPrompt(cacheKey, out var cached) && cached != null)
        {
            _typedLogger.LogDebug("Prompt cache hit for {Name}", name);
            return cached;
        }

        try
        {
            var response = await FetchPromptAsync(name, version, label, cancellationToken);

            if (response.Type != "chat")
            {
                throw new InvalidOperationException(
                    $"Expected chat prompt but got {response.Type}. Use GetPromptAsync for text prompts.");
            }

            var prompt = PromptFactory.CreateChatPrompt(response);

            // Cache the result
            _promptCache?.SetChatPrompt(cacheKey, prompt);

            return prompt;
        }
        catch (Exception ex) when (fallback != null)
        {
            _typedLogger.LogWarning(ex, "Failed to fetch prompt {Name}, using fallback", name);
            return fallback;
        }
    }

    /// <summary>
    /// Clears the prompt cache.
    /// </summary>
    public void ClearPromptCache()
    {
        _promptCache?.Clear();
    }

    private async Task<PromptApiResponse> FetchPromptAsync(
        string name,
        int? version,
        string? label,
        CancellationToken cancellationToken)
    {
        var path = BuildPromptPath(name, version, label);
        return await GetAsync<PromptApiResponse>(path, cancellationToken);
    }

    private static string BuildPromptPath(string name, int? version, string? label)
    {
        var encodedName = Uri.EscapeDataString(name);
        var path = $"{LangfuseConstants.PromptsPath}/{encodedName}";

        var queryParams = new List<string>();

        if (version.HasValue)
        {
            queryParams.Add($"version={version.Value}");
        }

        if (!string.IsNullOrEmpty(label))
        {
            queryParams.Add($"label={Uri.EscapeDataString(label)}");
        }

        if (queryParams.Count > 0)
        {
            path += "?" + string.Join("&", queryParams);
        }

        return path;
    }

    #endregion

    /// <summary>
    /// Disposes the client and releases resources.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _promptCache?.Dispose();
        }

        base.Dispose(disposing);
    }
}

