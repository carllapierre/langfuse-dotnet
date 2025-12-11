# Features

This document lists the Langfuse features implemented in this unofficial .NET SDK.

## Packages

| Package | Description | Status |
|---------|-------------|--------|
| `Langfuse.Core` | Shared configuration, authentication, HTTP utilities | Stable |
| `Langfuse.Client` | API client for Langfuse features | Stable |
| `Langfuse.OpenTelemetry` | OTEL trace exporter to Langfuse | Stable |

## Implemented Features

| Feature | Package | Docs |
|---------|---------|------|
| [OpenTelemetry Export](../README.md#langfuseopentelemetry) | `Langfuse.OpenTelemetry` | [Langfuse OTEL](https://langfuse.com/docs/integrations/otel) |
| [Prompt Management](prompt-management.md) | `Langfuse.Client` | [Langfuse Prompts](https://langfuse.com/docs/prompt-management/overview) |
| [User Feedback / Scores](user-feedback.md) | `Langfuse.Client` | [Langfuse Scores](https://langfuse.com/docs/observability/features/user-feedback) |

## Not Yet Implemented

| Feature | Langfuse Docs |
|---------|---------------|
| Manual trace/span creation via SDK | [SDK Tracing](https://langfuse.com/docs/observability/sdk/overview) |
| Dataset management | [Datasets](https://langfuse.com/docs/evaluation/experiments/datasets) |
| Query traces/observations via SDK | [Query via SDK](https://langfuse.com/docs/api-and-data-platform/features/query-via-sdk) |

## API Reference

This SDK uses the Langfuse Public API:

- [API Reference](https://api.reference.langfuse.com)
- [OpenAPI Spec](https://cloud.langfuse.com/generated/api/openapi.yml)
- [Authentication](https://langfuse.com/docs/api-and-data-platform/features/public-api#authentication)

## Contributing

Want to help implement missing features? See [CONTRIBUTING.md](../../CONTRIBUTING.md) for guidelines.

