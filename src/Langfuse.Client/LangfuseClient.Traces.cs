using Langfuse.Client.Traces;
using Langfuse.Core;
using Microsoft.Extensions.Logging;

namespace Langfuse.Client;

/// <summary>
/// Partial class containing Trace Management API methods.
/// </summary>
public partial class LangfuseClient
{
    #region List Traces

    /// <summary>
    /// Gets a paginated list of traces with optional filtering.
    /// </summary>
    /// <param name="page">Page number (1-indexed). Default: 1.</param>
    /// <param name="limit">Maximum number of traces per page. Default: 50.</param>
    /// <param name="userId">Optional filter by user ID.</param>
    /// <param name="name">Optional filter by trace name.</param>
    /// <param name="sessionId">Optional filter by session ID.</param>
    /// <param name="fromTimestamp">Optional filter to include traces on or after this timestamp.</param>
    /// <param name="toTimestamp">Optional filter to include traces before this timestamp.</param>
    /// <param name="orderBy">Optional ordering. Format: [field].[asc/desc]. Example: timestamp.desc</param>
    /// <param name="tags">Optional filter by tags. Only traces with all specified tags will be returned.</param>
    /// <param name="version">Optional filter by version.</param>
    /// <param name="release">Optional filter by release.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paginated list of traces with details.</returns>
    /// <exception cref="LangfuseApiException">Thrown when the API request fails.</exception>
    public async Task<PaginatedResponse<TraceWithDetails>> GetTracesAsync(
        int page = 1,
        int limit = 50,
        string? userId = null,
        string? name = null,
        string? sessionId = null,
        DateTime? fromTimestamp = null,
        DateTime? toTimestamp = null,
        string? orderBy = null,
        IEnumerable<string>? tags = null,
        string? version = null,
        string? release = null,
        CancellationToken cancellationToken = default)
    {
        var queryParams = new List<string>
        {
            $"page={page}",
            $"limit={limit}"
        };

        if (!string.IsNullOrEmpty(userId))
        {
            queryParams.Add($"userId={Uri.EscapeDataString(userId)}");
        }

        if (!string.IsNullOrEmpty(name))
        {
            queryParams.Add($"name={Uri.EscapeDataString(name)}");
        }

        if (!string.IsNullOrEmpty(sessionId))
        {
            queryParams.Add($"sessionId={Uri.EscapeDataString(sessionId)}");
        }

        if (fromTimestamp.HasValue)
        {
            queryParams.Add($"fromTimestamp={fromTimestamp.Value:O}");
        }

        if (toTimestamp.HasValue)
        {
            queryParams.Add($"toTimestamp={toTimestamp.Value:O}");
        }

        if (!string.IsNullOrEmpty(orderBy))
        {
            queryParams.Add($"orderBy={Uri.EscapeDataString(orderBy)}");
        }

        if (tags != null)
        {
            foreach (var tag in tags)
            {
                queryParams.Add($"tags={Uri.EscapeDataString(tag)}");
            }
        }

        if (!string.IsNullOrEmpty(version))
        {
            queryParams.Add($"version={Uri.EscapeDataString(version)}");
        }

        if (!string.IsNullOrEmpty(release))
        {
            queryParams.Add($"release={Uri.EscapeDataString(release)}");
        }

        var path = $"{LangfuseConstants.TracesPath}?{string.Join("&", queryParams)}";
        var result = await GetAsync<PaginatedResponse<TraceWithDetails>>(path, cancellationToken);

        Logger.LogDebug("Retrieved {Count} traces (page {Page})", result.Data.Count, page);
        return result;
    }

    #endregion

    #region Get Single Trace

    /// <summary>
    /// Gets a specific trace by ID with full details including observations and scores.
    /// </summary>
    /// <param name="traceId">The unique identifier of the trace.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The trace with full details.</returns>
    /// <exception cref="ArgumentNullException">Thrown when traceId is null.</exception>
    /// <exception cref="LangfuseApiException">Thrown when the API request fails or trace is not found.</exception>
    public async Task<TraceWithFullDetails> GetTraceAsync(
        string traceId,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(traceId);

        var path = $"{LangfuseConstants.TracesPath}/{Uri.EscapeDataString(traceId)}";
        var result = await GetAsync<TraceWithFullDetails>(path, cancellationToken);

        Logger.LogDebug("Retrieved trace {TraceId} with {ObservationCount} observations and {ScoreCount} scores",
            traceId, result.Observations.Count, result.Scores.Count);
        return result;
    }

    #endregion

    #region Delete Traces

    /// <summary>
    /// Deletes a specific trace by ID.
    /// Note: Trace deletions are processed asynchronously and may take up to 15 minutes to complete.
    /// </summary>
    /// <param name="traceId">The unique identifier of the trace to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <exception cref="ArgumentNullException">Thrown when traceId is null.</exception>
    /// <exception cref="LangfuseApiException">Thrown when the API request fails.</exception>
    public async Task DeleteTraceAsync(
        string traceId,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(traceId);

        var path = $"{LangfuseConstants.TracesPath}/{Uri.EscapeDataString(traceId)}";
        await DeleteAsync(path, cancellationToken);

        Logger.LogDebug("Deleted trace {TraceId}", traceId);
    }

    /// <summary>
    /// Deletes multiple traces by their IDs.
    /// Note: Trace deletions are processed asynchronously and may take up to 15 minutes to complete.
    /// </summary>
    /// <param name="traceIds">The list of trace IDs to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <exception cref="ArgumentNullException">Thrown when traceIds is null.</exception>
    /// <exception cref="ArgumentException">Thrown when traceIds is empty.</exception>
    /// <exception cref="LangfuseApiException">Thrown when the API request fails.</exception>
    public async Task DeleteTracesAsync(
        IEnumerable<string> traceIds,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(traceIds);

        var traceIdList = traceIds.ToList();
        if (traceIdList.Count == 0)
        {
            throw new ArgumentException("At least one trace ID must be provided", nameof(traceIds));
        }

        var request = new DeleteTracesRequest { TraceIds = traceIdList };
        await DeleteWithBodyAsync<DeleteTracesRequest, DeleteTraceResponse>(
            LangfuseConstants.TracesPath,
            request,
            cancellationToken);

        Logger.LogDebug("Deleted {Count} traces", traceIdList.Count);
    }

    #endregion
}
