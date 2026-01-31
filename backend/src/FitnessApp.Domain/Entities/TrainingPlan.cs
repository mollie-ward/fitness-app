using FitnessApp.Domain.Common;
using FitnessApp.Domain.Enums;

namespace FitnessApp.Domain.Entities;

/// <summary>
/// Represents a multi-week training plan for a user
/// </summary>
public class TrainingPlan : BaseEntity
{
    /// <summary>
    /// Gets or sets the user identifier (foreign key to UserProfile)
    /// </summary>
    public required Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the plan name
    /// </summary>
    public required string PlanName { get; set; }

    /// <summary>
    /// Gets or sets the plan start date
    /// </summary>
    public required DateTime StartDate { get; set; }

    /// <summary>
    /// Gets or sets the plan end date
    /// </summary>
    public required DateTime EndDate { get; set; }

    /// <summary>
    /// Gets or sets the total number of weeks in the plan
    /// </summary>
    public required int TotalWeeks { get; set; }

    /// <summary>
    /// Gets or sets the number of training days per week
    /// </summary>
    public required int TrainingDaysPerWeek { get; set; }

    /// <summary>
    /// Gets or sets the primary goal for this plan (reference to user's goal)
    /// </summary>
    public Guid? PrimaryGoalId { get; set; }

    /// <summary>
    /// Gets or sets the status of the training plan
    /// </summary>
    public PlanStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the current week number (for tracking progress)
    /// </summary>
    public int CurrentWeek { get; set; }

    /// <summary>
    /// Gets or sets whether this plan is soft-deleted
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// Gets or sets the collection of training weeks in this plan
    /// </summary>
    public ICollection<TrainingWeek> TrainingWeeks { get; set; } = new List<TrainingWeek>();

    /// <summary>
    /// Gets or sets the plan metadata
    /// </summary>
    public PlanMetadata? PlanMetadata { get; set; }

    /// <summary>
    /// Gets or sets the navigation property to the user profile
    /// </summary>
    public UserProfile? UserProfile { get; set; }

    /// <summary>
    /// Gets or sets the navigation property to the primary goal
    /// </summary>
    public TrainingGoal? PrimaryGoal { get; set; }
}
