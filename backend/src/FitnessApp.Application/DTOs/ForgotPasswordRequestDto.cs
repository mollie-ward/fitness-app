namespace FitnessApp.Application.DTOs;

/// <summary>
/// DTO for forgot password request
/// </summary>
public class ForgotPasswordRequestDto
{
    /// <summary>
    /// User's email address
    /// </summary>
    public required string Email { get; set; }
}
