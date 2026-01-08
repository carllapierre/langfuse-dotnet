using Langfuse.Client;
using Langfuse.Core;
using Xunit;

namespace Langfuse.IntegrationTests;

/// <summary>
/// Integration tests for Langfuse datasets.
/// Runs against Cloud (if LANGFUSE_PUBLIC_KEY/SECRET_KEY are set) or Local Docker instance.
/// </summary>
[Trait("Category", "Integration")]
public class DatasetIntegrationTests : IDisposable
{
    private readonly LangfuseClient? _client;
    private readonly bool _skipTests;
    private readonly string _environment;
    private readonly List<string> _createdDatasetNames = new();

    public DatasetIntegrationTests()
    {
        // Try Cloud first, then Local
        if (TestConfiguration.IsCloudConfigured())
        {
            _environment = "Cloud";
            var options = TestConfiguration.GetCloudOptions();
            _client = new LangfuseClient(new LangfuseClientOptions
            {
                BaseUrl = options.BaseUrl,
                PublicKey = options.PublicKey,
                SecretKey = options.SecretKey
            });
        }
        else if (TestConfiguration.IsLocalConfigured())
        {
            _environment = "Local";
            var options = TestConfiguration.GetLocalOptions();
            _client = new LangfuseClient(new LangfuseClientOptions
            {
                BaseUrl = options.BaseUrl,
                PublicKey = options.PublicKey,
                SecretKey = options.SecretKey,
                Timeout = TimeSpan.FromSeconds(10)
            });
        }
        else
        {
            _skipTests = true;
            _environment = "None";
        }
    }

    [SkippableFact]
    public async Task CreateDataset_WithValidName_ReturnsDataset()
    {
        Skip.If(_skipTests, "No Langfuse configuration found. Set LANGFUSE_PUBLIC_KEY/SECRET_KEY for cloud, or LANGFUSE_LOCAL_PUBLIC_KEY/SECRET_KEY for local.");

        var datasetName = $"integration-test-{Guid.NewGuid():N}";
        _createdDatasetNames.Add(datasetName);

        try
        {
            var dataset = await _client!.CreateDatasetAsync(
                name: datasetName,
                description: "Integration test dataset",
                metadata: new { author = "integration-test", type = "test" }
            );

            Assert.NotNull(dataset);
            Assert.Equal(datasetName, dataset.Name);
            Assert.Equal("Integration test dataset", dataset.Description);
            Assert.NotEmpty(dataset.Id);
        }
        catch (LangfuseApiException ex) when (ex.StatusCode == 401)
        {
            Assert.Fail($"Authentication failed against {_environment}. Check your API keys.");
        }
    }

    [SkippableFact]
    public async Task GetDataset_WithValidName_ReturnsDataset()
    {
        Skip.If(_skipTests, "No Langfuse configuration found.");

        var datasetName = $"integration-test-get-{Guid.NewGuid():N}";
        _createdDatasetNames.Add(datasetName);

        try
        {
            // Create first
            await _client!.CreateDatasetAsync(name: datasetName);

            // Then retrieve
            var dataset = await _client!.GetDatasetAsync(datasetName);

            Assert.NotNull(dataset);
            Assert.Equal(datasetName, dataset.Name);
        }
        catch (LangfuseApiException ex) when (ex.StatusCode == 401)
        {
            Assert.Fail($"Authentication failed against {_environment}. Check your API keys.");
        }
    }

    [SkippableFact]
    public async Task GetDataset_WithInvalidName_ThrowsNotFoundException()
    {
        Skip.If(_skipTests, "No Langfuse configuration found.");

        var exception = await Assert.ThrowsAsync<LangfuseApiException>(() =>
            _client!.GetDatasetAsync("non-existent-dataset-" + Guid.NewGuid()));

        Assert.Equal(404, exception.StatusCode);
    }

