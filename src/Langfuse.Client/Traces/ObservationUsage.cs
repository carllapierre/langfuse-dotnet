using System.Text.Json.Serialization;

namespace Langfuse.Client.Traces;

/// <summary>
/// Token usage details for an observation.
/// </summary>
public class ObservationUsage
{
    /// <summary>
    /// Input token count.
    /// </summary>
    [JsonPropertyName("input")]
    public int? Input { get; set; }

    /// <summary>
    /// Output token count.
    /// </summary>
    [JsonPropertyName("output")]
    public int? Output { get; set; }

    /// <summary>
    /// Total token count.
    /// </summary>
    [JsonPropertyName("total")]
    public int? Total { get; set; }

    /// <summary>
    /// Input cost in USD.
    /// </summary>
    [JsonPropertyName("inputCost")]
    public double? InputCost { get; set; }

    /// <summary>
    /// Output cost in USD.
    /// </summary>
    [JsonPropertyName("outputCost")]
    public double? OutputCost { get; set; }

    /// <summary>
    /// Total cost in USD.
    /// </summary>
    [JsonPropertyName("totalCost")]
    public double? TotalCost { get; set; }

    /// <summary>
    /// The unit used for token counting.
    /// </summary>
    [JsonPropertyName("unit")]
    public string? Unit { get; set; }
}
