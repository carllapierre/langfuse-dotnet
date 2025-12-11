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
}

