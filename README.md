# Langfuse .NET SDK (Unofficial)

[![Langfuse.OpenTelemetry](https://img.shields.io/nuget/v/Langfuse.OpenTelemetry.svg?label=Langfuse.OpenTelemetry)](https://www.nuget.org/packages/Langfuse.OpenTelemetry/)
[![Langfuse.Client](https://img.shields.io/nuget/v/Langfuse.Client.svg?label=Langfuse.Client)](https://www.nuget.org/packages/Langfuse.Client/)
[![Langfuse.Core](https://img.shields.io/nuget/v/Langfuse.Core.svg?label=Langfuse.Core)](https://www.nuget.org/packages/Langfuse.Core/)
[![Build](https://github.com/carllapierre/langfuse-otel-dotnet/actions/workflows/build.yml/badge.svg)](https://github.com/carllapierre/langfuse-otel-dotnet/actions/workflows/build.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

Unofficial .NET SDK for [Langfuse](https://langfuse.com) - the open-source LLM engineering platform.

## Packages

| Package | Description | Install |
|---------|-------------|---------|
| **Langfuse.OpenTelemetry** | Export OTEL traces to Langfuse | `dotnet add package Langfuse.OpenTelemetry` |
| **Langfuse.Client** | Prompt management, user feedback | `dotnet add package Langfuse.Client` |
| **Langfuse.Core** | Shared config & types (auto-installed) | `dotnet add package Langfuse.Core` |

---

## Langfuse.OpenTelemetry

Export .NET OpenTelemetry traces to Langfuse. Works with any OTEL-instrumented library including Semantic Kernel.

### Quick Start

**1. Install**
```bash
dotnet add package Langfuse.OpenTelemetry
```

**2. Set environment variables**
```bash
LANGFUSE_PUBLIC_KEY=pk-lf-...
LANGFUSE_SECRET_KEY=sk-lf-...
LANGFUSE_BASE_URL=https://cloud.langfuse.com  # EU region (default)
# LANGFUSE_BASE_URL=https://us.cloud.langfuse.com  # US region
```

**3. Add to your app**
```csharp
using Langfuse.OpenTelemetry;
using Microsoft.SemanticKernel;
using OpenTelemetry;
using OpenTelemetry.Trace;

// Enable GenAI diagnostics (prompts, tokens, completions)
AppContext.SetSwitch("Microsoft.SemanticKernel.Experimental.GenAI.EnableOTelDiagnosticsSensitive", true);

// Setup OpenTelemetry with Langfuse exporter
using var tracerProvider = Sdk.CreateTracerProviderBuilder()
    .AddSource("Microsoft.SemanticKernel*")
    .AddLangfuseExporter()
    .Build();

// Use Semantic Kernel as normal
var kernel = Kernel.CreateBuilder()
    .AddOpenAIChatCompletion("gpt-4o-mini", apiKey)
    .Build();

var result = await kernel.InvokePromptAsync("Hello!");
```

### Configuration Options

```csharp
// Option 1: Environment variables (recommended)
.AddLangfuseExporter()

// Option 2: Manual configuration
.AddLangfuseExporter(options =>
{
    options.PublicKey = "pk-lf-...";
    options.SecretKey = "sk-lf-...";
    options.BaseUrl = "https://cloud.langfuse.com";
})

// Option 3: From IConfiguration (appsettings.json)
.AddLangfuseExporter(configuration)
```

---

## Langfuse.Client

Access Langfuse features like Prompt Management and User Feedback directly from .NET.

### Quick Start

**1. Install**
```bash
dotnet add package Langfuse.Client
```

**2. Set environment variables**
```bash
LANGFUSE_PUBLIC_KEY=pk-lf-...
LANGFUSE_SECRET_KEY=sk-lf-...
LANGFUSE_BASE_URL=https://cloud.langfuse.com
```

**3. Use prompts**
```csharp
using Langfuse.Client;

var client = new LangfuseClient();

// Fetch a text prompt (cached for 60s by default)
var prompt = await client.GetPromptAsync("movie-critic");

// Compile with variables
var compiled = prompt.Compile(new Dictionary<string, string>
{
    ["criticlevel"] = "expert",
    ["movie"] = "Dune 2"
});
// -> "As an expert movie critic, do you like Dune 2?"

// Fetch a chat prompt
var chatPrompt = await client.GetChatPromptAsync("movie-critic-chat");
var messages = chatPrompt.Compile(("criticlevel", "expert"), ("movie", "Dune 2"));
// -> [{ role: "system", content: "..." }, { role: "user", content: "..." }]
```

### Features

- **Text & Chat prompts** - Full support for both prompt types
- **Variable compilation** - `{{variable}}` syntax support
- **Version/Label selection** - Fetch specific versions or labels (production, staging)
- **Client-side caching** - 60s TTL by default, configurable
- **Fallback prompts** - Graceful degradation when API fails
- **Config access** - Access prompt config (model, temperature, etc.)
- **User feedback** - Create scores and assign them to traces

```csharp
// Get specific version
var v1 = await client.GetPromptAsync("my-prompt", version: 1);

// Get by label
var staging = await client.GetPromptAsync("my-prompt", label: "staging");

// With fallback
var fallback = TextPrompt.CreateFallback("default", "Fallback prompt text");
var prompt = await client.GetPromptAsync("my-prompt", fallback: fallback);

// Access config
var model = prompt.GetConfigValue<string>("model");
var temperature = prompt.GetConfigValue<double>("temperature", 0.7);

// User feedback / scores
await client.CreateScoreAsync("trace-id", "user-feedback", value: true);
await client.CreateScoreAsync("trace-id", "quality", value: 0.95, comment: "Great!");
```

---

## Documentation

- [Testing Guide](docs/TESTING.md) - How to run tests
- [Features](docs/features/) - Implemented features with Langfuse docs links
- [Contributing](CONTRIBUTING.md) - How to contribute

## Running the Sample

```bash
# Set environment variables
export OPENAI_API_KEY="sk-..."
export LANGFUSE_PUBLIC_KEY="pk-lf-..."
export LANGFUSE_SECRET_KEY="sk-lf-..."
export LANGFUSE_BASE_URL="https://cloud.langfuse.com"

# Run sample
cd samples/SemanticKernel.Sample
dotnet run
```

Check your [Langfuse dashboard](https://cloud.langfuse.com) to see the traces.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Links

- [Langfuse](https://langfuse.com) - Open-source LLM engineering platform
- [Langfuse Docs](https://langfuse.com/docs) - Official documentation
- [OpenTelemetry Integration](https://langfuse.com/docs/integrations/otel) - OTEL docs
- [Prompt Management](https://langfuse.com/docs/prompt-management/overview) - Prompts docs
