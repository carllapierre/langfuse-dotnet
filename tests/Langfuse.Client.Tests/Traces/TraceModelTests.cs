using System.Text.Json;
using Langfuse.Client.Traces;
using Xunit;

namespace Langfuse.Client.Tests.Traces;

public class TraceModelTests
{
    [Fact]
    public void Trace_DeserializesFromJson()
    {
        var json = """
        {
            "id": "trace-123",
            "name": "test-trace",
            "timestamp": "2024-01-15T10:30:00Z",
            "userId": "user-456",
            "sessionId": "session-789",
            "release": "1.0.0",
            "version": "v1",
            "environment": "production",
            "tags": ["important", "test"],
            "metadata": {"key": "value"},
            "input": {"prompt": "Hello"},
            "output": {"response": "World"},
            "public": true
        }
        """;

        var trace = JsonSerializer.Deserialize<Trace>(json);

        Assert.NotNull(trace);
        Assert.Equal("trace-123", trace.Id);
        Assert.Equal("test-trace", trace.Name);
        Assert.Equal("user-456", trace.UserId);
        Assert.Equal("session-789", trace.SessionId);
        Assert.Equal("1.0.0", trace.Release);
        Assert.Equal("v1", trace.Version);
        Assert.Equal("production", trace.Environment);
        Assert.NotNull(trace.Tags);
        Assert.Contains("important", trace.Tags);
        Assert.Contains("test", trace.Tags);
        Assert.True(trace.Public);
    }

    [Fact]
    public void TraceWithDetails_DeserializesFromJson()
    {
        var json = """
        {
            "id": "trace-123",
            "name": "test-trace",
            "timestamp": "2024-01-15T10:30:00Z",
            "htmlPath": "/project/123/traces/trace-123",
            "latency": 1.5,
            "totalCost": 0.0025,
            "observations": ["obs-1", "obs-2"],
            "scores": ["score-1"]
        }
        """;

        var trace = JsonSerializer.Deserialize<TraceWithDetails>(json);

        Assert.NotNull(trace);
        Assert.Equal("trace-123", trace.Id);
        Assert.Equal("test-trace", trace.Name);
        Assert.Equal("/project/123/traces/trace-123", trace.HtmlPath);
        Assert.Equal(1.5, trace.Latency);
        Assert.Equal(0.0025, trace.TotalCost);
        Assert.Equal(2, trace.Observations.Count);
        Assert.Contains("obs-1", trace.Observations);
        Assert.Single(trace.Scores);
    }

    [Fact]
    public void TraceWithFullDetails_DeserializesWithObservations()
    {
        var json = """
        {
            "id": "trace-123",
            "name": "test-trace",
            "timestamp": "2024-01-15T10:30:00Z",
            "htmlPath": "/project/123/traces/trace-123",
            "latency": 2.0,
            "totalCost": 0.005,
            "observations": [
                {
                    "id": "obs-1",
                    "traceId": "trace-123",
                    "type": "GENERATION",
                    "name": "chat-completion",
                    "startTime": "2024-01-15T10:30:00Z",
                    "endTime": "2024-01-15T10:30:02Z",
                    "model": "gpt-4",
                    "usage": {
                        "input": 100,
                        "output": 50,
                        "total": 150
                    }
                }
            ],
            "scores": [
                {
                    "id": "score-1",
                    "traceId": "trace-123",
                    "name": "quality",
                    "value": 0.95,
                    "dataType": "NUMERIC"
                }
            ]
        }
        """;

        var trace = JsonSerializer.Deserialize<TraceWithFullDetails>(json);

        Assert.NotNull(trace);
        Assert.Equal("trace-123", trace.Id);
        Assert.Single(trace.Observations);
        Assert.Equal("obs-1", trace.Observations[0].Id);
        Assert.Equal("GENERATION", trace.Observations[0].Type);
        Assert.Equal("gpt-4", trace.Observations[0].Model);
        Assert.NotNull(trace.Observations[0].Usage);
        Assert.Equal(100, trace.Observations[0].Usage!.Input);
        Assert.Single(trace.Scores);
        Assert.Equal("quality", trace.Scores[0].Name);
        Assert.Equal(0.95, trace.Scores[0].Value);
    }

    [Fact]
    public void Observation_DeserializesAllProperties()
    {
        var json = """
        {
            "id": "obs-123",
            "traceId": "trace-456",
            "type": "SPAN",
            "name": "process-request",
            "startTime": "2024-01-15T10:30:00Z",
            "endTime": "2024-01-15T10:30:05Z",
            "model": "gpt-4-turbo",
            "modelId": "model-id-123",
            "level": "DEFAULT",
            "statusMessage": "Success",
            "version": "v1",
            "environment": "staging",
            "latency": 5.0,
            "calculatedInputCost": 0.001,
            "calculatedOutputCost": 0.002,
            "calculatedTotalCost": 0.003
        }
        """;

        var obs = JsonSerializer.Deserialize<Observation>(json);

        Assert.NotNull(obs);
        Assert.Equal("obs-123", obs.Id);
        Assert.Equal("trace-456", obs.TraceId);
        Assert.Equal("SPAN", obs.Type);
        Assert.Equal("process-request", obs.Name);
        Assert.Equal("gpt-4-turbo", obs.Model);
        Assert.Equal("DEFAULT", obs.Level);
        Assert.Equal("Success", obs.StatusMessage);
        Assert.Equal(5.0, obs.Latency);
        Assert.Equal(0.003, obs.CalculatedTotalCost);
    }

    [Fact]
    public void ObservationUsage_DeserializesCorrectly()
    {
        var json = """
        {
            "input": 500,
            "output": 200,
            "total": 700,
            "inputCost": 0.01,
            "outputCost": 0.02,
            "totalCost": 0.03,
            "unit": "TOKENS"
        }
        """;

        var usage = JsonSerializer.Deserialize<ObservationUsage>(json);

        Assert.NotNull(usage);
        Assert.Equal(500, usage.Input);
        Assert.Equal(200, usage.Output);
        Assert.Equal(700, usage.Total);
        Assert.Equal(0.01, usage.InputCost);
        Assert.Equal(0.02, usage.OutputCost);
        Assert.Equal(0.03, usage.TotalCost);
        Assert.Equal("TOKENS", usage.Unit);
    }

    [Fact]
    public void DeleteTraceResponse_DeserializesCorrectly()
    {
        var json = """
        {
            "message": "Trace deleted successfully"
        }
        """;

        var response = JsonSerializer.Deserialize<DeleteTraceResponse>(json);

        Assert.NotNull(response);
        Assert.Equal("Trace deleted successfully", response.Message);
    }
}
