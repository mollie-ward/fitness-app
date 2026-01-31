using FitnessApp.Domain.Common;

namespace FitnessApp.Domain.Entities;

/// <summary>
/// Represents the relationship between a workout and an exercise with workout-specific parameters
/// </summary>
public class WorkoutExercise : BaseEntity
{
    /// <summary>
    /// Gets or sets the workout identifier (foreign key)
    /// </summary>
    public required Guid WorkoutId { get; set; }

    /// <summary>
    /// Gets or sets the exercise identifier (foreign key)
    /// </summary>
    public required Guid ExerciseId { get; set; }

    /// <summary>
    /// Gets or sets the order of this exercise in the workout
    /// </summary>
    public required int OrderInWorkout { get; set; }

    /// <summary>
    /// Gets or sets the number of sets
    /// </summary>
    public int? Sets { get; set; }

    /// <summary>
    /// Gets or sets the number of reps
    /// </summary>
    public int? Reps { get; set; }

    /// <summary>
    /// Gets or sets the duration in seconds (for time-based exercises)
    /// </summary>
    public int? Duration { get; set; }

    /// <summary>
    /// Gets or sets the rest period in seconds between sets
    /// </summary>
    public int? RestPeriod { get; set; }

    /// <summary>
    /// Gets or sets the intensity guidance (e.g., "70% max", "RPE 7", "Easy pace")
    /// </summary>
    public string? IntensityGuidance { get; set; }

    /// <summary>
    /// Gets or sets additional notes or instructions
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets the navigation property to the workout
    /// </summary>
    public Workout? Workout { get; set; }

    /// <summary>
    /// Gets or sets the navigation property to the exercise
    /// </summary>
    public Exercise? Exercise { get; set; }
}
