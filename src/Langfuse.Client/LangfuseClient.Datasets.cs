using System.Text.Json;
using Langfuse.Client.Datasets;
using Langfuse.Core;
using Microsoft.Extensions.Logging;

namespace Langfuse.Client;

/// <summary>
/// Partial class containing Dataset Management API methods.
/// </summary>
public partial class LangfuseClient
{
    #region Dataset Operations

    /// <summary>
    /// Creates or updates a dataset.
    /// If a dataset with the same name exists, it will be updated.
    /// </summary>
    /// <param name="name">The name of the dataset (unique within a project). Use slashes for folder organization.</param>
    /// <param name="description">Optional description of the dataset.</param>
    /// <param name="metadata">Optional metadata to associate with the dataset.</param>
    /// <param name="inputSchema">Optional JSON Schema for validating dataset item inputs.</param>
    /// <param name="expectedOutputSchema">Optional JSON Schema for validating expected outputs.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created or updated dataset.</returns>
    /// <exception cref="LangfuseApiException">Thrown when the API request fails.</exception>
    public async Task<Dataset> CreateDatasetAsync(
        string name,
        string? description = null,
        object? metadata = null,
        object? inputSchema = null,
        object? expectedOutputSchema = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(name);

        var request = new CreateDatasetRequest
        {
            Name = name,
            Description = description,
            Metadata = SerializeToElement(metadata),
            InputSchema = SerializeToElement(inputSchema),
            ExpectedOutputSchema = SerializeToElement(expectedOutputSchema)
        };

        var result = await PostAsync<CreateDatasetRequest, Dataset>(
            LangfuseConstants.DatasetsPath,
            request,
            cancellationToken);

        Logger.LogDebug("Created dataset '{Name}' with ID {Id}", name, result.Id);
        return result;
    }

    /// <summary>
    /// Gets a dataset by name.
    /// </summary>
    /// <param name="name">The name of the dataset.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The dataset.</returns>
    /// <exception cref="LangfuseApiException">Thrown when the API request fails or dataset is not found.</exception>
    public async Task<Dataset> GetDatasetAsync(
        string name,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(name);

        var encodedName = Uri.EscapeDataString(name);
        var result = await GetAsync<Dataset>(
            $"{LangfuseConstants.DatasetsPath}/{encodedName}",
            cancellationToken);

        Logger.LogDebug("Retrieved dataset '{Name}'", name);
        return result;
    }

    /// <summary>
    /// Gets all datasets with pagination.
    /// </summary>
    /// <param name="page">Page number (1-indexed). Default: 1.</param>
    /// <param name="limit">Maximum number of datasets per page. Default: 50.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paginated list of datasets.</returns>
    /// <exception cref="LangfuseApiException">Thrown when the API request fails.</exception>
    public async Task<PaginatedDatasets> GetDatasetsAsync(
        int page = 1,
        int limit = 50,
        CancellationToken cancellationToken = default)
    {
        var path = $"{LangfuseConstants.DatasetsPath}?page={page}&limit={limit}";
        var result = await GetAsync<PaginatedDatasets>(path, cancellationToken);

        Logger.LogDebug("Retrieved {Count} datasets (page {Page})", result.Data.Count, page);
        return result;
    }

    #endregion

    #region Dataset Item Operations

    /// <summary>
    /// Creates a dataset item.
    /// </summary>
    /// <param name="datasetName">The name of the dataset to add the item to.</param>
    /// <param name="input">The input data for this item.</param>
    /// <param name="expectedOutput">Optional expected output for this item.</param>
    /// <param name="metadata">Optional metadata for this item.</param>
    /// <param name="sourceTraceId">Optional trace ID to link this item to a source trace.</param>
    /// <param name="sourceObservationId">Optional observation ID to link to a source observation.</param>
    /// <param name="id">Optional custom ID for the item.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created dataset item.</returns>
    /// <exception cref="LangfuseApiException">Thrown when the API request fails.</exception>
    public async Task<DatasetItem> CreateDatasetItemAsync(
        string datasetName,
        object input,
        object? expectedOutput = null,
        object? metadata = null,
        string? sourceTraceId = null,
        string? sourceObservationId = null,
        string? id = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(datasetName);
        ArgumentNullException.ThrowIfNull(input);

        var request = new CreateDatasetItemRequest
        {
            DatasetName = datasetName,
            Input = SerializeToElement(input) ?? throw new ArgumentException("Input cannot be null", nameof(input)),
            ExpectedOutput = SerializeToElement(expectedOutput),
            Metadata = SerializeToElement(metadata),
            SourceTraceId = sourceTraceId,
            SourceObservationId = sourceObservationId,
            Id = id
        };

        var result = await PostAsync<CreateDatasetItemRequest, DatasetItem>(
            LangfuseConstants.DatasetItemsPath,
            request,
            cancellationToken);

        Logger.LogDebug("Created dataset item {Id} in dataset '{DatasetName}'", result.Id, datasetName);
        return result;
    }

