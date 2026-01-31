using FitnessApp.Domain.Common;
using FitnessApp.Domain.Enums;

namespace FitnessApp.Domain.Entities;

/// <summary>
/// Represents an individual workout within a training week
/// </summary>
public class Workout : BaseEntity
{
    /// <summary>
    /// Gets or sets the training week identifier (foreign key)
    /// </summary>
    public required Guid WeekId { get; set; }

    /// <summary>
    /// Gets or sets the day of week for this workout
    /// </summary>
    public required WorkoutDay DayOfWeek { get; set; }

    /// <summary>
    /// Gets or sets the scheduled date for this workout
    /// </summary>
    public required DateTime ScheduledDate { get; set; }

    /// <summary>
    /// Gets or sets the discipline for this workout
    /// </summary>
    public required Discipline Discipline { get; set; }

    /// <summary>
    /// Gets or sets the session type
    /// </summary>
    public SessionType? SessionType { get; set; }

    /// <summary>
    /// Gets or sets the workout name
    /// </summary>
    public required string WorkoutName { get; set; }

    /// <summary>
    /// Gets or sets the detailed workout description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the estimated duration in minutes
    /// </summary>
    public int? EstimatedDuration { get; set; }

    /// <summary>
    /// Gets or sets the intensity level
    /// </summary>
    public IntensityLevel IntensityLevel { get; set; }

    /// <summary>
    /// Gets or sets whether this is a key/milestone workout
    /// </summary>
    public bool IsKeyWorkout { get; set; }

    /// <summary>
    /// Gets or sets the completion status
    /// </summary>
    public CompletionStatus CompletionStatus { get; set; }

    /// <summary>
    /// Gets or sets when the workout was completed (nullable)
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// Gets or sets the navigation property to the training week
    /// </summary>
    public TrainingWeek? TrainingWeek { get; set; }

    /// <summary>
    /// Gets or sets the collection of exercises in this workout
    /// </summary>
    public ICollection<WorkoutExercise> WorkoutExercises { get; set; } = new List<WorkoutExercise>();
}
