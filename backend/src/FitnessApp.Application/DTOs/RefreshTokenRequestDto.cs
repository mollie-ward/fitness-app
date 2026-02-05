namespace FitnessApp.Application.DTOs;

/// <summary>
/// DTO for refresh token request
/// </summary>
public class RefreshTokenRequestDto
{
    /// <summary>
    /// Refresh token
    /// </summary>
    public required string RefreshToken { get; set; }
}
