using System.Text.Json.Serialization;

namespace Langfuse.Client.Traces;

/// <summary>
/// Base trace model containing core trace properties.
/// </summary>
public class Trace
{
    /// <summary>
    /// The unique identifier of the trace.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// The name of the trace.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// The timestamp when the trace was created.
    /// </summary>
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// The user identifier associated with the trace.
    /// </summary>
    [JsonPropertyName("userId")]
    public string? UserId { get; set; }

    /// <summary>
    /// The session identifier associated with the trace.
    /// </summary>
    [JsonPropertyName("sessionId")]
    public string? SessionId { get; set; }

    /// <summary>
    /// The release version of the application when the trace was created.
    /// </summary>
    [JsonPropertyName("release")]
    public string? Release { get; set; }

    /// <summary>
    /// The version of the trace.
    /// </summary>
    [JsonPropertyName("version")]
    public string? Version { get; set; }

    /// <summary>
    /// The environment from which this trace originated.
    /// </summary>
    [JsonPropertyName("environment")]
    public string? Environment { get; set; }

    /// <summary>
    /// The tags associated with the trace.
    /// </summary>
    [JsonPropertyName("tags")]
    public List<string>? Tags { get; set; }

    /// <summary>
    /// The metadata associated with the trace. Can be any JSON.
    /// </summary>
    [JsonPropertyName("metadata")]
    public object? Metadata { get; set; }

    /// <summary>
    /// The input data of the trace. Can be any JSON.
    /// </summary>
    [JsonPropertyName("input")]
    public object? Input { get; set; }

    /// <summary>
    /// The output data of the trace. Can be any JSON.
    /// </summary>
    [JsonPropertyName("output")]
    public object? Output { get; set; }

    /// <summary>
    /// Whether the trace is publicly accessible via URL without login.
    /// </summary>
    [JsonPropertyName("public")]
    public bool? Public { get; set; }
}
