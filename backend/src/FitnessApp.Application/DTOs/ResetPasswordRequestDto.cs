namespace FitnessApp.Application.DTOs;

/// <summary>
/// DTO for reset password request
/// </summary>
public class ResetPasswordRequestDto
{
    /// <summary>
    /// Password reset token
    /// </summary>
    public required string Token { get; set; }

    /// <summary>
    /// New password
    /// </summary>
    public required string NewPassword { get; set; }
}
