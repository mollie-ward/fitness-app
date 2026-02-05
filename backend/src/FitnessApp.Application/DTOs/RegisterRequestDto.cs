namespace FitnessApp.Application.DTOs;

/// <summary>
/// DTO for user registration request
/// </summary>
public class RegisterRequestDto
{
    /// <summary>
    /// User's email address
    /// </summary>
    public required string Email { get; set; }

    /// <summary>
    /// User's password
    /// </summary>
    public required string Password { get; set; }

    /// <summary>
    /// User's display name
    /// </summary>
    public required string Name { get; set; }
}
