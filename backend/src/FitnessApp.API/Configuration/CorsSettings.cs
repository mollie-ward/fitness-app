namespace FitnessApp.API.Configuration;

/// <summary>
/// CORS configuration settings
/// </summary>
public class CorsSettings
{
    /// <summary>
    /// List of allowed origins for CORS
    /// </summary>
    public string[] AllowedOrigins { get; set; } = Array.Empty<string>();
}
