using System.Text.Json.Serialization;

namespace Langfuse.Client.Datasets;

/// <summary>
/// Paginated response containing datasets.
/// </summary>
public class PaginatedDatasets
{
    /// <summary>
    /// The list of datasets for the current page.
    /// </summary>
    [JsonPropertyName("data")]
    public List<Dataset> Data { get; set; } = new();

    /// <summary>
    /// Pagination metadata.
    /// </summary>
    [JsonPropertyName("meta")]
    public PaginationMeta Meta { get; set; } = new();
}

