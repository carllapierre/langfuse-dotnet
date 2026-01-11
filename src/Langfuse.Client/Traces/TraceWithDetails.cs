using System.Text.Json.Serialization;

namespace Langfuse.Client.Traces;

/// <summary>
/// Trace model with additional details returned from list endpoint.
/// Contains observation and score IDs rather than full objects.
/// </summary>
public class TraceWithDetails : Trace
{
    /// <summary>
    /// Path of trace in Langfuse UI.
    /// </summary>
    [JsonPropertyName("htmlPath")]
    public string? HtmlPath { get; set; }

    /// <summary>
    /// Latency of trace in seconds.
    /// </summary>
    [JsonPropertyName("latency")]
    public double Latency { get; set; }

    /// <summary>
    /// Cost of trace in USD.
    /// </summary>
    [JsonPropertyName("totalCost")]
    public double TotalCost { get; set; }

    /// <summary>
    /// List of observation IDs associated with this trace.
    /// </summary>
    [JsonPropertyName("observations")]
    public List<string> Observations { get; set; } = new();

    /// <summary>
    /// List of score IDs associated with this trace.
    /// </summary>
    [JsonPropertyName("scores")]
    public List<string> Scores { get; set; } = new();
}
