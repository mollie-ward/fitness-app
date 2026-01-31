using FitnessApp.Domain.Common;

namespace FitnessApp.Domain.Entities;

/// <summary>
/// Join entity for Exercise and Equipment many-to-many relationship
/// </summary>
public class ExerciseEquipment : BaseEntity
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
    /// Equipment ID
    /// </summary>
    public Guid EquipmentId { get; set; }

    /// <summary>
    /// Equipment navigation property
    /// </summary>
    public Equipment Equipment { get; set; } = null!;

    /// <summary>
    /// Indicates if this equipment is required or optional
    /// </summary>
    public bool IsRequired { get; set; } = true;
}
