using FitnessApp.Domain.Enums;

namespace FitnessApp.Application.DTOs;

/// <summary>
/// DTO for plan adaptation result
/// </summary>
public class PlanAdaptationResultDto
{
    /// <summary>
    /// Unique identifier for the adaptation
    /// </summary>
    public Guid AdaptationId { get; set; }

    /// <summary>
    /// Plan identifier
    /// </summary>
    public Guid PlanId { get; set; }

    /// <summary>
    /// Trigger that caused the adaptation
    /// </summary>
    public AdaptationTrigger Trigger { get; set; }

    /// <summary>
    /// Type of adaptation applied
    /// </summary>
    public AdaptationType Type { get; set; }

    /// <summary>
    /// Description of what changed
    /// </summary>
    public required string Description { get; set; }

    /// <summary>
    /// Number of workouts affected
    /// </summary>
    public int WorkoutsAffected { get; set; }

    /// <summary>
    /// When the adaptation was applied
    /// </summary>
    public DateTime AppliedAt { get; set; }

    /// <summary>
    /// Whether the adaptation was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Any warnings or notes about the adaptation
    /// </summary>
    public string? Warnings { get; set; }
}
