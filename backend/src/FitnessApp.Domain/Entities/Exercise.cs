using FitnessApp.Domain.Common;
using FitnessApp.Domain.Enums;

namespace FitnessApp.Domain.Entities;

/// <summary>
/// Represents an exercise in the fitness application
/// </summary>
public class Exercise : BaseEntity
{
    /// <summary>
    /// Exercise name
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Detailed description of the exercise
    /// </summary>
    public required string Description { get; set; }

    /// <summary>
    /// Instructions on how to perform the exercise
    /// </summary>
    public string? Instructions { get; set; }

    /// <summary>
    /// Primary discipline for this exercise
    /// </summary>
    public Discipline PrimaryDiscipline { get; set; }

    /// <summary>
    /// Difficulty level of the exercise
    /// </summary>
    public DifficultyLevel DifficultyLevel { get; set; }

    /// <summary>
    /// Approximate duration in minutes
    /// </summary>
    public int? ApproximateDuration { get; set; }

    /// <summary>
    /// Intensity level of the exercise
    /// </summary>
    public IntensityLevel IntensityLevel { get; set; }

    /// <summary>
    /// Session type this exercise is suitable for
    /// </summary>
    public SessionType? SessionType { get; set; }

    /// <summary>
    /// Movement patterns involved in this exercise
    /// </summary>
    public ICollection<ExerciseMovementPattern> ExerciseMovementPatterns { get; set; } = new List<ExerciseMovementPattern>();

    /// <summary>
    /// Muscle groups targeted by this exercise
    /// </summary>
    public ICollection<ExerciseMuscleGroup> ExerciseMuscleGroups { get; set; } = new List<ExerciseMuscleGroup>();

    /// <summary>
    /// Equipment required for this exercise
    /// </summary>
    public ICollection<ExerciseEquipment> ExerciseEquipments { get; set; } = new List<ExerciseEquipment>();

    /// <summary>
    /// Contraindications (injuries that prevent this exercise)
    /// </summary>
    public ICollection<ExerciseContraindication> ExerciseContraindications { get; set; } = new List<ExerciseContraindication>();

    /// <summary>
    /// Progressions where this exercise is the base
    /// </summary>
    public ICollection<ExerciseProgression> ProgressionsAsBase { get; set; } = new List<ExerciseProgression>();

    /// <summary>
    /// Progressions where this exercise is the regression (easier)
    /// </summary>
    public ICollection<ExerciseProgression> ProgressionsAsRegression { get; set; } = new List<ExerciseProgression>();

    /// <summary>
    /// Progressions where this exercise is the progression (harder)
    /// </summary>
    public ICollection<ExerciseProgression> ProgressionsAsProgression { get; set; } = new List<ExerciseProgression>();

    /// <summary>
    /// Progressions where this exercise is an alternative
    /// </summary>
    public ICollection<ExerciseProgression> ProgressionsAsAlternative { get; set; } = new List<ExerciseProgression>();
}
