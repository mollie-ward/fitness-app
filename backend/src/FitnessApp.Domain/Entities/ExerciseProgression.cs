using FitnessApp.Domain.Common;

namespace FitnessApp.Domain.Entities;

/// <summary>
/// Represents progression, regression, and alternative relationships between exercises
/// </summary>
public class ExerciseProgression : BaseEntity
{
    /// <summary>
    /// The base exercise
    /// </summary>
    public Guid BaseExerciseId { get; set; }

    /// <summary>
    /// The base exercise navigation property
    /// </summary>
    public Exercise BaseExercise { get; set; } = null!;

    /// <summary>
    /// The easier variation (regression)
    /// </summary>
    public Guid? RegressionExerciseId { get; set; }

    /// <summary>
    /// The easier variation navigation property
    /// </summary>
    public Exercise? RegressionExercise { get; set; }

    /// <summary>
    /// The harder variation (progression)
    /// </summary>
    public Guid? ProgressionExerciseId { get; set; }

    /// <summary>
    /// The harder variation navigation property
    /// </summary>
    public Exercise? ProgressionExercise { get; set; }

    /// <summary>
    /// Alternative exercise with similar stimulus
    /// </summary>
    public Guid? AlternativeExerciseId { get; set; }

    /// <summary>
    /// Alternative exercise navigation property
    /// </summary>
    public Exercise? AlternativeExercise { get; set; }

    /// <summary>
    /// Notes about the progression
    /// </summary>
    public string? Notes { get; set; }
}
