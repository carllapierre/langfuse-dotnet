using System.Text.Json;
using Langfuse.Client.Datasets;
using Xunit;

namespace Langfuse.Client.Tests.Datasets;

public class DatasetRunTests
{
    [Fact]
    public void DatasetRun_DeserializesFromJson()
    {
        var json = """
        {
            "id": "run-123",
            "name": "experiment-v1",
            "description": "First experiment run",
            "datasetId": "dataset-456",
            "datasetName": "qa-benchmark",
            "metadata": {"model": "gpt-4", "temperature": 0.7},
            "createdAt": "2024-01-15T10:30:00Z",
            "updatedAt": "2024-01-15T11:00:00Z"
        }
        """;

        var run = JsonSerializer.Deserialize<DatasetRun>(json);

        Assert.NotNull(run);
        Assert.Equal("run-123", run.Id);
        Assert.Equal("experiment-v1", run.Name);
        Assert.Equal("First experiment run", run.Description);
        Assert.Equal("dataset-456", run.DatasetId);
        Assert.Equal("qa-benchmark", run.DatasetName);
        Assert.NotNull(run.Metadata);
    }

    [Fact]
    public void DatasetRun_GetMetadataValue_ReturnsValue()
    {
        var json = """
        {
            "id": "run-123",
            "name": "experiment-v1",
            "metadata": {"model": "gpt-4", "temperature": 0.7, "streaming": true}
        }
        """;

        var run = JsonSerializer.Deserialize<DatasetRun>(json)!;

        Assert.Equal("gpt-4", run.GetMetadataValue<string>("model"));
        Assert.Equal(0.7, run.GetMetadataValue<double>("temperature"));
        Assert.True(run.GetMetadataValue<bool>("streaming"));
    }

    [Fact]
    public void DatasetRun_GetMetadataValue_ReturnsDefaultWhenNotFound()
    {
        var json = """
        {
            "id": "run-123",
            "name": "experiment-v1",
            "metadata": {}
        }
        """;

        var run = JsonSerializer.Deserialize<DatasetRun>(json)!;

        Assert.Null(run.GetMetadataValue<string>("missing"));
        Assert.Equal("default", run.GetMetadataValue("missing", "default"));
    }

    [Fact]
    public void DatasetRun_GetMetadataValue_ReturnsDefaultWhenMetadataIsNull()
    {
        var json = """
        {
            "id": "run-123",
            "name": "experiment-v1",
            "metadata": null
        }
        """;

        var run = JsonSerializer.Deserialize<DatasetRun>(json)!;

        Assert.Equal("gpt-4", run.GetMetadataValue("model", "gpt-4"));
    }
}
