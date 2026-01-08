using System.Text.Json;
using System.Text.Json.Serialization;

namespace Langfuse.Client.Datasets;

/// <summary>
/// Request model for creating a dataset item in Langfuse.
/// </summary>
internal class CreateDatasetItemRequest
{
    /// <summary>
    /// The name of the dataset to add the item to.
    /// </summary>
    [JsonPropertyName("datasetName")]
    public required string DatasetName { get; init; }

    /// <summary>
    /// The input data for this dataset item.
    /// </summary>
    [JsonPropertyName("input")]
    public required JsonElement Input { get; init; }

    /// <summary>
    /// Optional expected output for this dataset item.
    /// </summary>
    [JsonPropertyName("expectedOutput")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public JsonElement? ExpectedOutput { get; init; }

    /// <summary>
    /// Optional metadata for this dataset item.
    /// </summary>
    [JsonPropertyName("metadata")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public JsonElement? Metadata { get; init; }

    /// <summary>
    /// Optional trace ID to link this item to a source trace.
    /// </summary>
    [JsonPropertyName("sourceTraceId")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? SourceTraceId { get; init; }

    /// <summary>
    /// Optional observation ID to link this item to a source observation.
    /// </summary>
    [JsonPropertyName("sourceObservationId")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? SourceObservationId { get; init; }

    /// <summary>
    /// Optional custom ID for the dataset item.
    /// </summary>
    [JsonPropertyName("id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Id { get; init; }

    /// <summary>
    /// Status of the dataset item. Valid values: "ACTIVE", "ARCHIVED".
    /// </summary>
    [JsonPropertyName("status")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Status { get; init; }
}

