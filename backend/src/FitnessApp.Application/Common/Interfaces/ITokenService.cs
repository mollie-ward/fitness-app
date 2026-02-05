namespace FitnessApp.Application.Common.Interfaces;

/// <summary>
/// Interface for JWT token generation and validation
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Generates a JWT access token for a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="email">User email</param>
    /// <returns>JWT access token</returns>
    string GenerateAccessToken(Guid userId, string email);

    /// <summary>
    /// Generates a refresh token
    /// </summary>
    /// <returns>Refresh token string</returns>
    string GenerateRefreshToken();

    /// <summary>
    /// Validates a JWT access token and returns the user ID
    /// </summary>
    /// <param name="token">JWT access token</param>
    /// <returns>User ID if valid, null otherwise</returns>
    Guid? ValidateAccessToken(string token);
}
