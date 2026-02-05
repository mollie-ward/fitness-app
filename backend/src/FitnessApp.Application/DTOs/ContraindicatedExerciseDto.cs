namespace FitnessApp.Application.DTOs;

/// <summary>
/// Data transfer object for contraindicated exercise information
/// </summary>
public class ContraindicatedExerciseDto
{
    /// <summary>
    /// Exercise identifier
    /// </summary>
    public Guid ExerciseId { get; set; }

    /// <summary>
    /// Exercise name
    /// </summary>
    public required string ExerciseName { get; set; }

    /// <summary>
    /// Reason for contraindication (injury-related)
    /// </summary>
    public required string Reason { get; set; }

    /// <summary>
    /// Body part causing the contraindication
    /// </summary>
    public required string AffectedBodyPart { get; set; }

    /// <summary>
    /// Severity of the contraindication
    /// </summary>
    public string? Severity { get; set; }
}
