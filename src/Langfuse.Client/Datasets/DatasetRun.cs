using System.Text.Json;
using System.Text.Json.Serialization;

namespace Langfuse.Client.Datasets;

/// <summary>
/// Represents a dataset run in Langfuse.
/// A dataset run is an execution of your LLM application against a dataset for evaluation.
/// </summary>
public class DatasetRun
{
    /// <summary>
    /// Unique identifier for the dataset run.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Name of the dataset run (unique within a dataset).
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Optional description of the dataset run.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// The ID of the dataset this run belongs to.
    /// </summary>
    [JsonPropertyName("datasetId")]
    public string DatasetId { get; set; } = string.Empty;

    /// <summary>
    /// The name of the dataset this run belongs to.
    /// </summary>
    [JsonPropertyName("datasetName")]
    public string DatasetName { get; set; } = string.Empty;

    /// <summary>
    /// Arbitrary metadata associated with this run.
    /// </summary>
    [JsonPropertyName("metadata")]
    public JsonElement? Metadata { get; set; }

    /// <summary>
    /// When the run was created.
    /// </summary>
    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// When the run was last updated.
    /// </summary>
    [JsonPropertyName("updatedAt")]
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Gets a metadata value by key, or returns the default value if not found.
    /// </summary>
    /// <typeparam name="T">The type to convert the metadata value to.</typeparam>
    /// <param name="key">The metadata key.</param>
    /// <param name="defaultValue">The default value if key is not found.</param>
    /// <returns>The metadata value or default.</returns>
    public T? GetMetadataValue<T>(string key, T? defaultValue = default)
    {
        if (Metadata == null || Metadata.Value.ValueKind != JsonValueKind.Object)
            return defaultValue;

        if (!Metadata.Value.TryGetProperty(key, out var property))
            return defaultValue;

        try
        {
            return property.Deserialize<T>();
        }
        catch
        {
            return defaultValue;
        }
    }
}
