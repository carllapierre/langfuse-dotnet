using System.Text.Json.Serialization;

namespace Langfuse.Core;

/// <summary>
/// Generic paginated response wrapper for list API endpoints.
/// </summary>
/// <typeparam name="T">The type of items in the response.</typeparam>
public class PaginatedResponse<T>
{
    /// <summary>
    /// The list of items for the current page.
    /// </summary>
    [JsonPropertyName("data")]
    public List<T> Data { get; set; } = new();

    /// <summary>
    /// Pagination metadata.
    /// </summary>
    [JsonPropertyName("meta")]
    public PaginationMeta Meta { get; set; } = new();
}
