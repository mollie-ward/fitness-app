using FitnessApp.Domain.Common;

namespace FitnessApp.Domain.Entities;

/// <summary>
/// Represents a user's training background and history
/// </summary>
public class TrainingBackground : BaseEntity
{
    /// <summary>
    /// Gets or sets the user profile this training background belongs to
    /// </summary>
    public required Guid UserProfileId { get; set; }

    /// <summary>
    /// Gets or sets whether the user has previous structured training experience
    /// </summary>
    public bool HasStructuredTrainingExperience { get; set; }

    /// <summary>
    /// Gets or sets details about previous training experience
    /// </summary>
    public string? PreviousTrainingDetails { get; set; }

    /// <summary>
    /// Gets or sets the user's equipment familiarity
    /// </summary>
    public string? EquipmentFamiliarity { get; set; }

    /// <summary>
    /// Gets or sets additional training history details
    /// </summary>
    public string? TrainingHistoryDetails { get; set; }

    /// <summary>
    /// Gets or sets the user profile this training background belongs to
    /// </summary>
    public UserProfile? UserProfile { get; set; }
}
