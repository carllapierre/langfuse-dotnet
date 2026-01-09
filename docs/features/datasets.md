# Dataset Management

Manage datasets for testing, evaluation, and benchmarking your LLM applications.

## Langfuse Docs

- [Datasets Overview](https://langfuse.com/docs/evaluation/features/datasets)
- [Experiments via SDK](https://langfuse.com/docs/evaluation/experiments/experiments-via-sdk)
- [API Reference](https://api.reference.langfuse.com)

## Features

| Feature | Status | Notes |
|---------|--------|-------|
| Create datasets | Supported | `CreateDatasetAsync()` with metadata and description |
| Get dataset | Supported | `GetDatasetAsync()` by name |
| List datasets | Supported | `GetDatasetsAsync()` with pagination |
| Create dataset items | Supported | `CreateDatasetItemAsync()` with input/output pairs |
| List dataset items | Supported | `GetDatasetItemsAsync()` with filtering and pagination |
| Get dataset item | Supported | `GetDatasetItemAsync()` by ID |
| JSON Schema validation | Supported | Input/output schemas on datasets |
| Folder organization | Supported | Use slashes in names (e.g., "evaluation/qa-dataset") |
| Link to traces | Supported | `sourceTraceId` and `sourceObservationId` |
| **Dataset runs** | Supported | `CreateDatasetRunItemAsync()` to link traces to items |
| **Get dataset run** | Supported | `GetDatasetRunAsync()` by dataset and run name |
| **List run items** | Supported | `GetDatasetRunItemsAsync()` with pagination |

## Usage

### Basic Setup

```csharp
using Langfuse.Client;

var client = new LangfuseClient();

// Dataset methods are available directly on the client
// e.g., client.CreateDatasetAsync(), client.GetDatasetAsync(), etc.
```

### Creating a Dataset

```csharp
var dataset = await client.CreateDatasetAsync(
    name: "qa-benchmark",
    description: "QA testing dataset for production evaluation",
    metadata: new { author = "team", version = "1.0", type = "benchmark" }
);

Console.WriteLine($"Created dataset: {dataset.Id}");
```

### Creating a Dataset with Schema Validation

```csharp
var dataset = await client.CreateDatasetAsync(
    name: "chat-evaluation",
    description: "Chat model evaluation dataset",
    inputSchema: new
    {
        type = "object",
        properties = new
        {
            messages = new
            {
                type = "array",
                items = new
                {
                    type = "object",
                    properties = new
                    {
                        role = new { type = "string" },
                        content = new { type = "string" }
                    },
                    required = new[] { "role", "content" }
                }
            }
        },
        required = new[] { "messages" }
    },
    expectedOutputSchema: new
    {
        type = "object",
        properties = new
        {
            response = new { type = "string" }
        },
        required = new[] { "response" }
    }
);
```

### Adding Items to a Dataset

```csharp
// Add a test case
var item = await client.CreateDatasetItemAsync(
    datasetName: "qa-benchmark",
    input: new { question = "What is Langfuse?", context = "LLM observability" },
    expectedOutput: new { answer = "Langfuse is an open-source LLM engineering platform." },
    metadata: new { difficulty = "easy", category = "definition" }
);

Console.WriteLine($"Created item: {item.Id}");
```

### Creating Items from Production Traces

```csharp
// Link a dataset item to a production trace
var item = await client.CreateDatasetItemAsync(
    datasetName: "qa-benchmark",
    input: new { question = "Complex question from production" },
    expectedOutput: new { answer = "Expert-provided answer" },
    sourceTraceId: "trace-abc123",
    sourceObservationId: "observation-xyz789"
);
```

### Retrieving a Dataset

```csharp
var dataset = await client.GetDatasetAsync("qa-benchmark");

Console.WriteLine($"Dataset: {dataset.Name}");
Console.WriteLine($"Description: {dataset.Description}");
Console.WriteLine($"Created: {dataset.CreatedAt}");
```

### Listing All Datasets

```csharp
var result = await client.GetDatasetsAsync(page: 1, limit: 50);

foreach (var dataset in result.Data)
{
    Console.WriteLine($"- {dataset.Name}: {dataset.Description}");
}

Console.WriteLine($"Total: {result.Meta.TotalItems} datasets");
```

### Retrieving Dataset Items

```csharp
// Get all items for a dataset
var items = await client.GetItemsForDatasetAsync(
    datasetName: "qa-benchmark",
    page: 1,
    limit: 100
);

foreach (var item in items.Data)
{
    var input = item.GetInput<Dictionary<string, string>>();
    Console.WriteLine($"Question: {input?["question"]}");
}

// Check pagination
if (items.Meta.HasNextPage)
{
    var nextPage = await client.GetItemsForDatasetAsync(
        datasetName: "qa-benchmark",
        page: 2,
        limit: 100
    );
}
```

### Filtering Dataset Items

```csharp
// Filter by source trace
var items = await client.GetDatasetItemsAsync(
    datasetName: "qa-benchmark",
    sourceTraceId: "trace-abc123"
);

// Filter by source observation
var items = await client.GetDatasetItemsAsync(
    sourceObservationId: "observation-xyz789"
);
```

## Running Experiments

Dataset runs allow you to execute your LLM application against dataset items and track the results. Each run item links a trace (from your LLM execution) to a dataset item.

### Basic Experiment Flow

```csharp
using Langfuse.Client;
using Langfuse.OpenTelemetry;
using OpenTelemetry;
using OpenTelemetry.Trace;
using System.Diagnostics;

// Setup OpenTelemetry with Langfuse exporter
using var tracerProvider = Sdk.CreateTracerProviderBuilder()
    .AddSource("MyApp")
    .AddLangfuseExporter()
    .Build();

var activitySource = new ActivitySource("MyApp");
var client = new LangfuseClient();

// Get dataset items
var items = await client.GetItemsForDatasetAsync("qa-benchmark");

foreach (var item in items.Data)
{
    var input = item.GetInput<QuestionInput>();

    // Run your LLM application (creates a trace via OTEL)
    using var activity = activitySource.StartActivity("LLM Call");
    var traceId = activity?.TraceId.ToString();

    // Your LLM logic here...
    var result = await RunMyLlmApp(input.Question);

    activity?.Stop();

    // Link the trace to the dataset item
    if (traceId != null)
    {
        await client.CreateDatasetRunItemAsync(
            runName: "experiment-v1",
            datasetItemId: item.Id,
            traceId: traceId,
            runDescription: "Testing GPT-4 on QA benchmark",
            metadata: new { model = "gpt-4", temperature = 0.7 }
        );
    }
}
```

### Creating Dataset Run Items

Link a trace to a dataset item within a run:

```csharp
var runItem = await client.CreateDatasetRunItemAsync(
    runName: "experiment-v1",           // Run name (creates if doesn't exist)
    datasetItemId: item.Id,             // Dataset item to link
    traceId: "trace-abc123",            // Trace ID from your OTEL instrumentation
    observationId: "obs-xyz789",        // Optional: link to specific span
    runDescription: "GPT-4 evaluation", // Optional: updates run description
    metadata: new { model = "gpt-4" }   // Optional: updates run metadata
);

Console.WriteLine($"Created run item: {runItem.Id}");
Console.WriteLine($"Run: {runItem.DatasetRunName}");
```

### Retrieving a Dataset Run

Get a run with all its items:

```csharp
var run = await client.GetDatasetRunAsync(
    datasetName: "qa-benchmark",
    runName: "experiment-v1"
);

Console.WriteLine($"Run: {run.Name}");
Console.WriteLine($"Description: {run.Description}");
Console.WriteLine($"Items: {run.DatasetRunItems.Count}");

foreach (var runItem in run.DatasetRunItems)
{
    Console.WriteLine($"  - Item {runItem.DatasetItemId} -> Trace {runItem.TraceId}");
}
```

### Listing Run Items with Pagination

```csharp
// First get the dataset to obtain its ID
var dataset = await client.GetDatasetAsync("qa-benchmark");

// Then list run items
var result = await client.GetDatasetRunItemsAsync(
    datasetId: dataset.Id,
    runName: "experiment-v1",
    page: 1,
    limit: 50
);

foreach (var runItem in result.Data)
{
    Console.WriteLine($"Item: {runItem.DatasetItemId}");
    Console.WriteLine($"Trace: {runItem.TraceId}");
    Console.WriteLine($"Created: {runItem.CreatedAt}");
}

Console.WriteLine($"Total: {result.Meta.TotalItems} items");
```

### Comparing Multiple Runs

```csharp
// Run experiment with different configurations
var configs = new[]
{
    ("gpt-4-run", "gpt-4", 0.7),
    ("gpt-3.5-run", "gpt-3.5-turbo", 0.7),
    ("gpt-4-creative", "gpt-4", 1.0)
};

foreach (var (runName, model, temperature) in configs)
{
    var items = await client.GetItemsForDatasetAsync("qa-benchmark");

    foreach (var item in items.Data)
    {
        // Run with specific config
        var (result, traceId) = await RunLlmWithConfig(item, model, temperature);

        await client.CreateDatasetRunItemAsync(
            runName: runName,
            datasetItemId: item.Id,
            traceId: traceId,
            metadata: new { model, temperature }
        );
    }
}

// View results in Langfuse dashboard
```

### Getting a Specific Item

```csharp
var item = await client.GetDatasetItemAsync("item-id-123");

// Access typed data
var input = item.GetInput<MyInputType>();
var output = item.GetExpectedOutput<MyOutputType>();
var difficulty = item.GetMetadataValue<string>("difficulty");
```

## Dataset Folders

Organize datasets into virtual folders using slashes in the name:

```csharp
// Creates a dataset in the "evaluation" folder
await client.CreateDatasetAsync(name: "evaluation/qa-dataset");

// Creates a dataset in a nested folder
await client.CreateDatasetAsync(name: "evaluation/v2/benchmark");
```

## Working with Typed Data

The `DatasetItem` class provides helper methods for accessing typed data:

```csharp
public class QuestionInput
{
    public string Question { get; set; }
    public string Context { get; set; }
}

public class AnswerOutput
{
    public string Answer { get; set; }
    public double Confidence { get; set; }
}

// Retrieve and use typed data
var item = await client.GetDatasetItemAsync("item-123");
var input = item.GetInput<QuestionInput>();
var expected = item.GetExpectedOutput<AnswerOutput>();

Console.WriteLine($"Q: {input?.Question}");
Console.WriteLine($"Expected A: {expected?.Answer}");
```

## Error Handling

```csharp
try
{
    var dataset = await client.GetDatasetAsync("non-existent");
}
catch (LangfuseApiException ex) when (ex.StatusCode == 404)
{
    Console.WriteLine("Dataset not found");
}
catch (LangfuseApiException ex) when (ex.StatusCode == 401)
{
    Console.WriteLine("Authentication failed - check your API keys");
}
```

## Best Practices

1. **Use descriptive names**: Include the purpose and version in dataset names
2. **Add metadata**: Track author, creation date, and dataset type
3. **Use folders**: Organize related datasets using slash notation
4. **Link to traces**: Connect dataset items to production traces for context
5. **Use schema validation**: Define input/output schemas for data consistency

