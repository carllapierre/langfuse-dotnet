using System.Text.Json.Serialization;
using Langfuse.Client.Scores;

namespace Langfuse.Client.Traces;

/// <summary>
/// Trace model with full details returned from single trace endpoint.
/// Contains complete observation and score objects.
/// </summary>
public class TraceWithFullDetails : Trace
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
    /// List of full observation objects associated with this trace.
    /// </summary>
    [JsonPropertyName("observations")]
    public List<Observation> Observations { get; set; } = new();

    /// <summary>
    /// List of full score objects associated with this trace.
    /// </summary>
    [JsonPropertyName("scores")]
    public List<Score> Scores { get; set; } = new();
}
