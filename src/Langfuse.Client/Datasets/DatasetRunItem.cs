using System.Text.Json.Serialization;

namespace Langfuse.Client.Datasets;

/// <summary>
/// Represents a dataset run item in Langfuse.
/// A run item links a trace (LLM execution) to a dataset item within a specific run.
/// </summary>
public class DatasetRunItem
{
    /// <summary>
    /// Unique identifier for the dataset run item.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// The ID of the dataset run this item belongs to.
    /// </summary>
    [JsonPropertyName("datasetRunId")]
    public string DatasetRunId { get; set; } = string.Empty;

    /// <summary>
    /// The name of the dataset run this item belongs to.
    /// </summary>
    [JsonPropertyName("datasetRunName")]
    public string DatasetRunName { get; set; } = string.Empty;

    /// <summary>
    /// The ID of the dataset item this run item is linked to.
    /// </summary>
    [JsonPropertyName("datasetItemId")]
    public string DatasetItemId { get; set; } = string.Empty;

    /// <summary>
    /// The ID of the trace this run item is linked to.
    /// </summary>
    [JsonPropertyName("traceId")]
    public string TraceId { get; set; } = string.Empty;

    /// <summary>
    /// Optional observation ID if the run item is linked to a specific span/generation.
    /// </summary>
    [JsonPropertyName("observationId")]
    public string? ObservationId { get; set; }

    /// <summary>
    /// When the run item was created.
    /// </summary>
    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// When the run item was last updated.
    /// </summary>
    [JsonPropertyName("updatedAt")]
    public DateTime UpdatedAt { get; set; }
}
