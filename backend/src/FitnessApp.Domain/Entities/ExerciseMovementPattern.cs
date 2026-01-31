using FitnessApp.Domain.Common;
using FitnessApp.Domain.Enums;

namespace FitnessApp.Domain.Entities;

/// <summary>
/// Join entity for Exercise and MovementPattern many-to-many relationship
/// </summary>
public class ExerciseMovementPattern : BaseEntity
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
    /// Movement pattern
    /// </summary>
    public MovementPattern MovementPattern { get; set; }

    /// <summary>
    /// Indicates if this is the primary movement pattern for the exercise
    /// </summary>
    public bool IsPrimary { get; set; }
}
