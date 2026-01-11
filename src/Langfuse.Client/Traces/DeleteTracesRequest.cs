using System.Text.Json.Serialization;

namespace Langfuse.Client.Traces;

/// <summary>
/// Request body for deleting multiple traces.
/// </summary>
internal class DeleteTracesRequest
{
    /// <summary>
    /// List of trace IDs to delete.
    /// </summary>
    [JsonPropertyName("traceIds")]
    public List<string> TraceIds { get; set; } = new();
}
