namespace FitnessApp.Application.DTOs;

/// <summary>
/// DTO for change password request
/// </summary>
public class ChangePasswordRequestDto
{
    /// <summary>
    /// Current password
    /// </summary>
    public required string CurrentPassword { get; set; }

    /// <summary>
    /// New password
    /// </summary>
    public required string NewPassword { get; set; }
}
