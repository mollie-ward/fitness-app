using FitnessApp.Domain.Common;

namespace FitnessApp.Domain.Entities;

/// <summary>
/// Join entity for Exercise and MuscleGroup many-to-many relationship
/// </summary>
public class ExerciseMuscleGroup : BaseEntity
{
    /// <summary>
    /// Exercise ID
    /// </summary>
    public Guid ExerciseId { get; set; }

    /// <summary>
    /// Exercise navigation property
    /// </summary>
    public Exercise Exercise { get; set; } = null!;

    /// <summary>
    /// Muscle group ID
    /// </summary>
    public Guid MuscleGroupId { get; set; }

    /// <summary>
    /// Muscle group navigation property
    /// </summary>
    public MuscleGroup MuscleGroup { get; set; } = null!;

    /// <summary>
    /// Indicates if this is a primary muscle group (vs secondary/stabilizer)
    /// </summary>
    public bool IsPrimary { get; set; }
}
