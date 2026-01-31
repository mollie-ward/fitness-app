using FitnessApp.Domain.Common;

namespace FitnessApp.Domain.Entities;

/// <summary>
/// Represents equipment required for exercises
/// </summary>
public class Equipment : BaseEntity
{
    /// <summary>
    /// Name of the equipment (e.g., "Barbell", "Dumbbells", "Ski Erg")
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Description of the equipment
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Exercises that require this equipment
    /// </summary>
    public ICollection<ExerciseEquipment> ExerciseEquipments { get; set; } = new List<ExerciseEquipment>();
}
