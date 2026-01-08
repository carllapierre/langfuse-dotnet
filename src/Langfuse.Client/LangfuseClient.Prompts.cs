using Langfuse.Client.Caching;
using Langfuse.Client.Prompts;
using Langfuse.Core;
using Microsoft.Extensions.Logging;

namespace Langfuse.Client;

/// <summary>
/// Partial class containing Prompt Management API methods.
/// </summary>
public partial class LangfuseClient
{
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
            Logger.LogDebug("Prompt cache hit for {Name}", name);
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
            Logger.LogWarning(ex, "Failed to fetch prompt {Name}, using fallback", name);
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
            Logger.LogDebug("Prompt cache hit for {Name}", name);
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
            Logger.LogWarning(ex, "Failed to fetch prompt {Name}, using fallback", name);
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
}

