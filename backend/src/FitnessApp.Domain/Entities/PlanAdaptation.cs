using FitnessApp.Domain.Common;
using FitnessApp.Domain.Enums;

namespace FitnessApp.Domain.Entities;

/// <summary>
/// Represents an adaptation made to a training plan
/// Maintains an audit trail of all plan modifications
/// </summary>
public class PlanAdaptation : BaseEntity
{
    /// <summary>
    /// Gets or sets the training plan identifier (foreign key)
    /// </summary>
    public required Guid PlanId { get; set; }

    /// <summary>
    /// Gets or sets the trigger that caused this adaptation
    /// </summary>
    public required AdaptationTrigger Trigger { get; set; }

    /// <summary>
    /// Gets or sets the type of adaptation applied
    /// </summary>
    public required AdaptationType Type { get; set; }

    /// <summary>
    /// Gets or sets a human-readable description of the adaptation
    /// </summary>
    public required string Description { get; set; }

    /// <summary>
    /// Gets or sets the changes applied (JSON or structured data)
    /// </summary>
    public string? ChangesApplied { get; set; }

    /// <summary>
    /// Gets or sets when the adaptation was applied
    /// </summary>
    public DateTime AppliedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the navigation property to the training plan
    /// </summary>
    public TrainingPlan? TrainingPlan { get; set; }
}
