using System.Text.Json;
using Langfuse.Client.Scores;
using Xunit;

namespace Langfuse.Client.Tests.Scores;

public class ScoreModelTests
{
    [Fact]
    public void Score_DeserializesCorrectly()
    {
        var json = """
        {
            "id": "score-123",
            "traceId": "trace-456",
            "observationId": "obs-789",
            "name": "accuracy",
            "value": 0.92,
            "stringValue": null,
            "dataType": "NUMERIC",
            "comment": "Good accuracy",
            "source": "API",
            "timestamp": "2024-01-15T11:00:00Z"
        }
        """;

        var score = JsonSerializer.Deserialize<Score>(json);

        Assert.NotNull(score);
        Assert.Equal("score-123", score.Id);
        Assert.Equal("trace-456", score.TraceId);
        Assert.Equal("obs-789", score.ObservationId);
        Assert.Equal("accuracy", score.Name);
        Assert.Equal(0.92, score.Value);
        Assert.Equal("NUMERIC", score.DataType);
        Assert.Equal("Good accuracy", score.Comment);
        Assert.Equal("API", score.Source);
    }

    [Fact]
    public void Score_DeserializesCategoricalScore()
    {
        var json = """
        {
            "id": "score-cat-1",
            "traceId": "trace-123",
            "name": "sentiment",
            "value": 1,
            "stringValue": "positive",
            "dataType": "CATEGORICAL",
            "source": "EVAL"
        }
        """;

        var score = JsonSerializer.Deserialize<Score>(json);

        Assert.NotNull(score);
        Assert.Equal("sentiment", score.Name);
        Assert.Equal("positive", score.StringValue);
        Assert.Equal("CATEGORICAL", score.DataType);
        Assert.Equal("EVAL", score.Source);
    }

    [Fact]
    public void Score_DeserializesBooleanScore()
    {
        var json = """
        {
            "id": "score-bool-1",
            "traceId": "trace-123",
            "name": "is_helpful",
            "value": 1,
            "stringValue": "True",
            "dataType": "BOOLEAN",
            "source": "ANNOTATION"
        }
        """;

        var score = JsonSerializer.Deserialize<Score>(json);

        Assert.NotNull(score);
        Assert.Equal("is_helpful", score.Name);
        Assert.Equal(1, score.Value);
        Assert.Equal("True", score.StringValue);
        Assert.Equal("BOOLEAN", score.DataType);
        Assert.Equal("ANNOTATION", score.Source);
    }

    [Fact]
    public void Score_DeserializesWithEnvironment()
    {
        var json = """
        {
            "id": "score-env-1",
            "traceId": "trace-123",
            "name": "quality",
            "value": 0.85,
            "dataType": "NUMERIC",
            "environment": "production",
            "configId": "config-456"
        }
        """;

        var score = JsonSerializer.Deserialize<Score>(json);

        Assert.NotNull(score);
        Assert.Equal("production", score.Environment);
        Assert.Equal("config-456", score.ConfigId);
    }
}