    /// <summary>
    /// Gets a dataset item by ID.
    /// </summary>
    /// <param name="id">The ID of the dataset item.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The dataset item.</returns>
    /// <exception cref="LangfuseApiException">Thrown when the API request fails or item is not found.</exception>
    public async Task<DatasetItem> GetDatasetItemAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);

        var result = await GetAsync<DatasetItem>(
            $"{LangfuseConstants.DatasetItemsPath}/{id}",
            cancellationToken);

        Logger.LogDebug("Retrieved dataset item {Id}", id);
        return result;
    }

    /// <summary>
    /// Gets dataset items with filtering and pagination.
    /// </summary>
    /// <param name="datasetName">Filter by dataset name.</param>
    /// <param name="sourceTraceId">Optional filter by source trace ID.</param>
    /// <param name="sourceObservationId">Optional filter by source observation ID.</param>
    /// <param name="page">Page number (1-indexed). Default: 1.</param>
    /// <param name="limit">Maximum number of items per page. Default: 50.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paginated list of dataset items.</returns>
    /// <exception cref="LangfuseApiException">Thrown when the API request fails.</exception>
    public async Task<PaginatedDatasetItems> GetDatasetItemsAsync(
        string? datasetName = null,
        string? sourceTraceId = null,
        string? sourceObservationId = null,
        int page = 1,
        int limit = 50,
        CancellationToken cancellationToken = default)
    {
        var queryParams = new List<string>
        {
            $"page={page}",
            $"limit={limit}"
        };

        if (!string.IsNullOrEmpty(datasetName))
        {
            queryParams.Add($"datasetName={Uri.EscapeDataString(datasetName)}");
        }

        if (!string.IsNullOrEmpty(sourceTraceId))
        {
            queryParams.Add($"sourceTraceId={Uri.EscapeDataString(sourceTraceId)}");
        }

        if (!string.IsNullOrEmpty(sourceObservationId))
        {
            queryParams.Add($"sourceObservationId={Uri.EscapeDataString(sourceObservationId)}");
        }

        var path = $"{LangfuseConstants.DatasetItemsPath}?{string.Join("&", queryParams)}";
        var result = await GetAsync<PaginatedDatasetItems>(path, cancellationToken);

        Logger.LogDebug("Retrieved {Count} dataset items (page {Page})", result.Data.Count, page);
        return result;
    }

    /// <summary>
    /// Gets all items for a specific dataset with pagination.
    /// This is a convenience method that filters by dataset name.
    /// </summary>
    /// <param name="datasetName">The name of the dataset.</param>
    /// <param name="page">Page number (1-indexed). Default: 1.</param>
    /// <param name="limit">Maximum number of items per page. Default: 50.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paginated list of dataset items.</returns>
    /// <exception cref="LangfuseApiException">Thrown when the API request fails.</exception>
    public Task<PaginatedDatasetItems> GetItemsForDatasetAsync(
        string datasetName,
        int page = 1,
        int limit = 50,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(datasetName);
        return GetDatasetItemsAsync(datasetName, null, null, page, limit, cancellationToken);
    }

    #endregion

    #region Helper Methods

    private static JsonElement? SerializeToElement(object? value)
    {
        if (value == null)
            return null;

        // If it's already a JsonElement, return it directly
        if (value is JsonElement element)
            return element;

        var json = JsonSerializer.Serialize(value, JsonOptions);
        using var doc = JsonDocument.Parse(json);
        return doc.RootElement.Clone();
    }

    #endregion
}

