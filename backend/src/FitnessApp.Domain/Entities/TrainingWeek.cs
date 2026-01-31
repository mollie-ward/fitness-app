using FitnessApp.Domain.Common;
using FitnessApp.Domain.Enums;

namespace FitnessApp.Domain.Entities;

/// <summary>
/// Represents a week within a training plan
/// </summary>
public class TrainingWeek : BaseEntity
{
    /// <summary>
    /// Gets or sets the training plan identifier (foreign key)
    /// </summary>
    public required Guid PlanId { get; set; }

    /// <summary>
    /// Gets or sets the week number within the plan (1, 2, 3...)
    /// </summary>
    public required int WeekNumber { get; set; }

    /// <summary>
    /// Gets or sets the training phase for this week
    /// </summary>
    public required TrainingPhase Phase { get; set; }

    /// <summary>
    /// Gets or sets the total estimated weekly volume in minutes
    /// </summary>
    public int? WeeklyVolume { get; set; }

    /// <summary>
    /// Gets or sets the intensity level for this week
    /// </summary>
    public IntensityLevel IntensityLevel { get; set; }

    /// <summary>
    /// Gets or sets the focus area for this week
    /// </summary>
    public string? FocusArea { get; set; }

    /// <summary>
    /// Gets or sets the start date of this week
    /// </summary>
    public required DateTime StartDate { get; set; }

    /// <summary>
    /// Gets or sets the end date of this week
    /// </summary>
    public required DateTime EndDate { get; set; }

    /// <summary>
    /// Gets or sets the navigation property to the training plan
    /// </summary>
    public TrainingPlan? TrainingPlan { get; set; }

    /// <summary>
    /// Gets or sets the collection of workouts in this week
    /// </summary>
    public ICollection<Workout> Workouts { get; set; } = new List<Workout>();
}
