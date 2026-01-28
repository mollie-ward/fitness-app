using FitnessApp.Domain.Common;
using FitnessApp.Domain.Enums;

namespace FitnessApp.Domain.Entities;

/// <summary>
/// Represents a training goal for a user
/// </summary>
public class TrainingGoal : BaseEntity
{
    /// <summary>
    /// Gets or sets the user profile this goal belongs to
    /// </summary>
    public required Guid UserProfileId { get; set; }

    /// <summary>
    /// Gets or sets the type of goal
    /// </summary>
    public GoalType GoalType { get; set; }

    /// <summary>
    /// Gets or sets the description of the goal
    /// </summary>
    public required string Description { get; set; }

    /// <summary>
    /// Gets or sets the target date for achieving the goal (nullable for open-ended goals)
    /// </summary>
    public DateTime? TargetDate { get; set; }

    /// <summary>
    /// Gets or sets the priority level (1 = highest priority)
    /// </summary>
    public int Priority { get; set; } = 1;

    /// <summary>
    /// Gets or sets the status of the goal
    /// </summary>
    public GoalStatus Status { get; set; } = GoalStatus.Active;

    /// <summary>
    /// Gets or sets the user profile this goal belongs to
    /// </summary>
    public UserProfile? UserProfile { get; set; }

    /// <summary>
    /// Validates the training goal
    /// </summary>
    /// <returns>True if valid, false otherwise</returns>
    public bool IsValid()
    {
        // Target date must be in the future if specified
        if (TargetDate.HasValue && TargetDate.Value.Date <= DateTime.UtcNow.Date)
            return false;

        // Priority must be positive
        if (Priority < 1)
            return false;

        return true;
    }
}
