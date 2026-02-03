using FitnessApp.Domain.Enums;

namespace FitnessApp.Application.DTOs;

/// <summary>
/// Data transfer object for detailed workout with exercises
/// </summary>
public class WorkoutDetailDto
{
    /// <summary>
    /// Gets or sets the workout identifier
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the workout name
    /// </summary>
    public string WorkoutName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the scheduled date
    /// </summary>
    public DateTime ScheduledDate { get; set; }

    /// <summary>
    /// Gets or sets the day of week
    /// </summary>
    public WorkoutDay DayOfWeek { get; set; }

    /// <summary>
    /// Gets or sets the discipline
    /// </summary>
    public Discipline Discipline { get; set; }

    /// <summary>
    /// Gets or sets the session type
    /// </summary>
    public SessionType? SessionType { get; set; }

    /// <summary>
    /// Gets or sets the estimated duration in minutes
    /// </summary>
    public int? EstimatedDuration { get; set; }

    /// <summary>
    /// Gets or sets the intensity level
    /// </summary>
    public IntensityLevel IntensityLevel { get; set; }

    /// <summary>
    /// Gets or sets whether this is a key workout
    /// </summary>
    public bool IsKeyWorkout { get; set; }

    /// <summary>
    /// Gets or sets the completion status
    /// </summary>
    public CompletionStatus CompletionStatus { get; set; }

    /// <summary>
    /// Gets or sets when the workout was completed
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// Gets or sets the collection of exercises
    /// </summary>
    public ICollection<WorkoutExerciseDto> WorkoutExercises { get; set; } = new List<WorkoutExerciseDto>();
}
