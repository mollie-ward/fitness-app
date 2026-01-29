using FitnessApp.Domain.Enums;

namespace FitnessApp.Application.DTOs;

/// <summary>
/// Data transfer object for training goals
/// </summary>
public class TrainingGoalDto
{
    /// <summary>
    /// Gets or sets the goal identifier
    /// </summary>
    public Guid? Id { get; set; }

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
}
