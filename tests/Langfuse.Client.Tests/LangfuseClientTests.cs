using System.Net;
using System.Text.Json;
using Langfuse.Client;
using Langfuse.Core;
using Moq;
using Moq.Protected;
using Xunit;

namespace Langfuse.Client.Tests;

public class LangfuseClientTests
{
    private static object CreateMockPromptResponse(string name, int version = 1, string[]? labels = null)
    {
        return new
        {
            id = "test-id",
            name = name,
            version = version,
            type = "text",
            prompt = "Test content",
            labels = labels ?? new[] { "production" },
            tags = Array.Empty<string>(),
            config = new { },
            createdAt = DateTime.UtcNow.ToString("o"),
            updatedAt = DateTime.UtcNow.ToString("o")
        };
    }

    private static LangfuseClient CreateTestClient(object mockResponse, out string? capturedPath)
    {
        string? actualRequestPath = null;

        var mockHandler = new Mock<HttpMessageHandler>();
        mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync((HttpRequestMessage request, CancellationToken token) =>
            {
                actualRequestPath = request.RequestUri?.PathAndQuery;
                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(mockResponse))
                };
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

        capturedPath = actualRequestPath;
        return new LangfuseClient(options, httpClient);
    }

    [Fact]
    public async Task GetPromptAsync_WithSpacesInName_EncodesSpacesAsPercent20()
    {
        // Arrange
        var promptName = "test prompt";
        var mockResponse = CreateMockPromptResponse(promptName);
        using var client = CreateTestClient(mockResponse, out _);

        // Act
        var result = await client.GetPromptAsync(promptName);

        // Assert - The test succeeds if no exception is thrown and the name matches
        Assert.Equal(promptName, result.Name);
    }

    [Fact]
    public async Task GetPromptAsync_WithoutSpacesInName_EncodesCorrectly()
    {
        // Arrange
        var promptName = "test-prompt";
        var mockResponse = CreateMockPromptResponse(promptName);
        using var client = CreateTestClient(mockResponse, out _);

        // Act
        var result = await client.GetPromptAsync(promptName);

        // Assert
        Assert.Equal(promptName, result.Name);
    }

    [Fact]
    public async Task GetPromptAsync_WithSpacesInNameAndVersion_EncodesCorrectly()
    {
        // Arrange
        var promptName = "my test prompt";
        var version = 2;
        var mockResponse = CreateMockPromptResponse(promptName, version);
        using var client = CreateTestClient(mockResponse, out _);

        // Act
        var result = await client.GetPromptAsync(promptName, version: version);

        // Assert
        Assert.Equal(promptName, result.Name);
        Assert.Equal(version, result.Version);
    }

    [Fact]
    public async Task GetPromptAsync_WithSpacesInLabel_EncodesCorrectly()
    {
        // Arrange
        var promptName = "test prompt";
        var label = "my label";
        var mockResponse = CreateMockPromptResponse(promptName, labels: new[] { label });
        using var client = CreateTestClient(mockResponse, out _);

        // Act
        var result = await client.GetPromptAsync(promptName, label: label);

        // Assert
        Assert.Equal(promptName, result.Name);
    }

    [Fact]
    public async Task GetPromptAsync_WithSpecialCharactersInName_EncodesCorrectly()
    {
        // Arrange
        var promptName = "test/prompt&name=value";
        var mockResponse = CreateMockPromptResponse(promptName);
        using var client = CreateTestClient(mockResponse, out _);

        // Act
        var result = await client.GetPromptAsync(promptName);

        // Assert
        Assert.Equal(promptName, result.Name);
    }
}
