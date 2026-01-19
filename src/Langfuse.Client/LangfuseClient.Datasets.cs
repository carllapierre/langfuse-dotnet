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
    public async Task<PaginatedResponse<Dataset>> GetDatasetsAsync(
        int page = 1,
        int limit = 50,
        CancellationToken cancellationToken = default)
    {
        var path = $"{LangfuseConstants.DatasetsPath}?page={page}&limit={limit}";
        var result = await GetAsync<PaginatedResponse<Dataset>>(path, cancellationToken);

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
    public async Task<PaginatedResponse<DatasetItem>> GetDatasetItemsAsync(
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
        var result = await GetAsync<PaginatedResponse<DatasetItem>>(path, cancellationToken);

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
    public Task<PaginatedResponse<DatasetItem>> GetItemsForDatasetAsync(
        string datasetName,
        int page = 1,
        int limit = 50,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(datasetName);
        return GetDatasetItemsAsync(datasetName, null, null, page, limit, cancellationToken);
    }

    #endregion

    #region Dataset Run Operations

    /// <summary>
    /// Creates a dataset run item, linking a trace to a dataset item within a run.
    /// If a run with the specified name doesn't exist, it will be created automatically.
    /// </summary>
    /// <param name="runName">The name of the run. Creates the run if it doesn't exist.</param>
    /// <param name="datasetItemId">The ID of the dataset item to link.</param>
    /// <param name="traceId">The ID of the trace to link.</param>
    /// <param name="observationId">Optional observation ID to link to a specific span/generation.</param>
    /// <param name="runDescription">Optional description for the run. Updates the run if it exists.</param>
    /// <param name="metadata">Optional metadata for the run. Updates the run if it exists.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created dataset run item.</returns>
    /// <exception cref="LangfuseApiException">Thrown when the API request fails.</exception>
    public async Task<DatasetRunItem> CreateDatasetRunItemAsync(
        string runName,
        string datasetItemId,
        string traceId,
        string? observationId = null,
        string? runDescription = null,
        object? metadata = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(runName);
        ArgumentNullException.ThrowIfNull(datasetItemId);
        ArgumentNullException.ThrowIfNull(traceId);

        var request = new CreateDatasetRunItemRequest
        {
            RunName = runName,
            DatasetItemId = datasetItemId,
            TraceId = traceId,
            ObservationId = observationId,
            RunDescription = runDescription,
            Metadata = SerializeToElement(metadata)
        };

        var result = await PostAsync<CreateDatasetRunItemRequest, DatasetRunItem>(
            LangfuseConstants.DatasetRunItemsPath,
            request,
            cancellationToken);

        Logger.LogDebug("Created dataset run item {Id} in run '{RunName}' for item {DatasetItemId}",
            result.Id, runName, datasetItemId);
        return result;
    }

    /// <summary>
    /// Gets a dataset run by dataset name and run name.
    /// </summary>
    /// <param name="datasetName">The name of the dataset.</param>
    /// <param name="runName">The name of the run.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The dataset run with its items.</returns>
    /// <exception cref="LangfuseApiException">Thrown when the API request fails or run is not found.</exception>
    public async Task<DatasetRunWithItems> GetDatasetRunAsync(
        string datasetName,
        string runName,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(datasetName);
        ArgumentNullException.ThrowIfNull(runName);

        var encodedDatasetName = Uri.EscapeDataString(datasetName);
        var encodedRunName = Uri.EscapeDataString(runName);
        var path = $"{LangfuseConstants.DatasetRunsBasePath}/{encodedDatasetName}/runs/{encodedRunName}";

        var result = await GetAsync<DatasetRunWithItems>(path, cancellationToken);

        Logger.LogDebug("Retrieved dataset run '{RunName}' for dataset '{DatasetName}' with {ItemCount} items",
            runName, datasetName, result.DatasetRunItems.Count);
        return result;
    }

    /// <summary>
    /// Gets all runs for a dataset with pagination.
    /// </summary>
    /// <param name="datasetName">The name of the dataset.</param>
    /// <param name="page">Page number (1-indexed). Default: 1.</param>
    /// <param name="limit">Maximum number of runs per page. Default: 50.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paginated list of dataset runs.</returns>
    /// <exception cref="LangfuseApiException">Thrown when the API request fails.</exception>
    public async Task<PaginatedResponse<DatasetRun>> GetDatasetRunsAsync(
        string datasetName,
        int page = 1,
        int limit = 50,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(datasetName);

        var encodedDatasetName = Uri.EscapeDataString(datasetName);
        var path = $"{LangfuseConstants.DatasetRunsBasePath}/{encodedDatasetName}/runs?page={page}&limit={limit}";

        var result = await GetAsync<PaginatedResponse<DatasetRun>>(path, cancellationToken);

        Logger.LogDebug("Retrieved {Count} dataset runs for dataset '{DatasetName}' (page {Page})",
            result.Data.Count, datasetName, page);
        return result;
    }

    /// <summary>
    /// Gets dataset run items with pagination.
    /// </summary>
    /// <param name="datasetId">The ID of the dataset.</param>
    /// <param name="runName">The name of the run.</param>
    /// <param name="page">Page number (1-indexed). Default: 1.</param>
    /// <param name="limit">Maximum number of items per page. Default: 50.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paginated list of dataset run items.</returns>
    /// <exception cref="LangfuseApiException">Thrown when the API request fails.</exception>
    public async Task<PaginatedResponse<DatasetRunItem>> GetDatasetRunItemsAsync(
        string datasetId,
        string runName,
        int page = 1,
        int limit = 50,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(datasetId);
        ArgumentNullException.ThrowIfNull(runName);

        var queryParams = new List<string>
        {
            $"datasetId={Uri.EscapeDataString(datasetId)}",
            $"runName={Uri.EscapeDataString(runName)}",
            $"page={page}",
            $"limit={limit}"
        };

        var path = $"{LangfuseConstants.DatasetRunItemsPath}?{string.Join("&", queryParams)}";
        var result = await GetAsync<PaginatedResponse<DatasetRunItem>>(path, cancellationToken);

        Logger.LogDebug("Retrieved {Count} dataset run items for run '{RunName}' (page {Page})",
            result.Data.Count, runName, page);
        return result;
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

