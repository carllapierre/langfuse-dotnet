using System.Text.Json;
using Langfuse.Client.Datasets;
using Xunit;

namespace Langfuse.Client.Tests.Datasets;

public class DatasetTests
{
    [Fact]
    public void Dataset_DeserializesFromJson()
    {
        var json = """
        {
            "id": "dataset-123",
            "name": "test-dataset",
            "description": "A test dataset",
            "projectId": "project-456",
            "metadata": {"author": "test-user", "version": 1},
            "inputSchema": null,
            "expectedOutputSchema": null,
            "createdAt": "2024-01-15T10:30:00Z",
            "updatedAt": "2024-01-15T11:00:00Z"
        }
        """;

        var dataset = JsonSerializer.Deserialize<Dataset>(json);

        Assert.NotNull(dataset);
        Assert.Equal("dataset-123", dataset.Id);
        Assert.Equal("test-dataset", dataset.Name);
        Assert.Equal("A test dataset", dataset.Description);
        Assert.Equal("project-456", dataset.ProjectId);
        Assert.NotNull(dataset.Metadata);
    }

    [Fact]
    public void GetMetadataValue_ReturnsValue()
    {
        var json = """
        {
            "id": "dataset-123",
            "name": "test-dataset",
            "metadata": {"author": "test-user", "version": 1, "active": true}
        }
        """;

        var dataset = JsonSerializer.Deserialize<Dataset>(json)!;

        Assert.Equal("test-user", dataset.GetMetadataValue<string>("author"));
        Assert.Equal(1, dataset.GetMetadataValue<int>("version"));
        Assert.True(dataset.GetMetadataValue<bool>("active"));
    }

    [Fact]
    public void GetMetadataValue_ReturnsDefaultWhenNotFound()
    {
        var json = """
        {
            "id": "dataset-123",
            "name": "test-dataset",
            "metadata": {}
        }
        """;

        var dataset = JsonSerializer.Deserialize<Dataset>(json)!;

        Assert.Null(dataset.GetMetadataValue<string>("missing"));
        Assert.Equal("default", dataset.GetMetadataValue("missing", "default"));
    }

    [Fact]
    public void GetMetadataValue_ReturnsDefaultWhenMetadataIsNull()
    {
        var json = """
        {
            "id": "dataset-123",
            "name": "test-dataset",
            "metadata": null
        }
        """;

        var dataset = JsonSerializer.Deserialize<Dataset>(json)!;

        Assert.Equal("default", dataset.GetMetadataValue("author", "default"));
    }
}

