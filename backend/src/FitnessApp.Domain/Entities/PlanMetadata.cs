using FitnessApp.Domain.Common;

namespace FitnessApp.Domain.Entities;

/// <summary>
/// Represents metadata about how a training plan was generated
/// </summary>
public class PlanMetadata : BaseEntity
{
    /// <summary>
    /// Gets or sets the training plan identifier (foreign key)
    /// </summary>
    public required Guid PlanId { get; set; }

    /// <summary>
    /// Gets or sets the algorithm version used to generate the plan
    /// </summary>
    public string? AlgorithmVersion { get; set; }

    /// <summary>
    /// Gets or sets the user profile snapshot at generation time (JSON)
    /// </summary>
    public string? UserProfileSnapshot { get; set; }

    /// <summary>
    /// Gets or sets the modifications history (JSON)
    /// </summary>
    public string? ModificationsHistory { get; set; }

    /// <summary>
    /// Gets or sets the generation parameters (JSON)
    /// </summary>
    public string? GenerationParameters { get; set; }

    /// <summary>
    /// Gets or sets the navigation property to the training plan
    /// </summary>
    public TrainingPlan? TrainingPlan { get; set; }
}
