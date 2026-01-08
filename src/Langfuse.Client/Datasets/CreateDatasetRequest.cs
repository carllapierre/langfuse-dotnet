using System.Text.Json;
using System.Text.Json.Serialization;

namespace Langfuse.Client.Datasets;

/// <summary>
/// Request model for creating or updating a dataset in Langfuse.
/// </summary>
internal class CreateDatasetRequest
{
    /// <summary>
    /// The name of the dataset (unique within a project).
    /// Use slashes to organize datasets into folders (e.g., "evaluation/qa-dataset").
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    /// Optional description of the dataset.
    /// </summary>
    [JsonPropertyName("description")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Description { get; init; }

    /// <summary>
    /// Arbitrary metadata to associate with the dataset.
    /// </summary>
    [JsonPropertyName("metadata")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public JsonElement? Metadata { get; init; }

    /// <summary>
    /// JSON Schema for validating dataset item inputs.
    /// </summary>
    [JsonPropertyName("inputSchema")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public JsonElement? InputSchema { get; init; }

    /// <summary>
    /// JSON Schema for validating dataset item expected outputs.
    /// </summary>
    [JsonPropertyName("expectedOutputSchema")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public JsonElement? ExpectedOutputSchema { get; init; }
}

