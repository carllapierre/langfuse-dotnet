# Langfuse .NET SDK (Unofficial) 🪢

[![Langfuse.OpenTelemetry](https://img.shields.io/nuget/v/Langfuse.OpenTelemetry.svg?label=Langfuse.OpenTelemetry)](https://www.nuget.org/packages/Langfuse.OpenTelemetry/)
[![Langfuse.Client](https://img.shields.io/nuget/v/Langfuse.Client.svg?label=Langfuse.Client)](https://www.nuget.org/packages/Langfuse.Client/)
[![Langfuse.Core](https://img.shields.io/nuget/v/Langfuse.Core.svg?label=Langfuse.Core)](https://www.nuget.org/packages/Langfuse.Core/)
[![Build](https://github.com/carllapierre/langfuse-otel-dotnet/actions/workflows/build.yml/badge.svg)](https://github.com/carllapierre/langfuse-otel-dotnet/actions/workflows/build.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

Unofficial .NET SDK for [Langfuse](https://langfuse.com) - the open-source LLM engineering platform.

## Features

| Feature | Description | Docs |
|---------|-------------|------|
| **OpenTelemetry Tracing** | Export .NET OTEL traces to Langfuse | [OTEL Integration](https://langfuse.com/docs/integrations/otel) |
| **Prompt Management** | Fetch, compile, and cache text & chat prompts | [Prompt Management](https://langfuse.com/docs/prompts/get-started) |
| **Scores** | Create user feedback and evaluation scores | [Scores](https://langfuse.com/docs/scores/overview) |
| **Trace Management** | List, get, and delete traces via API | [Public API](https://langfuse.com/docs/api-and-data-platform/features/public-api) |
| **Datasets** | Create and manage evaluation datasets | [Datasets](https://langfuse.com/docs/datasets/overview) |
| **Experiments** | Log agent runs with datasets | [Experiments](https://langfuse.com/docs/datasets/overview) |

---

## Packages

| Package | Description | Install |
|---------|-------------|---------|
| **Langfuse.OpenTelemetry** | Export OTEL traces to Langfuse | `dotnet add package Langfuse.OpenTelemetry` |
| **Langfuse.Client** | Prompt management, scores, traces, datasets, experiments | `dotnet add package Langfuse.Client` |
| **Langfuse.Core** | Shared config & types (auto-installed) | `dotnet add package Langfuse.Core` |

---

## Quick Start

### Configuration

Set environment variables:

```bash
LANGFUSE_PUBLIC_KEY=pk-lf-...
LANGFUSE_SECRET_KEY=sk-lf-...
LANGFUSE_BASE_URL=https://cloud.langfuse.com  # EU region (default)
# LANGFUSE_BASE_URL=https://us.cloud.langfuse.com  # US region
```

### OpenTelemetry Tracing

Export traces from any OTEL-instrumented library (including Semantic Kernel) to Langfuse:

```csharp
using Langfuse.OpenTelemetry;
using OpenTelemetry;
using OpenTelemetry.Trace;

using var tracerProvider = Sdk.CreateTracerProviderBuilder()
    .AddSource("Microsoft.SemanticKernel*")
    .AddLangfuseExporter()
    .Build();
```

### Langfuse Client

Access Langfuse features directly from .NET:

```csharp
using Langfuse.Client;

var client = new LangfuseClient();

// Prompts, Scores, Datasets, Experiments
```

---

## Documentation

- [Features](docs/features/) - Detailed feature documentation
- [Testing Guide](docs/TESTING.md) - How to run tests
- [Contributing](CONTRIBUTING.md) - How to contribute

## Running the Sample

```bash
cd samples/SemanticKernel.Sample
dotnet run
```

Check your [Langfuse dashboard](https://cloud.langfuse.com) to see the traces.

## License

MIT License - see [LICENSE](LICENSE) for details.

## Links

- [Langfuse](https://langfuse.com) - Open-source LLM engineering platform
- [Langfuse Docs](https://langfuse.com/docs) - Official documentation
