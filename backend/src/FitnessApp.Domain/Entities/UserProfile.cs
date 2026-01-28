using FitnessApp.Domain.Common;
using FitnessApp.Domain.Enums;
using FitnessApp.Domain.ValueObjects;

namespace FitnessApp.Domain.Entities;

/// <summary>
/// Represents a user's fitness profile with comprehensive training information
/// </summary>
public class UserProfile : BaseEntity
{
    /// <summary>
    /// Gets or sets the user identifier (from authentication system)
    /// </summary>
    public required Guid UserId { get; set; }

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
    public ScheduleAvailability? ScheduleAvailability { get; set; }

    /// <summary>
    /// Gets or sets the training background
    /// </summary>
    public TrainingBackground? TrainingBackground { get; set; }

    /// <summary>
    /// Gets or sets the collection of training goals
    /// </summary>
    public ICollection<TrainingGoal> TrainingGoals { get; set; } = new List<TrainingGoal>();

    /// <summary>
    /// Gets or sets the collection of injury limitations
    /// </summary>
    public ICollection<InjuryLimitation> InjuryLimitations { get; set; } = new List<InjuryLimitation>();
}
