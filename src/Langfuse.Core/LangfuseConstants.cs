namespace Langfuse.Core;

/// <summary>
/// Constants used throughout the Langfuse SDK.
/// </summary>
public static class LangfuseConstants
{
    /// <summary>
    /// Default Langfuse Cloud URL (EU region).
    /// </summary>
    public const string DefaultBaseUrl = "https://cloud.langfuse.com";

    /// <summary>
    /// Langfuse Cloud US region URL.
    /// </summary>
    public const string UsCloudUrl = "https://us.cloud.langfuse.com";

    /// <summary>
    /// Environment variable name for Langfuse public key.
    /// </summary>
    public const string EnvPublicKey = "LANGFUSE_PUBLIC_KEY";

    /// <summary>
    /// Environment variable name for Langfuse secret key.
    /// </summary>
    public const string EnvSecretKey = "LANGFUSE_SECRET_KEY";

    /// <summary>
    /// Environment variable name for Langfuse base URL.
    /// </summary>
    public const string EnvBaseUrl = "LANGFUSE_BASE_URL";

    /// <summary>
    /// Configuration section name for Langfuse settings.
    /// </summary>
    public const string ConfigurationSectionName = "Langfuse";

    /// <summary>
    /// API path prefix for public API endpoints.
    /// </summary>
    public const string ApiPublicPath = "/api/public";

    /// <summary>
    /// OTEL traces endpoint path.
    /// </summary>
    public const string OtelTracesPath = "/api/public/otel/v1/traces";

    /// <summary>
    /// Prompts API endpoint path.
    /// </summary>
    public const string PromptsPath = "/api/public/v2/prompts";

    /// <summary>
    /// Scores API endpoint path.
    /// </summary>
    public const string ScoresPath = "/api/public/scores";

    /// <summary>
    /// Traces API endpoint path.
    /// </summary>
    public const string TracesPath = "/api/public/traces";

    /// <summary>
    /// Datasets API endpoint path (v2).
    /// </summary>
    public const string DatasetsPath = "/api/public/v2/datasets";

    /// <summary>
    /// Dataset Items API endpoint path.
    /// </summary>
    public const string DatasetItemsPath = "/api/public/dataset-items";

    /// <summary>
    /// Dataset Run Items API endpoint path.
    /// </summary>
    public const string DatasetRunItemsPath = "/api/public/dataset-run-items";

    /// <summary>
    /// Dataset Runs API base path (note: uses v1 API, not v2).
    /// Format: /api/public/datasets/{datasetName}/runs/{runName}
    /// </summary>
    public const string DatasetRunsBasePath = "/api/public/datasets";
}

