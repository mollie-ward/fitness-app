using FitnessApp.Domain.Common;

namespace FitnessApp.Domain.Entities;

/// <summary>
/// Represents a user in the fitness application
/// </summary>
public class User : BaseEntity
{
    /// <summary>
    /// User's email address
    /// </summary>
    public required string Email { get; set; }

    /// <summary>
    /// User's display name
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Hashed password for authentication
    /// </summary>
    public string? PasswordHash { get; set; }

    /// <summary>
    /// User's fitness level (e.g., Beginner, Intermediate, Advanced)
    /// </summary>
    public string? FitnessLevel { get; set; }
}
