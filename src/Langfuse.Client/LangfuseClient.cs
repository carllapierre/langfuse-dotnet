using Langfuse.Client.Caching;
using Langfuse.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Langfuse.Client;

/// <summary>
/// Client for interacting with the Langfuse API.
/// Provides access to Prompt Management, Score/Feedback, and Dataset features.
/// </summary>
public partial class LangfuseClient : LangfuseHttpClientBase
{
    private readonly LangfuseClientOptions _clientOptions;
    private readonly PromptCache? _promptCache;

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
