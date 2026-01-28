namespace FitnessApp.API.Configuration;

/// <summary>
/// JWT authentication settings
/// </summary>
public class JwtSettings
{
    /// <summary>
    /// Secret key for signing JWT tokens
    /// </summary>
    public required string Secret { get; set; }

    /// <summary>
    /// Issuer of the JWT token
    /// </summary>
    public required string Issuer { get; set; }

    /// <summary>
    /// Intended audience of the JWT token
    /// </summary>
    public required string Audience { get; set; }

    /// <summary>
    /// Token expiration time in minutes
    /// </summary>
    public int ExpirationInMinutes { get; set; } = 60;
}
