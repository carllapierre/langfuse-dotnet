using System.Text.Json.Serialization;

namespace Langfuse.Client.Traces;

/// <summary>
/// Observation (span/generation) within a trace.
/// </summary>
public class Observation
{
    /// <summary>
    /// The unique identifier of the observation.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// The trace ID associated with the observation.
    /// </summary>
    [JsonPropertyName("traceId")]
    public string? TraceId { get; set; }

    /// <summary>
    /// The type of the observation (e.g., "SPAN", "GENERATION").
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>
    /// The name of the observation.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// The start time of the observation.
    /// </summary>
    [JsonPropertyName("startTime")]
    public DateTime StartTime { get; set; }

    /// <summary>
    /// The end time of the observation.
    /// </summary>
    [JsonPropertyName("endTime")]
    public DateTime? EndTime { get; set; }

    /// <summary>
    /// The completion start time of the observation (for streaming).
    /// </summary>
    [JsonPropertyName("completionStartTime")]
    public DateTime? CompletionStartTime { get; set; }

    /// <summary>
    /// The model used for the observation.
    /// </summary>
    [JsonPropertyName("model")]
    public string? Model { get; set; }

    /// <summary>
    /// The model ID.
    /// </summary>
    [JsonPropertyName("modelId")]
    public string? ModelId { get; set; }

    /// <summary>
    /// Model parameters used for the observation.
    /// </summary>
    [JsonPropertyName("modelParameters")]
    public object? ModelParameters { get; set; }

    /// <summary>
    /// The input data of the observation.
    /// </summary>
    [JsonPropertyName("input")]
    public object? Input { get; set; }

    /// <summary>
    /// The output data of the observation.
    /// </summary>
    [JsonPropertyName("output")]
    public object? Output { get; set; }

    /// <summary>
    /// The metadata associated with the observation.
    /// </summary>
    [JsonPropertyName("metadata")]
    public object? Metadata { get; set; }

    /// <summary>
    /// The parent observation ID for nested observations.
    /// </summary>
    [JsonPropertyName("parentObservationId")]
    public string? ParentObservationId { get; set; }

    /// <summary>
    /// The log level of the observation.
    /// </summary>
    [JsonPropertyName("level")]
    public string? Level { get; set; }

    /// <summary>
    /// Status message for the observation.
    /// </summary>
    [JsonPropertyName("statusMessage")]
    public string? StatusMessage { get; set; }

    /// <summary>
    /// The version of the observation.
    /// </summary>
    [JsonPropertyName("version")]
    public string? Version { get; set; }

    /// <summary>
    /// The environment from which this observation originated.
    /// </summary>
    [JsonPropertyName("environment")]
    public string? Environment { get; set; }

    /// <summary>
    /// The prompt ID if a managed prompt was used.
    /// </summary>
    [JsonPropertyName("promptId")]
    public string? PromptId { get; set; }

    /// <summary>
    /// The prompt name if a managed prompt was used.
    /// </summary>
    [JsonPropertyName("promptName")]
    public string? PromptName { get; set; }

    /// <summary>
    /// The prompt version if a managed prompt was used.
    /// </summary>
    [JsonPropertyName("promptVersion")]
    public int? PromptVersion { get; set; }

    /// <summary>
    /// Token usage details.
    /// </summary>
    [JsonPropertyName("usage")]
    public ObservationUsage? Usage { get; set; }

    /// <summary>
    /// Detailed usage breakdown by category.
    /// </summary>
    [JsonPropertyName("usageDetails")]
    public Dictionary<string, double>? UsageDetails { get; set; }

    /// <summary>
    /// Detailed cost breakdown by category.
    /// </summary>
    [JsonPropertyName("costDetails")]
    public Dictionary<string, double>? CostDetails { get; set; }

    /// <summary>
    /// Latency of the observation in seconds.
    /// </summary>
    [JsonPropertyName("latency")]
    public double? Latency { get; set; }

    /// <summary>
    /// Time to first token in seconds (for streaming).
    /// </summary>
    [JsonPropertyName("timeToFirstToken")]
    public double? TimeToFirstToken { get; set; }

    /// <summary>
    /// Calculated input cost in USD.
    /// </summary>
    [JsonPropertyName("calculatedInputCost")]
    public double? CalculatedInputCost { get; set; }

    /// <summary>
    /// Calculated output cost in USD.
    /// </summary>
    [JsonPropertyName("calculatedOutputCost")]
    public double? CalculatedOutputCost { get; set; }

    /// <summary>
    /// Calculated total cost in USD.
    /// </summary>
    [JsonPropertyName("calculatedTotalCost")]
    public double? CalculatedTotalCost { get; set; }

    /// <summary>
    /// Input price per unit.
    /// </summary>
    [JsonPropertyName("inputPrice")]
    public double? InputPrice { get; set; }

    /// <summary>
    /// Output price per unit.
    /// </summary>
    [JsonPropertyName("outputPrice")]
    public double? OutputPrice { get; set; }

    /// <summary>
    /// Total price per unit.
    /// </summary>
    [JsonPropertyName("totalPrice")]
    public double? TotalPrice { get; set; }
}
