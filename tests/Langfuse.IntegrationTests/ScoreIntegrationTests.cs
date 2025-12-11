using Langfuse.Client;
using Langfuse.Core;
using Xunit;

namespace Langfuse.IntegrationTests;

/// <summary>
/// Integration tests for Langfuse scores/user feedback.
/// Runs against Cloud (if LANGFUSE_PUBLIC_KEY/SECRET_KEY are set) or Local Docker instance.
/// Requires LANGFUSE_TEST_TRACE_ID environment variable to be set to a valid trace ID.
/// </summary>
[Trait("Category", "Integration")]
public class ScoreIntegrationTests : IDisposable
{
    private readonly LangfuseClient? _client;
    private readonly bool _skipTests;
    private readonly string _environment;
    private readonly string? _testTraceId;

    public ScoreIntegrationTests()
    {
        _testTraceId = Environment.GetEnvironmentVariable("LANGFUSE_TEST_TRACE_ID");

        // Try Cloud first, then Local
        if (TestConfiguration.IsCloudConfigured())
        {
            _environment = "Cloud";
            var options = TestConfiguration.GetCloudOptions();
            _client = new LangfuseClient(new LangfuseClientOptions
            {
                BaseUrl = options.BaseUrl,
                PublicKey = options.PublicKey,
                SecretKey = options.SecretKey
            });
        }
        else if (TestConfiguration.IsLocalConfigured())
        {
            _environment = "Local";
            var options = TestConfiguration.GetLocalOptions();
            _client = new LangfuseClient(new LangfuseClientOptions
            {
                BaseUrl = options.BaseUrl,
                PublicKey = options.PublicKey,
                SecretKey = options.SecretKey,
                Timeout = TimeSpan.FromSeconds(10)
            });
        }
        else
        {
            _skipTests = true;
            _environment = "None";
        }
    }

    [SkippableFact]
    public async Task CreateScore_NumericScore_Succeeds()
    {
        Skip.If(_skipTests, "No Langfuse configuration found. Set LANGFUSE_PUBLIC_KEY/SECRET_KEY for cloud, or LANGFUSE_LOCAL_PUBLIC_KEY/SECRET_KEY for local.");
        Skip.If(string.IsNullOrEmpty(_testTraceId), "LANGFUSE_TEST_TRACE_ID environment variable not set. Create a trace and set this variable to test score creation.");

        try
        {
            // Act - should not throw
            await _client!.CreateScoreAsync(
                _testTraceId!,
                "integration-test-numeric",
                0.85,
                comment: "Integration test score");

            // If we get here, the API call succeeded
            Assert.True(true);
        }
        catch (LangfuseApiException ex) when (ex.StatusCode == 401)
        {
            Assert.Fail($"Authentication failed against {_environment}. Check your API keys.");
        }
        catch (LangfuseApiException ex) when (ex.StatusCode == 404)
        {
            Skip.If(true, $"Trace '{_testTraceId}' not found in {_environment}. Ensure the trace exists.");
        }
    }

    [SkippableFact]
    public async Task CreateScore_BooleanScore_Succeeds()
    {
        Skip.If(_skipTests, "No Langfuse configuration found.");
        Skip.If(string.IsNullOrEmpty(_testTraceId), "LANGFUSE_TEST_TRACE_ID environment variable not set.");

        try
        {
            await _client!.CreateScoreAsync(
                _testTraceId!,
                "integration-test-boolean",
                true,
                comment: "Thumbs up test");

            Assert.True(true);
        }
        catch (LangfuseApiException ex) when (ex.StatusCode == 401)
        {
            Assert.Fail($"Authentication failed against {_environment}. Check your API keys.");
        }
        catch (LangfuseApiException ex) when (ex.StatusCode == 404)
        {
            Skip.If(true, $"Trace '{_testTraceId}' not found in {_environment}.");
        }
    }

    [SkippableFact]
    public async Task CreateScore_CategoricalScore_Succeeds()
    {
        Skip.If(_skipTests, "No Langfuse configuration found.");
        Skip.If(string.IsNullOrEmpty(_testTraceId), "LANGFUSE_TEST_TRACE_ID environment variable not set.");

        try
        {
            await _client!.CreateScoreAsync(
                _testTraceId!,
                "integration-test-categorical",
                "positive",
                comment: "Sentiment test");

            Assert.True(true);
        }
        catch (LangfuseApiException ex) when (ex.StatusCode == 401)
        {
            Assert.Fail($"Authentication failed against {_environment}. Check your API keys.");
        }
        catch (LangfuseApiException ex) when (ex.StatusCode == 404)
        {
            Skip.If(true, $"Trace '{_testTraceId}' not found in {_environment}.");
        }
    }

    [SkippableFact]
    public async Task CreateScore_WithRandomTraceId_ApiAccepts()
    {
        Skip.If(_skipTests, "No Langfuse configuration found.");

        // Langfuse API accepts scores for any trace ID (creates orphaned score if trace doesn't exist)
        var randomTraceId = Guid.NewGuid().ToString();

        // Should not throw - API is lenient about trace IDs
        await _client!.CreateScoreAsync(randomTraceId, "test-score", 1.0);

        // If we get here without exception, the test passes
        Assert.True(true);
    }

    public void Dispose()
    {
        _client?.Dispose();
    }
}

