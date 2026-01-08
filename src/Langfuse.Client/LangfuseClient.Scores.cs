using Langfuse.Client.Scores;
using Langfuse.Core;
using Microsoft.Extensions.Logging;

namespace Langfuse.Client;

/// <summary>
/// Partial class containing Scores/User Feedback API methods.
/// </summary>
public partial class LangfuseClient
{
    /// <summary>
    /// Creates a numeric score linked to a trace.
    /// Use this for user feedback (thumbs up/down), quality ratings, or any numeric evaluation.
    /// </summary>
    /// <param name="traceId">The ID of the trace to attach the score to.</param>
    /// <param name="name">The name of the score (e.g., "user-feedback", "quality").</param>
    /// <param name="value">Numeric value for the score. For thumbs up/down, use 1 or 0.</param>
    /// <param name="comment">Optional comment providing additional context.</param>
    /// <param name="observationId">Optional observation ID to link the score to a specific span.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <exception cref="LangfuseApiException">Thrown when the API request fails.</exception>
    public async Task CreateScoreAsync(
        string traceId,
        string name,
        double value,
        string? comment = null,
        string? observationId = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(traceId);
        ArgumentNullException.ThrowIfNull(name);

        var request = new ScoreRequest
        {
            TraceId = traceId,
            Name = name,
            Value = value,
            Comment = comment,
            ObservationId = observationId,
            DataType = "NUMERIC"
        };

        await PostAsync(LangfuseConstants.ScoresPath, request, cancellationToken);
        Logger.LogDebug("Created numeric score '{Name}' for trace {TraceId}", name, traceId);
    }

    /// <summary>
    /// Creates a boolean score linked to a trace.
    /// Use this for pass/fail evaluations or yes/no feedback.
    /// </summary>
    /// <param name="traceId">The ID of the trace to attach the score to.</param>
    /// <param name="name">The name of the score (e.g., "correct", "helpful").</param>
    /// <param name="value">Boolean value for the score.</param>
    /// <param name="comment">Optional comment providing additional context.</param>
    /// <param name="observationId">Optional observation ID to link the score to a specific span.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <exception cref="LangfuseApiException">Thrown when the API request fails.</exception>
    public async Task CreateScoreAsync(
        string traceId,
        string name,
        bool value,
        string? comment = null,
        string? observationId = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(traceId);
        ArgumentNullException.ThrowIfNull(name);

        var request = new ScoreRequest
        {
            TraceId = traceId,
            Name = name,
            Value = value ? 1 : 0,
            Comment = comment,
            ObservationId = observationId,
            DataType = "BOOLEAN"
        };

        await PostAsync(LangfuseConstants.ScoresPath, request, cancellationToken);
        Logger.LogDebug("Created boolean score '{Name}' for trace {TraceId}", name, traceId);
    }

    /// <summary>
    /// Creates a categorical score linked to a trace.
    /// Use this for classification or labeling feedback.
    /// </summary>
    /// <param name="traceId">The ID of the trace to attach the score to.</param>
    /// <param name="name">The name of the score (e.g., "category", "sentiment").</param>
    /// <param name="stringValue">Categorical string value for the score.</param>
    /// <param name="comment">Optional comment providing additional context.</param>
    /// <param name="observationId">Optional observation ID to link the score to a specific span.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <exception cref="LangfuseApiException">Thrown when the API request fails.</exception>
    public async Task CreateScoreAsync(
        string traceId,
        string name,
        string stringValue,
        string? comment = null,
        string? observationId = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(traceId);
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(stringValue);

        var request = new ScoreRequest
        {
            TraceId = traceId,
            Name = name,
            Value = stringValue,
            Comment = comment,
            ObservationId = observationId,
            DataType = "CATEGORICAL"
        };

        await PostAsync(LangfuseConstants.ScoresPath, request, cancellationToken);
        Logger.LogDebug("Created categorical score '{Name}' for trace {TraceId}", name, traceId);
    }
}

