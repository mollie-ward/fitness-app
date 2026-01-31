using FitnessApp.Domain.Common;

namespace FitnessApp.Domain.Entities;

/// <summary>
/// Represents a muscle group targeted by exercises
/// </summary>
public class MuscleGroup : BaseEntity
{
    /// <summary>
    /// Name of the muscle group (e.g., "Quadriceps", "Chest", "Core")
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Description of the muscle group
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Broad category (e.g., "Upper Body", "Lower Body", "Core", "Full Body")
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// Exercises that target this muscle group
    /// </summary>
    public ICollection<ExerciseMuscleGroup> ExerciseMuscleGroups { get; set; } = new List<ExerciseMuscleGroup>();
}
