using System.Text.Json;
using System.Text.Json.Serialization;

namespace Langfuse.Client.Datasets;

/// <summary>
/// Represents a Langfuse dataset.
/// Datasets are collections of inputs and expected outputs used to test your application.
/// </summary>
public class Dataset
{
    /// <summary>
    /// Unique identifier for the dataset.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// The name of the dataset (unique within a project).
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Optional description of the dataset.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// The project ID this dataset belongs to.
    /// </summary>
    [JsonPropertyName("projectId")]
    public string ProjectId { get; set; } = string.Empty;

    /// <summary>
    /// Arbitrary metadata associated with the dataset.
    /// </summary>
    [JsonPropertyName("metadata")]
    public JsonElement? Metadata { get; set; }

    /// <summary>
    /// JSON Schema for validating dataset item inputs.
    /// </summary>
    [JsonPropertyName("inputSchema")]
    public JsonElement? InputSchema { get; set; }

    /// <summary>
    /// JSON Schema for validating dataset item expected outputs.
    /// </summary>
    [JsonPropertyName("expectedOutputSchema")]
    public JsonElement? ExpectedOutputSchema { get; set; }

    /// <summary>
    /// When the dataset was created.
    /// </summary>
    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// When the dataset was last updated.
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

