namespace Langfuse.Client.Scores;

/// <summary>
/// Request model for creating a score in Langfuse.
/// Scores can be used for user feedback, evaluations, or any numeric/categorical rating.
/// </summary>
public class ScoreRequest
{
    /// <summary>
    /// The ID of the trace to attach the score to.
    /// </summary>
    public required string TraceId { get; init; }

    /// <summary>
    /// The name of the score (e.g., "user-feedback", "quality", "relevance").
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// The value for the score. Can be a number (for NUMERIC/BOOLEAN) or string (for CATEGORICAL).
    /// For boolean scores, use 1 for true and 0 for false.
    /// </summary>
    public object? Value { get; init; }

    /// <summary>
    /// Optional comment providing additional context for the score.
    /// </summary>
    public string? Comment { get; init; }

    /// <summary>
    /// Optional observation ID to link the score to a specific span within the trace.
    /// </summary>
    public string? ObservationId { get; init; }

    /// <summary>
    /// The data type of the score. Valid values: "NUMERIC", "BOOLEAN", "CATEGORICAL".
    /// If not specified, defaults based on which value field is provided.
    /// </summary>
    public string? DataType { get; init; }
}
