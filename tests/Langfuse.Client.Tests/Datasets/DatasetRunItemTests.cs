using System.Text.Json;
using Langfuse.Client.Datasets;
using Xunit;

namespace Langfuse.Client.Tests.Datasets;

public class DatasetRunItemTests
{
    [Fact]
    public void DatasetRunItem_DeserializesFromJson()
    {
        var json = """
        {
            "id": "run-item-123",
            "datasetRunId": "run-456",
            "datasetRunName": "experiment-v1",
            "datasetItemId": "item-789",
            "traceId": "trace-abc",
            "observationId": "obs-def",
            "createdAt": "2024-01-15T10:30:00Z",
            "updatedAt": "2024-01-15T11:00:00Z"
        }
        """;

        var runItem = JsonSerializer.Deserialize<DatasetRunItem>(json);

        Assert.NotNull(runItem);
        Assert.Equal("run-item-123", runItem.Id);
        Assert.Equal("run-456", runItem.DatasetRunId);
        Assert.Equal("experiment-v1", runItem.DatasetRunName);
        Assert.Equal("item-789", runItem.DatasetItemId);
        Assert.Equal("trace-abc", runItem.TraceId);
        Assert.Equal("obs-def", runItem.ObservationId);
    }

    [Fact]
    public void DatasetRunItem_DeserializesWithNullObservationId()
    {
        var json = """
        {
            "id": "run-item-123",
            "datasetRunId": "run-456",
            "datasetRunName": "experiment-v1",
            "datasetItemId": "item-789",
            "traceId": "trace-abc",
            "observationId": null,
            "createdAt": "2024-01-15T10:30:00Z",
            "updatedAt": "2024-01-15T11:00:00Z"
        }
        """;

        var runItem = JsonSerializer.Deserialize<DatasetRunItem>(json);

        Assert.NotNull(runItem);
        Assert.Equal("trace-abc", runItem.TraceId);
        Assert.Null(runItem.ObservationId);
    }
}
