using FitnessApp.Domain.Common;

namespace FitnessApp.Domain.Entities;

/// <summary>
/// Represents an injury or condition that contraindicates certain exercises
/// </summary>
public class Contraindication : BaseEntity
{
    /// <summary>
    /// Type of injury or body part affected (e.g., "Shoulder", "Knee", "Lower Back")
    /// </summary>
    public required string InjuryType { get; set; }

    /// <summary>
    /// Description of the contraindication
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Movement restriction (e.g., "Overhead", "Impact", "Heavy Load")
    /// </summary>
    public string? MovementRestriction { get; set; }

    /// <summary>
    /// Exercises contraindicated by this injury
    /// </summary>
    public ICollection<ExerciseContraindication> ExerciseContraindications { get; set; } = new List<ExerciseContraindication>();
}
