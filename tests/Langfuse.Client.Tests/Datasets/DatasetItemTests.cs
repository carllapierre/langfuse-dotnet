using System.Text.Json;
using Langfuse.Client.Datasets;
using Xunit;

namespace Langfuse.Client.Tests.Datasets;

public class DatasetItemTests
{
    [Fact]
    public void DatasetItem_DeserializesFromJson()
    {
        var json = """
        {
            "id": "item-123",
            "datasetId": "dataset-456",
            "datasetName": "test-dataset",
            "input": {"question": "What is 2+2?"},
            "expectedOutput": {"answer": "4"},
            "metadata": {"difficulty": "easy"},
            "sourceTraceId": null,
            "sourceObservationId": null,
            "status": "ACTIVE",
            "createdAt": "2024-01-15T10:30:00Z",
            "updatedAt": "2024-01-15T11:00:00Z"
        }
        """;

        var item = JsonSerializer.Deserialize<DatasetItem>(json);

        Assert.NotNull(item);
        Assert.Equal("item-123", item.Id);
        Assert.Equal("dataset-456", item.DatasetId);
        Assert.Equal("test-dataset", item.DatasetName);
        Assert.Equal("ACTIVE", item.Status);
    }

    [Fact]
    public void GetInput_ReturnsTypedInput()
    {
        var json = """
        {
            "id": "item-123",
            "datasetId": "dataset-456",
            "datasetName": "test-dataset",
            "input": {"question": "What is 2+2?", "context": "math"},
            "status": "ACTIVE"
        }
        """;

        var item = JsonSerializer.Deserialize<DatasetItem>(json)!;
        var input = item.GetInput<Dictionary<string, string>>();

        Assert.NotNull(input);
        Assert.Equal("What is 2+2?", input["question"]);
        Assert.Equal("math", input["context"]);
    }

    [Fact]
    public void GetExpectedOutput_ReturnsTypedOutput()
    {
        var json = """
        {
            "id": "item-123",
            "datasetId": "dataset-456",
            "datasetName": "test-dataset",
            "input": {},
            "expectedOutput": {"answer": "4", "confidence": 0.95},
            "status": "ACTIVE"
        }
        """;

        var item = JsonSerializer.Deserialize<DatasetItem>(json)!;
        var output = item.GetExpectedOutput<Dictionary<string, object>>();

        Assert.NotNull(output);
    }

    [Fact]
    public void GetExpectedOutput_ReturnsDefaultWhenNull()
    {
        var json = """
        {
            "id": "item-123",
            "datasetId": "dataset-456",
            "datasetName": "test-dataset",
            "input": {},
            "expectedOutput": null,
            "status": "ACTIVE"
        }
        """;

        var item = JsonSerializer.Deserialize<DatasetItem>(json)!;
        var output = item.GetExpectedOutput<Dictionary<string, string>>();

        Assert.Null(output);
    }

    [Fact]
    public void GetMetadataValue_ReturnsValue()
    {
        var json = """
        {
            "id": "item-123",
            "datasetId": "dataset-456",
            "datasetName": "test-dataset",
            "input": {},
            "metadata": {"difficulty": "hard", "category": "math"},
            "status": "ACTIVE"
        }
        """;

        var item = JsonSerializer.Deserialize<DatasetItem>(json)!;

        Assert.Equal("hard", item.GetMetadataValue<string>("difficulty"));
        Assert.Equal("math", item.GetMetadataValue<string>("category"));
    }

    [Fact]
    public void GetMetadataValue_ReturnsDefaultWhenNotFound()
    {
        var json = """
        {
            "id": "item-123",
            "datasetId": "dataset-456",
            "datasetName": "test-dataset",
            "input": {},
            "metadata": {},
            "status": "ACTIVE"
        }
        """;

        var item = JsonSerializer.Deserialize<DatasetItem>(json)!;

        Assert.Equal("default", item.GetMetadataValue("missing", "default"));
    }
}

