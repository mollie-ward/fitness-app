using FitnessApp.Domain.Enums;

namespace FitnessApp.Application.DTOs;

/// <summary>
/// Data transfer object for substitute exercise recommendation
/// </summary>
public class SubstituteExerciseDto
{
    /// <summary>
    /// Original exercise identifier
    /// </summary>
    public Guid OriginalExerciseId { get; set; }

    /// <summary>
    /// Original exercise name
    /// </summary>
    public required string OriginalExerciseName { get; set; }

    /// <summary>
    /// Substitute exercise identifier
    /// </summary>
    public Guid SubstituteExerciseId { get; set; }

    /// <summary>
    /// Substitute exercise name
    /// </summary>
    public required string SubstituteExerciseName { get; set; }

    /// <summary>
    /// Reason for substitution
    /// </summary>
    public required string Reason { get; set; }

    /// <summary>
    /// Description of the substitute exercise
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Difficulty level of the substitute
    /// </summary>
    public DifficultyLevel DifficultyLevel { get; set; }

    /// <summary>
    /// Instructions for performing the substitute
    /// </summary>
    public string? Instructions { get; set; }
}
