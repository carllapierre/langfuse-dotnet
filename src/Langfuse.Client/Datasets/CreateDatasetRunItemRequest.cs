using System.Text.Json;
using System.Text.Json.Serialization;

namespace Langfuse.Client.Datasets;

/// <summary>
/// Request model for creating a dataset run item.
/// </summary>
internal class CreateDatasetRunItemRequest
{
    /// <summary>
    /// Name of the run. If a run with this name doesn't exist, it will be created.
    /// </summary>
    [JsonPropertyName("runName")]
    public string RunName { get; set; } = string.Empty;

    /// <summary>
    /// Optional description for the run. If run exists, description will be updated.
    /// </summary>
    [JsonPropertyName("runDescription")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? RunDescription { get; set; }

    /// <summary>
    /// Optional metadata for the run. Updates run if run already exists.
    /// </summary>
    [JsonPropertyName("metadata")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public JsonElement? Metadata { get; set; }

    /// <summary>
    /// The ID of the dataset item to link.
    /// </summary>
    [JsonPropertyName("datasetItemId")]
    public string DatasetItemId { get; set; } = string.Empty;

    /// <summary>
    /// The ID of the trace to link.
    /// </summary>
    [JsonPropertyName("traceId")]
    public string TraceId { get; set; } = string.Empty;

    /// <summary>
    /// Optional observation ID to link to a specific span/generation.
    /// </summary>
    [JsonPropertyName("observationId")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ObservationId { get; set; }
}
