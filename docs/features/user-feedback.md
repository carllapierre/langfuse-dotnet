# User Feedback / Scores

Create scores in Langfuse to capture user feedback, evaluations, or any rating linked to traces.

## Langfuse Docs

- [User Feedback](https://langfuse.com/docs/observability/features/user-feedback)
- [Custom Scores](https://langfuse.com/docs/evaluation/evaluation-methods/custom-scores)

## Features

| Feature | Status | Notes |
|---------|--------|-------|
| Numeric scores | Supported | `CreateScoreAsync(traceId, name, double value)` |
| Boolean scores | Supported | `CreateScoreAsync(traceId, name, bool value)` |
| Categorical scores | Supported | `CreateScoreAsync(traceId, name, string stringValue)` |
| Comments | Supported | Optional `comment` parameter |
| Link to observation | Supported | Optional `observationId` parameter |

## Usage

### Numeric Score (Ratings, Quality Metrics)

```csharp
using Langfuse.Client;

var client = new LangfuseClient();

// Submit a numeric score (e.g., quality rating 0-1)
await client.CreateScoreAsync(
    traceId: "trace-123",
    name: "quality",
    value: 0.95,
    comment: "Excellent response"
);
```

### Boolean Score (Thumbs Up/Down)

```csharp
// Thumbs up
await client.CreateScoreAsync(
    traceId: "trace-123",
    name: "user-feedback",
    value: true,
    comment: "User liked the response"
);

// Thumbs down
await client.CreateScoreAsync(
    traceId: "trace-123",
    name: "user-feedback",
    value: false
);
```

### Categorical Score (Classification, Sentiment)

```csharp
await client.CreateScoreAsync(
    traceId: "trace-123",
    name: "sentiment",
    stringValue: "positive",
    comment: "User expressed satisfaction"
);
```

### Linking to a Specific Observation

```csharp
// Score a specific span within a trace
await client.CreateScoreAsync(
    traceId: "trace-123",
    name: "relevance",
    value: 0.8,
    observationId: "span-456"
);
```

## Getting the Trace ID

To submit feedback, you need the trace ID from your LLM interaction. When using OpenTelemetry with Langfuse:

1. The trace ID is available from your OpenTelemetry `Activity`
2. Pass this ID to your frontend/feedback collection system
3. Submit scores using `CreateScoreAsync`

```csharp
using System.Diagnostics;

// During your LLM call
var activity = Activity.Current;
var traceId = activity?.TraceId.ToString();

// Later, when collecting feedback
if (traceId != null)
{
    await client.CreateScoreAsync(traceId, "user-feedback", userLikedResponse);
}
```

