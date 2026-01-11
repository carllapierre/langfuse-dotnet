using System.Net;
using System.Text.Json;
using Langfuse.Client;
using Langfuse.Core;
using Moq;
using Moq.Protected;
using Xunit;

namespace Langfuse.Client.Tests.Traces;

public class TraceClientTests
{
    private static (LangfuseClient client, Mock<HttpMessageHandler> handler) CreateTestClient()
    {
        var mockHandler = new Mock<HttpMessageHandler>();
        mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{}")
            });

        var httpClient = new HttpClient(mockHandler.Object)
        {
            BaseAddress = new Uri("https://cloud.langfuse.com")
        };

        var options = new LangfuseClientOptions
        {
            BaseUrl = "https://cloud.langfuse.com",
            PublicKey = "test-key",
            SecretKey = "test-secret"
        };

        return (new LangfuseClient(options, httpClient), mockHandler);
    }

    #region GetTracesAsync Tests

    [Fact]
    public async Task GetTracesAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var (client, handler) = CreateTestClient();
        string? capturedPath = null;

        handler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((request, _) =>
            {
                capturedPath = request.RequestUri?.PathAndQuery;
            })
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("""{"data": [], "meta": {"page": 1, "limit": 50, "totalItems": 0, "totalPages": 0}}""")
            });

        // Act
        await client.GetTracesAsync();

        // Assert
        Assert.NotNull(capturedPath);
        Assert.StartsWith("/api/public/traces?", capturedPath);
        Assert.Contains("page=1", capturedPath);
        Assert.Contains("limit=50", capturedPath);
        client.Dispose();
    }

    [Fact]
    public async Task GetTracesAsync_WithFilters_IncludesQueryParameters()
    {
        // Arrange
        var (client, handler) = CreateTestClient();
        string? capturedPath = null;

        handler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((request, _) =>
            {
                capturedPath = request.RequestUri?.PathAndQuery;
            })
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("""{"data": [], "meta": {"page": 1, "limit": 20, "totalItems": 0, "totalPages": 0}}""")
            });

        // Act
        await client.GetTracesAsync(
            page: 2,
            limit: 20,
            userId: "user-123",
            name: "chat-completion",
            sessionId: "session-456",
            orderBy: "timestamp.desc",
            version: "1.0",
            release: "prod");

        // Assert
        Assert.NotNull(capturedPath);
        Assert.Contains("page=2", capturedPath);
        Assert.Contains("limit=20", capturedPath);
        Assert.Contains("userId=user-123", capturedPath);
        Assert.Contains("name=chat-completion", capturedPath);
        Assert.Contains("sessionId=session-456", capturedPath);
        Assert.Contains("orderBy=timestamp.desc", capturedPath);
        Assert.Contains("version=1.0", capturedPath);
        Assert.Contains("release=prod", capturedPath);
        client.Dispose();
    }

    [Fact]
    public async Task GetTracesAsync_WithTags_IncludesMultipleTags()
    {
        // Arrange
        var (client, handler) = CreateTestClient();
        string? capturedPath = null;

        handler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((request, _) =>
            {
                capturedPath = request.RequestUri?.PathAndQuery;
            })
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("""{"data": [], "meta": {"page": 1, "limit": 50, "totalItems": 0, "totalPages": 0}}""")
            });

        // Act
        await client.GetTracesAsync(tags: new[] { "production", "important" });

        // Assert
        Assert.NotNull(capturedPath);
        Assert.Contains("tags=production", capturedPath);
        Assert.Contains("tags=important", capturedPath);
        client.Dispose();
    }

    [Fact]
    public async Task GetTracesAsync_WithTimestamps_FormatsCorrectly()
    {
        // Arrange
        var (client, handler) = CreateTestClient();
        string? capturedPath = null;
        var fromTime = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc);
        var toTime = new DateTime(2024, 1, 16, 10, 0, 0, DateTimeKind.Utc);

        handler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((request, _) =>
            {
                capturedPath = request.RequestUri?.PathAndQuery;
            })
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("""{"data": [], "meta": {"page": 1, "limit": 50, "totalItems": 0, "totalPages": 0}}""")
            });

        // Act
        await client.GetTracesAsync(fromTimestamp: fromTime, toTimestamp: toTime);

        // Assert
        Assert.NotNull(capturedPath);
        Assert.Contains("fromTimestamp=", capturedPath);
        Assert.Contains("toTimestamp=", capturedPath);
        client.Dispose();
    }

    [Fact]
    public async Task GetTracesAsync_ReturnsDeserializedResponse()
    {
        // Arrange
        var (client, handler) = CreateTestClient();

        handler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("""
                {
                    "data": [
                        {
                            "id": "trace-1",
                            "name": "test-trace",
                            "timestamp": "2024-01-15T10:00:00Z",
                            "htmlPath": "/traces/trace-1",
                            "latency": 1.5,
                            "totalCost": 0.001,
                            "observations": ["obs-1"],
                            "scores": []
                        }
                    ],
                    "meta": {
                        "page": 1,
                        "limit": 50,
                        "totalItems": 1,
                        "totalPages": 1
                    }
                }
                """)
            });

        // Act
        var result = await client.GetTracesAsync();

        // Assert
        Assert.Single(result.Data);
        Assert.Equal("trace-1", result.Data[0].Id);
        Assert.Equal("test-trace", result.Data[0].Name);
        Assert.Equal(1.5, result.Data[0].Latency);
        Assert.Equal(1, result.Meta.TotalItems);
        client.Dispose();
    }

    #endregion

    #region GetTraceAsync Tests

    [Fact]
    public async Task GetTraceAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var (client, handler) = CreateTestClient();
        string? capturedPath = null;

        handler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((request, _) =>
            {
                capturedPath = request.RequestUri?.PathAndQuery;
            })
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("""
                {
                    "id": "trace-123",
                    "name": "test",
                    "timestamp": "2024-01-15T10:00:00Z",
                    "observations": [],
                    "scores": []
                }
                """)
            });

        // Act
        await client.GetTraceAsync("trace-123");

        // Assert
        Assert.Equal("/api/public/traces/trace-123", capturedPath);
        client.Dispose();
    }

    [Fact]
    public async Task GetTraceAsync_ReturnsFullDetails()
    {
        // Arrange
        var (client, handler) = CreateTestClient();

        handler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("""
                {
                    "id": "trace-123",
                    "name": "chat-completion",
                    "timestamp": "2024-01-15T10:00:00Z",
                    "userId": "user-456",
                    "htmlPath": "/traces/trace-123",
                    "latency": 2.5,
                    "totalCost": 0.005,
                    "observations": [
                        {
                            "id": "obs-1",
                            "type": "GENERATION",
                            "name": "gpt-call",
                            "startTime": "2024-01-15T10:00:00Z"
                        }
                    ],
                    "scores": [
                        {
                            "id": "score-1",
                            "name": "quality",
                            "value": 0.95,
                            "dataType": "NUMERIC"
                        }
                    ]
                }
                """)
            });

        // Act
        var result = await client.GetTraceAsync("trace-123");

        // Assert
        Assert.Equal("trace-123", result.Id);
        Assert.Equal("chat-completion", result.Name);
        Assert.Equal("user-456", result.UserId);
        Assert.Single(result.Observations);
        Assert.Equal("obs-1", result.Observations[0].Id);
        Assert.Single(result.Scores);
        Assert.Equal("quality", result.Scores[0].Name);
        client.Dispose();
    }

    [Fact]
    public async Task GetTraceAsync_ThrowsOnNull()
    {
        // Arrange
        var (client, _) = CreateTestClient();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            client.GetTraceAsync(null!));
        client.Dispose();
    }

    [Fact]
    public async Task GetTraceAsync_NotFound_ThrowsException()
    {
        // Arrange
        var (client, handler) = CreateTestClient();

        handler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotFound,
                Content = new StringContent("""{"error": "Trace not found"}""")
            });

        // Act & Assert
        var exception = await Assert.ThrowsAsync<LangfuseApiException>(() =>
            client.GetTraceAsync("non-existent-trace"));

        Assert.Equal(404, exception.StatusCode);
        client.Dispose();
    }

    #endregion

    #region DeleteTraceAsync Tests

    [Fact]
    public async Task DeleteTraceAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var (client, handler) = CreateTestClient();
        string? capturedPath = null;
        HttpMethod? capturedMethod = null;

        handler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((request, _) =>
            {
                capturedPath = request.RequestUri?.PathAndQuery;
                capturedMethod = request.Method;
            })
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("""{"message": "Deleted"}""")
            });

        // Act
        await client.DeleteTraceAsync("trace-123");

        // Assert
        Assert.Equal("/api/public/traces/trace-123", capturedPath);
        Assert.Equal(HttpMethod.Delete, capturedMethod);
        client.Dispose();
    }

    [Fact]
    public async Task DeleteTraceAsync_ThrowsOnNull()
    {
        // Arrange
        var (client, _) = CreateTestClient();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            client.DeleteTraceAsync(null!));
        client.Dispose();
    }

    #endregion

    #region DeleteTracesAsync Tests

    [Fact]
    public async Task DeleteTracesAsync_CallsCorrectEndpoint()
    {
        // Arrange
        var (client, handler) = CreateTestClient();
        string? capturedPath = null;
        HttpMethod? capturedMethod = null;
        string? capturedBody = null;

        handler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((request, _) =>
            {
                capturedPath = request.RequestUri?.PathAndQuery;
                capturedMethod = request.Method;
                capturedBody = request.Content?.ReadAsStringAsync().GetAwaiter().GetResult();
            })
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("""{"message": "Deleted"}""")
            });

        // Act
        await client.DeleteTracesAsync(new[] { "trace-1", "trace-2", "trace-3" });

        // Assert
        Assert.Equal("/api/public/traces", capturedPath);
        Assert.Equal(HttpMethod.Delete, capturedMethod);
        Assert.NotNull(capturedBody);

        using var doc = JsonDocument.Parse(capturedBody);
        var traceIds = doc.RootElement.GetProperty("traceIds").EnumerateArray()
            .Select(e => e.GetString())
            .ToList();

        Assert.Contains("trace-1", traceIds);
        Assert.Contains("trace-2", traceIds);
        Assert.Contains("trace-3", traceIds);
        client.Dispose();
    }

    [Fact]
    public async Task DeleteTracesAsync_ThrowsOnNull()
    {
        // Arrange
        var (client, _) = CreateTestClient();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            client.DeleteTracesAsync(null!));
        client.Dispose();
    }

    [Fact]
    public async Task DeleteTracesAsync_ThrowsOnEmpty()
    {
        // Arrange
        var (client, _) = CreateTestClient();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            client.DeleteTracesAsync(Array.Empty<string>()));
        client.Dispose();
    }

    #endregion

    #region Error Handling Tests

    [Fact]
    public async Task GetTracesAsync_ApiError_ThrowsLangfuseApiException()
    {
        // Arrange
        var (client, handler) = CreateTestClient();

        handler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Content = new StringContent("""{"error": "Internal server error"}""")
            });

        // Act & Assert
        var exception = await Assert.ThrowsAsync<LangfuseApiException>(() =>
            client.GetTracesAsync());

        Assert.Equal(500, exception.StatusCode);
        client.Dispose();
    }

    [Fact]
    public async Task DeleteTraceAsync_Unauthorized_ThrowsLangfuseApiException()
    {
        // Arrange
        var (client, handler) = CreateTestClient();

        handler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.Unauthorized,
                Content = new StringContent("""{"error": "Invalid API key"}""")
            });

        // Act & Assert
        var exception = await Assert.ThrowsAsync<LangfuseApiException>(() =>
            client.DeleteTraceAsync("trace-123"));

        Assert.Equal(401, exception.StatusCode);
        client.Dispose();
    }

    #endregion
}
