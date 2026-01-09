using System.Text.Json;
using Langfuse.Client.Datasets;
using Langfuse.Core;
using Xunit;

namespace Langfuse.Client.Tests.Datasets;

public class PaginatedResponseTests
{
    [Fact]
    public void PaginatedResponse_DatasetItems_DeserializesFromJson()
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

        var result = JsonSerializer.Deserialize<PaginatedResponse<DatasetItem>>(json);

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
    public void PaginatedResponse_Datasets_DeserializesFromJson()
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

        var result = JsonSerializer.Deserialize<PaginatedResponse<Dataset>>(json);

        Assert.NotNull(result);
        Assert.Equal(2, result.Data.Count);
        Assert.Equal("dataset-1", result.Data[0].Id);
        Assert.Equal("test-dataset-1", result.Data[0].Name);
        Assert.Equal(1, result.Meta.TotalPages);
        Assert.False(result.Meta.HasNextPage);
    }

    [Fact]
    public void PaginatedResponse_DatasetRunItems_DeserializesFromJson()
    {
        var json = """
        {
            "data": [
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
            ],
            "meta": {
                "page": 1,
                "limit": 50,
                "totalItems": 2,
                "totalPages": 1
            }
        }
        """;

        var result = JsonSerializer.Deserialize<PaginatedResponse<DatasetRunItem>>(json);

        Assert.NotNull(result);
        Assert.Equal(2, result.Data.Count);
        Assert.Equal(1, result.Meta.Page);
        Assert.Equal(50, result.Meta.Limit);
        Assert.Equal(2, result.Meta.TotalItems);
        Assert.Equal(1, result.Meta.TotalPages);

        Assert.Equal("run-item-1", result.Data[0].Id);
        Assert.Equal("trace-1", result.Data[0].TraceId);

        Assert.Equal("run-item-2", result.Data[1].Id);
        Assert.Equal("trace-2", result.Data[1].TraceId);
        Assert.Equal("obs-2", result.Data[1].ObservationId);
    }

    [Fact]
    public void PaginatedResponse_DeserializesEmptyData()
    {
        var json = """
        {
            "data": [],
            "meta": {
                "page": 1,
                "limit": 50,
                "totalItems": 0,
                "totalPages": 0
            }
        }
        """;

        var result = JsonSerializer.Deserialize<PaginatedResponse<DatasetItem>>(json);

        Assert.NotNull(result);
        Assert.Empty(result.Data);
        Assert.Equal(0, result.Meta.TotalItems);
    }
}
