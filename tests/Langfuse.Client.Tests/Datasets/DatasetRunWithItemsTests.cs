using System.Text.Json;
using Langfuse.Client.Datasets;
using Xunit;

namespace Langfuse.Client.Tests.Datasets;

public class DatasetRunWithItemsTests
{
    [Fact]
    public void DatasetRunWithItems_DeserializesFromJson()
    {
        var json = """
        {
            "id": "run-123",
            "name": "experiment-v1",
            "description": "First experiment run",
            "datasetId": "dataset-456",
            "datasetName": "qa-benchmark",
            "metadata": {"model": "gpt-4"},
            "createdAt": "2024-01-15T10:30:00Z",
            "updatedAt": "2024-01-15T11:00:00Z",
            "datasetRunItems": [
                {
                    "id": "run-item-1",
                    "datasetRunId": "run-123",
                    "datasetRunName": "experiment-v1",
                    "datasetItemId": "item-1",
                    "traceId": "trace-1",
                    "observationId": null,
                    "createdAt": "2024-01-15T10:30:00Z",
                    "updatedAt": "2024-01-15T10:30:00Z"
                },
                {
                    "id": "run-item-2",
                    "datasetRunId": "run-123",
                    "datasetRunName": "experiment-v1",
                    "datasetItemId": "item-2",
                    "traceId": "trace-2",
                    "observationId": "obs-2",
                    "createdAt": "2024-01-15T10:35:00Z",
                    "updatedAt": "2024-01-15T10:35:00Z"
                }
            ]
        }
        """;

        var run = JsonSerializer.Deserialize<DatasetRunWithItems>(json);

        Assert.NotNull(run);
        Assert.Equal("run-123", run.Id);
        Assert.Equal("experiment-v1", run.Name);
        Assert.Equal("First experiment run", run.Description);
        Assert.Equal("dataset-456", run.DatasetId);
        Assert.Equal("qa-benchmark", run.DatasetName);
        Assert.NotNull(run.Metadata);
        Assert.Equal(2, run.DatasetRunItems.Count);

        Assert.Equal("run-item-1", run.DatasetRunItems[0].Id);
        Assert.Equal("item-1", run.DatasetRunItems[0].DatasetItemId);
        Assert.Equal("trace-1", run.DatasetRunItems[0].TraceId);
        Assert.Null(run.DatasetRunItems[0].ObservationId);

        Assert.Equal("run-item-2", run.DatasetRunItems[1].Id);
        Assert.Equal("item-2", run.DatasetRunItems[1].DatasetItemId);
        Assert.Equal("trace-2", run.DatasetRunItems[1].TraceId);
        Assert.Equal("obs-2", run.DatasetRunItems[1].ObservationId);
    }

    [Fact]
    public void DatasetRunWithItems_DeserializesEmptyItems()
    {
        var json = """
        {
            "id": "run-123",
            "name": "experiment-v1",
            "datasetId": "dataset-456",
            "datasetName": "qa-benchmark",
            "createdAt": "2024-01-15T10:30:00Z",
            "updatedAt": "2024-01-15T11:00:00Z",
            "datasetRunItems": []
        }
        """;

        var run = JsonSerializer.Deserialize<DatasetRunWithItems>(json);

        Assert.NotNull(run);
        Assert.Empty(run.DatasetRunItems);
    }

    [Fact]
    public void DatasetRunWithItems_GetMetadataValue_ReturnsValue()
    {
        var json = """
        {
            "id": "run-123",
            "name": "experiment-v1",
            "metadata": {"model": "gpt-4", "temperature": 0.7},
            "datasetRunItems": []
        }
        """;

        var run = JsonSerializer.Deserialize<DatasetRunWithItems>(json)!;

        Assert.Equal("gpt-4", run.GetMetadataValue<string>("model"));
        Assert.Equal(0.7, run.GetMetadataValue<double>("temperature"));
    }

    [Fact]
    public void DatasetRunWithItems_GetMetadataValue_ReturnsDefaultWhenNotFound()
    {
        var json = """
        {
            "id": "run-123",
            "name": "experiment-v1",
            "metadata": {},
            "datasetRunItems": []
        }
        """;

        var run = JsonSerializer.Deserialize<DatasetRunWithItems>(json)!;

        Assert.Null(run.GetMetadataValue<string>("missing"));
        Assert.Equal("default", run.GetMetadataValue("missing", "default"));
    }
}
