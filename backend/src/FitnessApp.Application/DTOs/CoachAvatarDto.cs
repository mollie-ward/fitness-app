namespace FitnessApp.Application.DTOs;

/// <summary>
/// DTO for Coach Tom's avatar information
/// </summary>
public class CoachAvatarDto
{
    /// <summary>
    /// URL to the avatar image
    /// </summary>
    public required string AvatarUrl { get; set; }

    /// <summary>
    /// The coach's name
    /// </summary>
    public string Name { get; set; } = "Coach Tom";

    /// <summary>
    /// Brief description or tagline
    /// </summary>
    public string? Description { get; set; }
}
