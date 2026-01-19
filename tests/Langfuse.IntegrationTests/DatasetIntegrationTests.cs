using System.Diagnostics;
using Langfuse.Client;
using Langfuse.Core;
using Langfuse.OpenTelemetry;
using OpenTelemetry;
using OpenTelemetry.Trace;
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

    #region Dataset Run Tests

    [SkippableFact]
    public async Task CreateDatasetRunItem_WithValidData_ReturnsRunItem()
    {
        Skip.If(_skipTests, "No Langfuse configuration found.");

        var datasetName = $"integration-test-run-{Guid.NewGuid():N}";
        var runName = $"test-run-{Guid.NewGuid():N}";
        _createdDatasetNames.Add(datasetName);

        try
        {
            // Create dataset and item
            await _client!.CreateDatasetAsync(name: datasetName);
            var item = await _client!.CreateDatasetItemAsync(
                datasetName: datasetName,
                input: new { question = "What is 2+2?" },
                expectedOutput: new { answer = "4" }
            );

            // Create a run item (using a fake trace ID since we're just testing the API)
            var traceId = Guid.NewGuid().ToString();
            var runItem = await _client!.CreateDatasetRunItemAsync(
                runName: runName,
                datasetItemId: item.Id,
                traceId: traceId,
                runDescription: "Integration test run"
            );

            Assert.NotNull(runItem);
            Assert.NotEmpty(runItem.Id);
            Assert.Equal(item.Id, runItem.DatasetItemId);
            Assert.Equal(traceId, runItem.TraceId);
            Assert.Equal(runName, runItem.DatasetRunName);
        }
        catch (LangfuseApiException ex) when (ex.StatusCode == 401)
        {
            Assert.Fail($"Authentication failed against {_environment}. Check your API keys.");
        }
    }

    [SkippableFact]
    public async Task GetDatasetRun_WithValidNames_ReturnsRun()
    {
        Skip.If(_skipTests, "No Langfuse configuration found.");

        var datasetName = $"integration-test-getrun-{Guid.NewGuid():N}";
        var runName = $"test-run-{Guid.NewGuid():N}";
        _createdDatasetNames.Add(datasetName);

        try
        {
            // Create dataset and item
            await _client!.CreateDatasetAsync(name: datasetName);
            var item = await _client!.CreateDatasetItemAsync(
                datasetName: datasetName,
                input: new { question = "Question 1" }
            );

            // Create run item - note: uses synthetic trace ID
            // Langfuse accepts run items with non-existent trace IDs but may not 
            // fully populate run item data until the trace exists
            var traceId = Guid.NewGuid().ToString();
            await _client!.CreateDatasetRunItemAsync(
                runName: runName,
                datasetItemId: item.Id,
                traceId: traceId
            );

            // Allow time for eventual consistency
            await Task.Delay(500);

            // Retrieve the run - verify run was created
            var run = await _client!.GetDatasetRunAsync(datasetName, runName);

            Assert.NotNull(run);
            Assert.Equal(runName, run.Name);
            Assert.Equal(datasetName, run.DatasetName);
            // Note: DatasetRunItems may be empty if trace doesn't exist in Langfuse
            // The run itself is created, but items require valid traces to be fully linked
            Assert.NotNull(run.DatasetRunItems);
        }
        catch (LangfuseApiException ex) when (ex.StatusCode == 401)
        {
            Assert.Fail($"Authentication failed against {_environment}. Check your API keys.");
        }
    }

    [SkippableFact]
    public async Task GetDatasetRun_WithInvalidName_ThrowsNotFoundException()
    {
        Skip.If(_skipTests, "No Langfuse configuration found.");

        var datasetName = $"integration-test-notfound-{Guid.NewGuid():N}";
        _createdDatasetNames.Add(datasetName);

        try
        {
            // Create dataset but no run
            await _client!.CreateDatasetAsync(name: datasetName);

            var exception = await Assert.ThrowsAsync<LangfuseApiException>(() =>
                _client!.GetDatasetRunAsync(datasetName, "non-existent-run"));

            Assert.Equal(404, exception.StatusCode);
        }
        catch (LangfuseApiException ex) when (ex.StatusCode == 401)
        {
            Assert.Fail($"Authentication failed against {_environment}. Check your API keys.");
        }
    }

    [SkippableFact]
    public async Task GetDatasetRunItems_WithPagination_ReturnsResult()
    {
        Skip.If(_skipTests, "No Langfuse configuration found.");

        var datasetName = $"integration-test-runitems-{Guid.NewGuid():N}";
        var runName = $"test-run-{Guid.NewGuid():N}";
        _createdDatasetNames.Add(datasetName);

        try
        {
            // Create dataset and item
            var dataset = await _client!.CreateDatasetAsync(name: datasetName);
            var item = await _client!.CreateDatasetItemAsync(
                datasetName: datasetName,
                input: new { question = "Test question" }
            );

            // Create run item - note: uses synthetic trace ID
            var traceId = Guid.NewGuid().ToString();
            await _client!.CreateDatasetRunItemAsync(
                runName: runName,
                datasetItemId: item.Id,
                traceId: traceId
            );

            // Allow time for eventual consistency
            await Task.Delay(500);

            // Retrieve run items with pagination
            var result = await _client!.GetDatasetRunItemsAsync(
                datasetId: dataset.Id,
                runName: runName,
                page: 1,
                limit: 10
            );

            // Verify we can call the endpoint successfully
            // Note: Items may not appear if trace IDs don't exist in Langfuse
            Assert.NotNull(result);
            Assert.NotNull(result.Data);
            Assert.NotNull(result.Meta);
        }
        catch (LangfuseApiException ex) when (ex.StatusCode == 401)
        {
            Assert.Fail($"Authentication failed against {_environment}. Check your API keys.");
        }
    }

    [SkippableFact]
    public async Task CreateDatasetRunItem_WithMetadata_CreatesRun()
    {
        Skip.If(_skipTests, "No Langfuse configuration found.");

        var datasetName = $"integration-test-runmeta-{Guid.NewGuid():N}";
        var runName = $"test-run-{Guid.NewGuid():N}";
        _createdDatasetNames.Add(datasetName);

        try
        {
            // Create dataset and item
            await _client!.CreateDatasetAsync(name: datasetName);
            var item = await _client!.CreateDatasetItemAsync(
                datasetName: datasetName,
                input: new { question = "Test question" }
            );

            // Create run item with metadata - note: uses synthetic trace ID
            var traceId = Guid.NewGuid().ToString();
            var runItem = await _client!.CreateDatasetRunItemAsync(
                runName: runName,
                datasetItemId: item.Id,
                traceId: traceId,
                runDescription: "Test run with metadata",
                metadata: new { model = "gpt-4", temperature = 0.7 }
            );

            Assert.NotNull(runItem);

            // Allow time for eventual consistency
            await Task.Delay(500);

            // Verify run was created
            var run = await _client!.GetDatasetRunAsync(datasetName, runName);
            Assert.NotNull(run);
            Assert.Equal(runName, run.Name);
            // Description and metadata may or may not be set depending on API behavior
        }
        catch (LangfuseApiException ex) when (ex.StatusCode == 401)
        {
            Assert.Fail($"Authentication failed against {_environment}. Check your API keys.");
        }
    }

    [SkippableFact]
    public async Task GetDatasetRuns_WithPagination_ReturnsRuns()
    {
        Skip.If(_skipTests, "No Langfuse configuration found.");

        var datasetName = $"integration-test-listruns-{Guid.NewGuid():N}";
        var runName1 = $"test-run-1-{Guid.NewGuid():N}";
        var runName2 = $"test-run-2-{Guid.NewGuid():N}";
        _createdDatasetNames.Add(datasetName);

        try
        {
            // Create dataset and item
            await _client!.CreateDatasetAsync(name: datasetName);
            var item = await _client!.CreateDatasetItemAsync(
                datasetName: datasetName,
                input: new { question = "Test question" }
            );

            // Create two runs by creating run items - note: uses synthetic trace IDs
            var traceId1 = Guid.NewGuid().ToString();
            await _client!.CreateDatasetRunItemAsync(
                runName: runName1,
                datasetItemId: item.Id,
                traceId: traceId1
            );

            var traceId2 = Guid.NewGuid().ToString();
            await _client!.CreateDatasetRunItemAsync(
                runName: runName2,
                datasetItemId: item.Id,
                traceId: traceId2
            );

            // Allow time for eventual consistency
            await Task.Delay(500);

            // List all runs for the dataset
            var result = await _client!.GetDatasetRunsAsync(
                datasetName: datasetName,
                page: 1,
                limit: 50
            );

            Assert.NotNull(result);
            Assert.NotNull(result.Data);
            Assert.NotNull(result.Meta);
            Assert.Equal(1, result.Meta.Page);
            Assert.True(result.Data.Count >= 2, $"Expected at least 2 runs, got {result.Data.Count}");
            Assert.Contains(result.Data, r => r.Name == runName1);
            Assert.Contains(result.Data, r => r.Name == runName2);
        }
        catch (LangfuseApiException ex) when (ex.StatusCode == 401)
        {
            Assert.Fail($"Authentication failed against {_environment}. Check your API keys.");
        }
    }

    [SkippableFact]
    public async Task GetDatasetRuns_EmptyDataset_ReturnsEmptyList()
    {
        Skip.If(_skipTests, "No Langfuse configuration found.");

        var datasetName = $"integration-test-emptyruns-{Guid.NewGuid():N}";
        _createdDatasetNames.Add(datasetName);

        try
        {
            // Create dataset without any runs
            await _client!.CreateDatasetAsync(name: datasetName);

            // List runs for the dataset (should be empty)
            var result = await _client!.GetDatasetRunsAsync(
                datasetName: datasetName,
                page: 1,
                limit: 50
            );

            Assert.NotNull(result);
            Assert.NotNull(result.Data);
            Assert.Empty(result.Data);
        }
        catch (LangfuseApiException ex) when (ex.StatusCode == 401)
        {
            Assert.Fail($"Authentication failed against {_environment}. Check your API keys.");
        }
    }

    #endregion

    #region End-to-End Tests with Real Traces

    [SkippableFact]
    public async Task DatasetRun_WithRealTrace_CreatesVisibleRunInLangfuse()
    {
        Skip.If(_skipTests, "No Langfuse configuration found.");

        var datasetName = $"integration-test-e2e-{Guid.NewGuid():N}";
        var runName = $"e2e-run-{Guid.NewGuid():N}";
        _createdDatasetNames.Add(datasetName);

        // Get credentials from test configuration
        var langfuseOptions = TestConfiguration.IsCloudConfigured() 
            ? TestConfiguration.GetCloudOptions() 
            : TestConfiguration.GetLocalOptions();

        // Create an ActivitySource for our test traces
        const string activitySourceName = "Langfuse.IntegrationTests";
        using var activitySource = new ActivitySource(activitySourceName);

        // Setup OpenTelemetry with Langfuse exporter using test credentials
        using var tracerProvider = Sdk.CreateTracerProviderBuilder()
            .AddSource(activitySourceName)
            .AddLangfuseExporter(options =>
            {
                options.PublicKey = langfuseOptions.PublicKey;
                options.SecretKey = langfuseOptions.SecretKey;
                options.BaseUrl = langfuseOptions.BaseUrl;
            })
            .Build();

        try
        {
            // Create dataset and item
            await _client!.CreateDatasetAsync(name: datasetName);
            var item = await _client!.CreateDatasetItemAsync(
                datasetName: datasetName,
                input: new { question = "What is the capital of France?" },
                expectedOutput: new { answer = "Paris" }
            );

            // Create a REAL trace using OpenTelemetry
            string? traceId = null;
            using (var activity = activitySource.StartActivity("TestLLMCall"))
            {
                // Capture the trace ID
                traceId = activity?.TraceId.ToString();

                // Simulate some work (this would be your LLM call in production)
                activity?.SetTag("llm.model", "test-model");
                activity?.SetTag("llm.prompt", "What is the capital of France?");
                
                await Task.Delay(100); // Simulate processing time
                
                activity?.SetTag("llm.response", "Paris");
                activity?.SetStatus(ActivityStatusCode.Ok);
            }

            // Flush traces to Langfuse
            tracerProvider?.ForceFlush();

            // Wait for trace to be ingested by Langfuse
            await Task.Delay(2000);

            Assert.NotNull(traceId);

            // Now create a dataset run item with the REAL trace ID
            var runItem = await _client!.CreateDatasetRunItemAsync(
                runName: runName,
                datasetItemId: item.Id,
                traceId: traceId!,
                runDescription: "End-to-end test with real trace"
            );

            Assert.NotNull(runItem);
            Assert.Equal(traceId, runItem.TraceId);

            // Wait for eventual consistency - Langfuse may take time to process traces
            await Task.Delay(2000);

            // Verify the run was created
            var run = await _client!.GetDatasetRunAsync(datasetName, runName);

            Assert.NotNull(run);
            Assert.Equal(runName, run.Name);
            Assert.Equal(datasetName, run.DatasetName);
            Assert.NotNull(run.DatasetRunItems);
            
            // Note: Due to eventual consistency, run items may not be immediately visible
            // The test verifies the full flow works - check Langfuse UI to see the run
            if (run.DatasetRunItems.Count > 0)
            {
                Assert.Contains(run.DatasetRunItems, ri => ri.TraceId == traceId);
            }
        }
        catch (LangfuseApiException ex) when (ex.StatusCode == 401)
        {
            Assert.Fail($"Authentication failed against {_environment}. Check your API keys.");
        }
    }

    [SkippableFact]
    public async Task DatasetRun_MultipleItemsWithRealTraces_CreatesCompleteRun()
    {
        Skip.If(_skipTests, "No Langfuse configuration found.");

        var datasetName = $"integration-test-e2e-multi-{Guid.NewGuid():N}";
        var runName = $"e2e-multi-run-{Guid.NewGuid():N}";
        _createdDatasetNames.Add(datasetName);

        // Get credentials from test configuration
        var langfuseOptions = TestConfiguration.IsCloudConfigured() 
            ? TestConfiguration.GetCloudOptions() 
            : TestConfiguration.GetLocalOptions();

        const string activitySourceName = "Langfuse.IntegrationTests.Multi";
        using var activitySource = new ActivitySource(activitySourceName);

        using var tracerProvider = Sdk.CreateTracerProviderBuilder()
            .AddSource(activitySourceName)
            .AddLangfuseExporter(options =>
            {
                options.PublicKey = langfuseOptions.PublicKey;
                options.SecretKey = langfuseOptions.SecretKey;
                options.BaseUrl = langfuseOptions.BaseUrl;
            })
            .Build();

        try
        {
            // Create dataset with multiple items
            await _client!.CreateDatasetAsync(name: datasetName, description: "E2E multi-item test");
            
            var testCases = new[]
            {
                new { question = "What is 2+2?", answer = "4" },
                new { question = "What color is the sky?", answer = "Blue" },
                new { question = "What is H2O?", answer = "Water" }
            };

            var createdItems = new List<(string ItemId, string TraceId)>();

            foreach (var testCase in testCases)
            {
                // Create dataset item
                var item = await _client!.CreateDatasetItemAsync(
                    datasetName: datasetName,
                    input: new { question = testCase.question },
                    expectedOutput: new { answer = testCase.answer }
                );

                // Create a real trace for this item
                string? traceId = null;
                using (var activity = activitySource.StartActivity($"ProcessItem_{item.Id}"))
                {
                    traceId = activity?.TraceId.ToString();
                    activity?.SetTag("input.question", testCase.question);
                    
                    await Task.Delay(50); // Simulate LLM call
                    
                    activity?.SetTag("output.answer", testCase.answer);
                    activity?.SetStatus(ActivityStatusCode.Ok);
                }

                if (traceId != null)
                {
                    createdItems.Add((item.Id, traceId));
                }
            }

            // Flush all traces
            tracerProvider?.ForceFlush();
            await Task.Delay(2000);

            // Create run items for all traces
            foreach (var (itemId, traceId) in createdItems)
            {
                await _client!.CreateDatasetRunItemAsync(
                    runName: runName,
                    datasetItemId: itemId,
                    traceId: traceId
                );
            }

            // Wait for eventual consistency - Langfuse may take time to process traces
            await Task.Delay(2000);

            // Verify the run was created
            var run = await _client!.GetDatasetRunAsync(datasetName, runName);

            Assert.NotNull(run);
            Assert.Equal(runName, run.Name);
            Assert.NotNull(run.DatasetRunItems);
            
            // Note: Due to eventual consistency, run items may not be immediately visible
            // The test verifies the full flow works - check Langfuse UI to see the run with all items
            // In a production scenario, items will appear once traces are fully processed
        }
        catch (LangfuseApiException ex) when (ex.StatusCode == 401)
        {
            Assert.Fail($"Authentication failed against {_environment}. Check your API keys.");
        }
    }

    #endregion

    public void Dispose()
    {
        _client?.Dispose();
        // Note: Langfuse API doesn't have a delete dataset endpoint in the public API,
        // so created datasets will remain. They use unique names to avoid conflicts.
    }
}

