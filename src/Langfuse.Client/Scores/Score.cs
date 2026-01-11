using System.Text.Json.Serialization;

namespace Langfuse.Client.Scores;

/// <summary>
/// Score associated with a trace or observation.
/// Represents the response model when reading scores from the API.
/// </summary>
public class Score
{
    /// <summary>
    /// The unique identifier of the score.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// The trace ID this score is associated with.
    /// </summary>
    [JsonPropertyName("traceId")]
    public string TraceId { get; set; } = string.Empty;

    /// <summary>
    /// The observation ID this score is associated with (optional).
    /// </summary>
    [JsonPropertyName("observationId")]
    public string? ObservationId { get; set; }

    /// <summary>
    /// The name of the score.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The numeric value of the score.
    /// </summary>
    [JsonPropertyName("value")]
    public double Value { get; set; }

    /// <summary>
    /// The string representation of the score value (for categorical/boolean scores).
    /// </summary>
    [JsonPropertyName("stringValue")]
    public string? StringValue { get; set; }

    /// <summary>
    /// The data type of the score (NUMERIC, CATEGORICAL, BOOLEAN).
    /// </summary>
    [JsonPropertyName("dataType")]
    public string DataType { get; set; } = string.Empty;

    /// <summary>
    /// Optional comment providing additional context.
    /// </summary>
    [JsonPropertyName("comment")]
    public string? Comment { get; set; }

    /// <summary>
    /// The source of the score (API, EVAL, ANNOTATION).
    /// </summary>
    [JsonPropertyName("source")]
    public string? Source { get; set; }

    /// <summary>
    /// The timestamp when the score was created.
    /// </summary>
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Reference to a score config.
    /// </summary>
    [JsonPropertyName("configId")]
    public string? ConfigId { get; set; }

    /// <summary>
    /// The environment from which this score originated.
    /// </summary>
    [JsonPropertyName("environment")]
    public string? Environment { get; set; }
}