    [SkippableFact]
    public async Task CreateDatasetItem_WithInput_ReturnsItem()
    {
        Skip.If(_skipTests, "No Langfuse configuration found.");

        var datasetName = $"integration-test-items-{Guid.NewGuid():N}";
        _createdDatasetNames.Add(datasetName);

        try
        {
            // Create dataset first
            await _client!.CreateDatasetAsync(name: datasetName);

            // Create item
            var item = await _client!.CreateDatasetItemAsync(
                datasetName: datasetName,
                input: new { question = "What is 2+2?", context = "math" },
                expectedOutput: new { answer = "4" },
                metadata: new { difficulty = "easy" }
            );

            Assert.NotNull(item);
            Assert.NotEmpty(item.Id);
            Assert.Equal(datasetName, item.DatasetName);
            Assert.Equal("ACTIVE", item.Status);
        }
        catch (LangfuseApiException ex) when (ex.StatusCode == 401)
        {
            Assert.Fail($"Authentication failed against {_environment}. Check your API keys.");
        }
    }

    [SkippableFact]
    public async Task GetDatasetItems_WithPagination_ReturnsItems()
    {
        Skip.If(_skipTests, "No Langfuse configuration found.");

        var datasetName = $"integration-test-pagination-{Guid.NewGuid():N}";
        _createdDatasetNames.Add(datasetName);

        try
        {
            // Create dataset
            await _client!.CreateDatasetAsync(name: datasetName);

            // Create multiple items
            await _client!.CreateDatasetItemAsync(
                datasetName: datasetName,
                input: new { question = "Item 1" }
            );
            await _client!.CreateDatasetItemAsync(
                datasetName: datasetName,
                input: new { question = "Item 2" }
            );

            // Retrieve items
            var result = await _client!.GetItemsForDatasetAsync(
                datasetName: datasetName,
                page: 1,
                limit: 10
            );

            Assert.NotNull(result);
            Assert.Equal(2, result.Data.Count);
            Assert.Equal(1, result.Meta.Page);
        }
        catch (LangfuseApiException ex) when (ex.StatusCode == 401)
        {
            Assert.Fail($"Authentication failed against {_environment}. Check your API keys.");
        }
    }

    [SkippableFact]
    public async Task GetDatasetItem_ById_ReturnsItem()
    {
        Skip.If(_skipTests, "No Langfuse configuration found.");

        var datasetName = $"integration-test-getitem-{Guid.NewGuid():N}";
        _createdDatasetNames.Add(datasetName);

        try
        {
            // Create dataset
            await _client!.CreateDatasetAsync(name: datasetName);

            // Create item
            var createdItem = await _client!.CreateDatasetItemAsync(
                datasetName: datasetName,
                input: new { question = "Test question" }
            );

            // Retrieve by ID
            var item = await _client!.GetDatasetItemAsync(createdItem.Id);

            Assert.NotNull(item);
            Assert.Equal(createdItem.Id, item.Id);
        }
        catch (LangfuseApiException ex) when (ex.StatusCode == 401)
        {
            Assert.Fail($"Authentication failed against {_environment}. Check your API keys.");
        }
    }

    [SkippableFact]
    public async Task GetDatasets_WithPagination_ReturnsDatasets()
    {
        Skip.If(_skipTests, "No Langfuse configuration found.");

        try
        {
            // Just verify we can call the endpoint without error
            var result = await _client!.GetDatasetsAsync(page: 1, limit: 10);

            Assert.NotNull(result);
            Assert.NotNull(result.Data);
            Assert.Equal(1, result.Meta.Page);
        }
        catch (LangfuseApiException ex) when (ex.StatusCode == 401)
        {
            Assert.Fail($"Authentication failed against {_environment}. Check your API keys.");
        }
    }

    public void Dispose()
    {
        _client?.Dispose();
        // Note: Langfuse API doesn't have a delete dataset endpoint in the public API,
        // so created datasets will remain. They use unique names to avoid conflicts.
    }
}

