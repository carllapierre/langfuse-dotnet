using System.Net;
using System.Text.Json;
using Langfuse.Client;
using Langfuse.Core;
using Moq;
using Moq.Protected;
using Xunit;

namespace Langfuse.Client.Tests.Scores;

public class ScoreTests
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

    [Fact]
    public async Task CreateScoreAsync_NumericScore_CallsCorrectEndpoint()
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
                Content = new StringContent("{}")
            });

        // Act
        await client.CreateScoreAsync("trace-123", "user-feedback", 1.0);

        // Assert
        Assert.Equal("/api/public/scores", capturedPath);
        client.Dispose();
    }

    [Fact]
    public async Task CreateScoreAsync_NumericScore_SerializesCorrectly()
    {
        // Arrange
        var (client, handler) = CreateTestClient();
        string? capturedBody = null;

        handler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((request, _) =>
            {
                capturedBody = request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
            })
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{}")
            });

        // Act
        await client.CreateScoreAsync("trace-123", "quality", 0.95, comment: "Great response!");

        // Assert
        Assert.NotNull(capturedBody);
        using var doc = JsonDocument.Parse(capturedBody);
        var root = doc.RootElement;
        
        Assert.Equal("trace-123", root.GetProperty("traceId").GetString());
        Assert.Equal("quality", root.GetProperty("name").GetString());
        Assert.Equal(0.95, root.GetProperty("value").GetDouble());
        Assert.Equal("Great response!", root.GetProperty("comment").GetString());
        Assert.Equal("NUMERIC", root.GetProperty("dataType").GetString());
        
        client.Dispose();
    }

    [Fact]
    public async Task CreateScoreAsync_BooleanScore_SerializesCorrectly()
    {
        // Arrange
        var (client, handler) = CreateTestClient();
        string? capturedBody = null;

        handler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((request, _) =>
            {
                capturedBody = request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
            })
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{}")
            });

        // Act
        await client.CreateScoreAsync("trace-456", "helpful", true);

        // Assert
        Assert.NotNull(capturedBody);
        using var doc = JsonDocument.Parse(capturedBody);
        var root = doc.RootElement;
        
        Assert.Equal("trace-456", root.GetProperty("traceId").GetString());
        Assert.Equal("helpful", root.GetProperty("name").GetString());
        Assert.Equal(1, root.GetProperty("value").GetDouble());
        Assert.Equal("BOOLEAN", root.GetProperty("dataType").GetString());
        
        client.Dispose();
    }

    [Fact]
    public async Task CreateScoreAsync_BooleanScoreFalse_SerializesAsZero()
    {
        // Arrange
        var (client, handler) = CreateTestClient();
        string? capturedBody = null;

        handler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((request, _) =>
            {
                capturedBody = request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
            })
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{}")
            });

        // Act
        await client.CreateScoreAsync("trace-456", "helpful", false);

        // Assert
        Assert.NotNull(capturedBody);
        using var doc = JsonDocument.Parse(capturedBody);
        var root = doc.RootElement;
        
        Assert.Equal(0, root.GetProperty("value").GetDouble());
        
        client.Dispose();
    }

    [Fact]
    public async Task CreateScoreAsync_CategoricalScore_SerializesCorrectly()
    {
        // Arrange
        var (client, handler) = CreateTestClient();
        string? capturedBody = null;

        handler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((request, _) =>
            {
                capturedBody = request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
            })
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{}")
            });

        // Act
        await client.CreateScoreAsync("trace-789", "sentiment", "positive", comment: "User seemed happy");

        // Assert
        Assert.NotNull(capturedBody);
        using var doc = JsonDocument.Parse(capturedBody);
        var root = doc.RootElement;
        
        Assert.Equal("trace-789", root.GetProperty("traceId").GetString());
        Assert.Equal("sentiment", root.GetProperty("name").GetString());
        Assert.Equal("positive", root.GetProperty("value").GetString());
        Assert.Equal("User seemed happy", root.GetProperty("comment").GetString());
        Assert.Equal("CATEGORICAL", root.GetProperty("dataType").GetString());
        
        client.Dispose();
    }

    [Fact]
    public async Task CreateScoreAsync_WithObservationId_IncludesInRequest()
    {
        // Arrange
        var (client, handler) = CreateTestClient();
        string? capturedBody = null;

        handler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((request, _) =>
            {
                capturedBody = request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
            })
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{}")
            });

        // Act
        await client.CreateScoreAsync("trace-123", "quality", 1.0, observationId: "obs-456");

        // Assert
        Assert.NotNull(capturedBody);
        using var doc = JsonDocument.Parse(capturedBody);
        var root = doc.RootElement;
        
        Assert.Equal("obs-456", root.GetProperty("observationId").GetString());
        
        client.Dispose();
    }

    [Fact]
    public async Task CreateScoreAsync_ApiError_ThrowsLangfuseApiException()
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
                StatusCode = HttpStatusCode.BadRequest,
                Content = new StringContent("{\"error\": \"Invalid traceId\"}")
            });

        // Act & Assert
        var exception = await Assert.ThrowsAsync<LangfuseApiException>(() =>
            client.CreateScoreAsync("invalid-trace", "test", 1.0));

        Assert.Equal(400, exception.StatusCode);
        client.Dispose();
    }
}
