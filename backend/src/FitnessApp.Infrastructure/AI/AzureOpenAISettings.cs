namespace FitnessApp.Infrastructure.AI;

/// <summary>
/// Configuration settings for Azure OpenAI service
/// </summary>
public class AzureOpenAISettings
{
    /// <summary>
    /// Configuration section name
    /// </summary>
    public const string SectionName = "AzureOpenAI";

    /// <summary>
    /// Azure OpenAI endpoint URL
    /// </summary>
    public string Endpoint { get; set; } = string.Empty;

    /// <summary>
    /// API key for authentication (can be stored in Azure Key Vault)
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Deployment name for the GPT model
    /// </summary>
    public string DeploymentName { get; set; } = "gpt-4";

    /// <summary>
    /// Maximum number of tokens in the context window
    /// </summary>
    public int MaxTokens { get; set; } = 8000;

    /// <summary>
    /// Temperature for response generation (0.0-2.0)
    /// </summary>
    public double Temperature { get; set; } = 0.7;

    /// <summary>
    /// Maximum number of retries for failed requests
    /// </summary>
    public int MaxRetries { get; set; } = 3;

    /// <summary>
    /// Timeout in seconds for API requests
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Enable or disable LLM client (useful for testing)
    /// </summary>
    public bool Enabled { get; set; } = true;
}
