using FitnessApp.Domain.Common;

namespace FitnessApp.Domain.Entities;

/// <summary>
/// Represents user training streak information
/// </summary>
public class UserStreak : BaseEntity
{
    /// <summary>
    /// Gets or sets the user identifier
    /// </summary>
    public required Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the current streak in days
    /// </summary>
    public int CurrentStreak { get; set; }

    /// <summary>
    /// Gets or sets the longest streak (personal best) in days
    /// </summary>
    public int LongestStreak { get; set; }

    /// <summary>
    /// Gets or sets the date of the last completed workout
    /// </summary>
    public DateTime? LastWorkoutDate { get; set; }

    /// <summary>
    /// Gets or sets the current weekly streak (consecutive weeks with minimum workouts)
    /// </summary>
    public int CurrentWeeklyStreak { get; set; }

    /// <summary>
    /// Gets or sets the longest weekly streak
    /// </summary>
    public int LongestWeeklyStreak { get; set; }

    /// <summary>
    /// Gets or sets the navigation property to the user profile
    /// </summary>
    public UserProfile? UserProfile { get; set; }
}
