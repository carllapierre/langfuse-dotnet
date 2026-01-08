using System.Text.Json;
using Langfuse.Client.Datasets;
using Xunit;

namespace Langfuse.Client.Tests.Datasets;

public class PaginatedResponseTests
{
    [Fact]
    public void PaginatedDatasetItems_DeserializesFromJson()
    {
        var json = """
        {
            "data": [
                {
                    "id": "item-1",
                    "datasetId": "dataset-123",
                    "datasetName": "test-dataset",
                    "input": {"text": "hello"},
                    "status": "ACTIVE"
                },
                {
                    "id": "item-2",
                    "datasetId": "dataset-123",
                    "datasetName": "test-dataset",
                    "input": {"text": "world"},
                    "status": "ACTIVE"
                }
            ],
            "meta": {
                "page": 1,
                "limit": 50,
                "totalItems": 100,
                "totalPages": 2
            }
        }
        """;

        var result = JsonSerializer.Deserialize<PaginatedDatasetItems>(json);

        Assert.NotNull(result);
        Assert.Equal(2, result.Data.Count);
        Assert.Equal("item-1", result.Data[0].Id);
        Assert.Equal("item-2", result.Data[1].Id);
        Assert.Equal(1, result.Meta.Page);
        Assert.Equal(50, result.Meta.Limit);
        Assert.Equal(100, result.Meta.TotalItems);
        Assert.Equal(2, result.Meta.TotalPages);
    }

    [Fact]
    public void PaginationMeta_HasNextPage_ReturnsCorrectly()
    {
        var meta = new PaginationMeta { Page = 1, TotalPages = 3 };
        Assert.True(meta.HasNextPage);

        meta.Page = 3;
        Assert.False(meta.HasNextPage);
    }

    [Fact]
    public void PaginationMeta_HasPreviousPage_ReturnsCorrectly()
    {
        var meta = new PaginationMeta { Page = 1 };
        Assert.False(meta.HasPreviousPage);

        meta.Page = 2;
        Assert.True(meta.HasPreviousPage);
    }

    [Fact]
    public void PaginatedDatasets_DeserializesFromJson()
    {
        var json = """
        {
            "data": [
                {
                    "id": "dataset-1",
                    "name": "test-dataset-1",
                    "projectId": "project-123"
                },
                {
                    "id": "dataset-2",
                    "name": "test-dataset-2",
                    "projectId": "project-123"
                }
            ],
            "meta": {
                "page": 1,
                "limit": 10,
                "totalItems": 2,
                "totalPages": 1
            }
        }
        """;

        var result = JsonSerializer.Deserialize<PaginatedDatasets>(json);

        Assert.NotNull(result);
        Assert.Equal(2, result.Data.Count);
        Assert.Equal("dataset-1", result.Data[0].Id);
        Assert.Equal("test-dataset-1", result.Data[0].Name);
        Assert.Equal(1, result.Meta.TotalPages);
        Assert.False(result.Meta.HasNextPage);
    }
}

