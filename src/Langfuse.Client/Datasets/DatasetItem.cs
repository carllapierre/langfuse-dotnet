using System.Text.Json;
using System.Text.Json.Serialization;

namespace Langfuse.Client.Datasets;

/// <summary>
/// Represents an item in a Langfuse dataset.
/// Each item contains an input and optionally an expected output for testing.
/// </summary>
public class DatasetItem
{
    /// <summary>
    /// Unique identifier for the dataset item.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// The ID of the dataset this item belongs to.
    /// </summary>
    [JsonPropertyName("datasetId")]
    public string DatasetId { get; set; } = string.Empty;

    /// <summary>
    /// The name of the dataset this item belongs to.
    /// </summary>
    [JsonPropertyName("datasetName")]
    public string DatasetName { get; set; } = string.Empty;

    /// <summary>
    /// The input data for this dataset item.
    /// Can be any JSON-serializable object.
    /// </summary>
    [JsonPropertyName("input")]
    public JsonElement Input { get; set; }

    /// <summary>
    /// The expected output for this dataset item.
    /// Can be any JSON-serializable object.
    /// </summary>
    [JsonPropertyName("expectedOutput")]
    public JsonElement? ExpectedOutput { get; set; }

    /// <summary>
    /// Arbitrary metadata associated with this item.
    /// </summary>
    [JsonPropertyName("metadata")]
    public JsonElement? Metadata { get; set; }

    /// <summary>
    /// The ID of the trace this item was created from (if any).
    /// </summary>
    [JsonPropertyName("sourceTraceId")]
    public string? SourceTraceId { get; set; }

    /// <summary>
    /// The ID of the observation this item was created from (if any).
    /// </summary>
    [JsonPropertyName("sourceObservationId")]
    public string? SourceObservationId { get; set; }

    /// <summary>
    /// Status of the dataset item. Valid values: "ACTIVE", "ARCHIVED".
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; set; } = "ACTIVE";

    /// <summary>
    /// When the item was created.
    /// </summary>
    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// When the item was last updated.
    /// </summary>
    [JsonPropertyName("updatedAt")]
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Gets the input as a strongly-typed object.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the input to.</typeparam>
    /// <returns>The deserialized input, or default if deserialization fails.</returns>
    public T? GetInput<T>()
    {
        try
        {
            return Input.Deserialize<T>();
        }
        catch
        {
            return default;
        }
    }

    /// <summary>
    /// Gets the expected output as a strongly-typed object.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the expected output to.</typeparam>
    /// <returns>The deserialized expected output, or default if null or deserialization fails.</returns>
    public T? GetExpectedOutput<T>()
    {
        if (ExpectedOutput == null || ExpectedOutput.Value.ValueKind == JsonValueKind.Null)
            return default;

        try
        {
            return ExpectedOutput.Value.Deserialize<T>();
        }
        catch
        {
            return default;
        }
    }

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

