namespace FitnessApp.Application.DTOs;

/// <summary>
/// DTO for authentication token response
/// </summary>
public class TokenResponseDto
{
    /// <summary>
    /// JWT access token
    /// </summary>
    public required string AccessToken { get; set; }

    /// <summary>
    /// Refresh token for obtaining new access tokens
    /// </summary>
    public required string RefreshToken { get; set; }

    /// <summary>
    /// Token expiration time in seconds
    /// </summary>
    public required int ExpiresIn { get; set; }

    /// <summary>
    /// User information
    /// </summary>
    public required UserDto User { get; set; }
}

/// <summary>
/// DTO for user information
/// </summary>
public class UserDto
{
    /// <summary>
    /// User's unique identifier
    /// </summary>
    public required Guid Id { get; set; }

    /// <summary>
    /// User's email address
    /// </summary>
    public required string Email { get; set; }

    /// <summary>
    /// User's display name
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Indicates whether the user's email has been verified
    /// </summary>
    public required bool EmailVerified { get; set; }
}
