# Prompt Management

Fetch and use prompts managed in Langfuse with client-side caching.

## Langfuse Docs

- [Prompt Management Overview](https://langfuse.com/docs/prompt-management/overview)
- [Get Started with Prompts](https://langfuse.com/docs/prompt-management/get-started)
- [Prompt Version Control](https://langfuse.com/docs/prompt-management/features/prompt-version-control)
- [Caching](https://langfuse.com/docs/prompt-management/features/caching)

## Features

| Feature | Status | Notes |
|---------|--------|-------|
| Fetch text prompts | Supported | `GetPromptAsync()` |
| Fetch chat prompts | Supported | `GetChatPromptAsync()` |
| Variable compilation | Supported | `prompt.Compile()` with `{{variable}}` syntax |
| Version selection | Supported | `version` parameter |
| Label selection | Supported | `label` parameter (production, staging, etc.) |
| Client-side caching | Supported | 60s TTL (configurable) |
| Fallback prompts | Supported | Use fallback when API fails |
| Config access | Supported | `prompt.Config`, `GetConfigValue<T>()` |

## Usage

### Basic Text Prompt

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
```

### Chat Prompt

```csharp
var chatPrompt = await client.GetChatPromptAsync("movie-critic-chat");
var messages = chatPrompt.Compile(("criticlevel", "expert"), ("movie", "Dune 2"));
// -> [{ role: "system", content: "..." }, { role: "user", content: "..." }]
```

### Version and Label Selection

```csharp
// Get specific version
var v1 = await client.GetPromptAsync("my-prompt", version: 1);

// Get by label
var staging = await client.GetPromptAsync("my-prompt", label: "staging");
```

### Fallback Prompts

```csharp
var fallback = TextPrompt.CreateFallback("default", "Default prompt text");
var prompt = await client.GetPromptAsync("my-prompt", fallback: fallback);
```

### Accessing Config

```csharp
var model = prompt.GetConfigValue<string>("model");
var temperature = prompt.GetConfigValue<double>("temperature", 0.7);
```

