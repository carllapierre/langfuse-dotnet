using System.Text.Json.Serialization;

namespace Langfuse.Client.Datasets;

/// <summary>
/// Paginated response containing dataset items.
/// </summary>
public class PaginatedDatasetItems
{
    /// <summary>
    /// The list of dataset items for the current page.
    /// </summary>
    [JsonPropertyName("data")]
    public List<DatasetItem> Data { get; set; } = new();

    /// <summary>
    /// Pagination metadata.
    /// </summary>
    [JsonPropertyName("meta")]
    public PaginationMeta Meta { get; set; } = new();
}

/// <summary>
/// Pagination metadata for list responses.
/// </summary>
public class PaginationMeta
{
    /// <summary>
    /// The current page number (1-indexed).
    /// </summary>
    [JsonPropertyName("page")]
    public int Page { get; set; }

    /// <summary>
    /// The maximum number of items per page.
    /// </summary>
    [JsonPropertyName("limit")]
    public int Limit { get; set; }

    /// <summary>
    /// The total number of items across all pages.
    /// </summary>
    [JsonPropertyName("totalItems")]
    public int TotalItems { get; set; }

    /// <summary>
    /// The total number of pages.
    /// </summary>
    [JsonPropertyName("totalPages")]
    public int TotalPages { get; set; }

    /// <summary>
    /// Whether there are more pages after the current page.
    /// </summary>
    public bool HasNextPage => Page < TotalPages;

    /// <summary>
    /// Whether there are pages before the current page.
    /// </summary>
    public bool HasPreviousPage => Page > 1;
}

