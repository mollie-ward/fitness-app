namespace FitnessApp.Application.DTOs;

/// <summary>
/// Data transfer object for exercise within a workout
/// </summary>
public class WorkoutExerciseDto
{
    /// <summary>
    /// Gets or sets the workout exercise identifier
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the exercise identifier
    /// </summary>
    public Guid ExerciseId { get; set; }

    /// <summary>
    /// Gets or sets the exercise name
    /// </summary>
    public string ExerciseName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the order in workout
    /// </summary>
    public int OrderInWorkout { get; set; }

    /// <summary>
    /// Gets or sets the number of sets
    /// </summary>
    public int? Sets { get; set; }

    /// <summary>
    /// Gets or sets the number of reps
    /// </summary>
    public int? Reps { get; set; }

    /// <summary>
    /// Gets or sets the duration in seconds
    /// </summary>
    public int? Duration { get; set; }

    /// <summary>
    /// Gets or sets the rest period in seconds
    /// </summary>
    public int? RestPeriod { get; set; }

    /// <summary>
    /// Gets or sets the intensity guidance
    /// </summary>
    public string? IntensityGuidance { get; set; }

    /// <summary>
    /// Gets or sets additional notes
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets the exercise description
    /// </summary>
    public string? Description { get; set; }
}
