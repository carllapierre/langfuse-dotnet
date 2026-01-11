# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.4.0] - 2025-01-10

### Added
- Trace Management API: List, retrieve, and delete traces via the Langfuse API
  - `GetTracesAsync()` - List traces with pagination and filtering (userId, name, sessionId, tags, timestamps, etc.)
  - `GetTraceAsync()` - Retrieve a single trace with full details including observations and scores
  - `DeleteTraceAsync()` - Delete a single trace by ID
  - `DeleteTracesAsync()` - Delete multiple traces by IDs
- New trace models: `Trace`, `TraceWithDetails`, `TraceWithFullDetails`, `Observation`, `ObservationUsage`
- `Score` response model in Scores domain for reading score data from API responses
- `DeleteAsync` and `DeleteWithBodyAsync` methods in `LangfuseHttpClientBase` for DELETE operations
- Added `LangfuseClient.Traces.cs` partial class for trace management methods

### Changed
- Refactored `LangfuseConstants` to use `ApiPublicPath` constant as base for all API endpoint paths (DRY improvement)

## [1.3.0] - 2025-01-09

### Added
- Dataset Management API: Full CRUD operations for datasets and dataset items
  - `CreateDatasetAsync()` - Create or update datasets with optional schema validation
  - `GetDatasetAsync()` - Retrieve datasets by name
  - `GetDatasetsAsync()` - List all datasets with pagination
  - `CreateDatasetItemAsync()` - Add items to datasets with input/output data
  - `GetDatasetItemAsync()` - Retrieve individual dataset items by ID
  - `GetDatasetItemsAsync()` - Query dataset items with filtering and pagination
  - `GetItemsForDatasetAsync()` - Convenience method to get all items for a specific dataset
- New models: `Dataset`, `DatasetItem`, `DatasetRun`, `DatasetRunItem`, `CreateDatasetRequest`, `CreateDatasetItemRequest`, `CreateDatasetRunItemRequest`
- Dataset Runs/Experiments API for evaluation workflows
  - `CreateDatasetRunItemAsync()` - Link traces to dataset items within a run
  - `GetDatasetRunAsync()` - Retrieve dataset runs by name
  - `GetDatasetRunItemsAsync()` - List run items with pagination
- Generic `PaginatedResponse<T>` and `PaginationMeta` in `Langfuse.Core` for reusable pagination
- Comprehensive dataset documentation in `docs/features/datasets.md`
- Integration tests for dataset operations

### Changed
- Refactored `LangfuseClient` to use partial classes for better code organization by API domain
  - `LangfuseClient.Prompts.cs` - Prompt management methods
  - `LangfuseClient.Scores.cs` - Score/feedback methods
  - `LangfuseClient.Datasets.cs` - Dataset management methods

## [1.2.0] - 2025-12-10

### Added
- User Feedback / Scores feature: `CreateScoreAsync()` methods for numeric, boolean, and categorical scores
- New `ScoreRequest` model for score submission
- Reorganized documentation into `docs/features/` with dedicated pages for each feature
- Support spaces and special characters in prompt management labels/names

## [1.1.0] - 2025-11-26

### Added
- Release of Client and Core
- Prompt mangement features

## [1.0.0] - 2025-10-07

### Added
- Better documentation
- Official release version 1.0.0

## [0.1.0] - 2025-10-07

### Added
- Initial alpha release
- Core Langfuse OTLP exporter functionality
- `AddLangfuseExporter()` extension method for OpenTelemetry TracerProviderBuilder
- Automatic configuration from environment variables (`LANGFUSE_PUBLIC_KEY`, `LANGFUSE_SECRET_KEY`, `LANGFUSE_BASE_URL`)
- Support for IConfiguration-based configuration
- Code-based configuration override option
- Automatic OTLP endpoint construction with Basic Auth
- Full support for Semantic Kernel GenAI instrumentation
- Sample application demonstrating Semantic Kernel integration

