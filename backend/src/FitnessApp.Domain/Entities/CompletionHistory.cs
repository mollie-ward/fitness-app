using FitnessApp.Domain.Common;

namespace FitnessApp.Domain.Entities;

/// <summary>
/// Represents a workout completion record for tracking user progress
/// </summary>
public class CompletionHistory : BaseEntity
{
    /// <summary>
    /// Gets or sets the user identifier who completed the workout
    /// </summary>
    public required Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the workout identifier that was completed
    /// </summary>
    public required Guid WorkoutId { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the workout was completed
    /// </summary>
    public required DateTime CompletedAt { get; set; }

    /// <summary>
    /// Gets or sets the actual duration taken to complete the workout in minutes
    /// </summary>
    public int? Duration { get; set; }

    /// <summary>
    /// Gets or sets optional user feedback or notes about the workout
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets the navigation property to the user profile
    /// </summary>
    public UserProfile? UserProfile { get; set; }

    /// <summary>
    /// Gets or sets the navigation property to the workout
    /// </summary>
    public Workout? Workout { get; set; }
}
