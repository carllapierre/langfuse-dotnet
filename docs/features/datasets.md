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

