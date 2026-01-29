namespace FitnessApp.Application.DTOs;

/// <summary>
/// Data transfer object for training background
/// </summary>
public class TrainingBackgroundDto
{
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
}
