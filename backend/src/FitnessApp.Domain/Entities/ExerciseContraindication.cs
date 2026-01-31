using FitnessApp.Domain.Common;

namespace FitnessApp.Domain.Entities;

/// <summary>
/// Join entity for Exercise and Contraindication many-to-many relationship
/// </summary>
public class ExerciseContraindication : BaseEntity
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
    /// Contraindication ID
    /// </summary>
    public Guid ContraindicationId { get; set; }

    /// <summary>
    /// Contraindication navigation property
    /// </summary>
    public Contraindication Contraindication { get; set; } = null!;

    /// <summary>
    /// Severity of contraindication (e.g., "Absolute", "Relative")
    /// </summary>
    public string? Severity { get; set; }

    /// <summary>
    /// Recommended substitute exercise IDs
    /// </summary>
    public string? RecommendedSubstitutes { get; set; }
}
