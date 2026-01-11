using System.Text.Json.Serialization;

namespace Langfuse.Client.Traces;

/// <summary>
/// Response from trace delete operations.
/// </summary>
public class DeleteTraceResponse
{
    /// <summary>
    /// Message confirming the deletion.
    /// </summary>
    [JsonPropertyName("message")]
    public string? Message { get; set; }
}
