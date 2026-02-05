namespace FitnessApp.Application.DTOs;

/// <summary>
/// DTO for email verification request
/// </summary>
public class VerifyEmailRequestDto
{
    /// <summary>
    /// Email verification token
    /// </summary>
    public required string Token { get; set; }
}
