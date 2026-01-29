using FitnessApp.Domain.Enums;

namespace FitnessApp.Application.DTOs;

/// <summary>
/// Data transfer object for user profile with comprehensive training information
/// </summary>
public class UserProfileDto
{
    /// <summary>
    /// Gets or sets the profile identifier
    /// </summary>
    public Guid? Id { get; set; }

    /// <summary>
    /// Gets or sets the user's name
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Gets or sets the user's email address
    /// </summary>
    public required string Email { get; set; }

    /// <summary>
    /// Gets or sets the fitness level for HYROX training
    /// </summary>
    public FitnessLevel HyroxLevel { get; set; }

    /// <summary>
    /// Gets or sets the fitness level for running
    /// </summary>
    public FitnessLevel RunningLevel { get; set; }

    /// <summary>
    /// Gets or sets the fitness level for strength training
    /// </summary>
    public FitnessLevel StrengthLevel { get; set; }

    /// <summary>
    /// Gets or sets the user's schedule availability
    /// </summary>
    public ScheduleAvailabilityDto? ScheduleAvailability { get; set; }

    /// <summary>
    /// Gets or sets the training background
    /// </summary>
    public TrainingBackgroundDto? TrainingBackground { get; set; }

    /// <summary>
    /// Gets or sets the collection of training goals
    /// </summary>
    public ICollection<TrainingGoalDto> TrainingGoals { get; set; } = new List<TrainingGoalDto>();

    /// <summary>
    /// Gets or sets the collection of injury limitations
    /// </summary>
    public ICollection<InjuryLimitationDto> InjuryLimitations { get; set; } = new List<InjuryLimitationDto>();
}
