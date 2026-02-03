namespace FitnessApp.Application.DTOs;

/// <summary>
/// DTO for historical completion data
/// </summary>
public class CompletionHistoryDto
{
    /// <summary>
    /// Gets or sets the completion date
    /// </summary>
    public DateTime CompletedAt { get; set; }

    /// <summary>
    /// Gets or sets the workout ID
    /// </summary>
    public Guid WorkoutId { get; set; }

    /// <summary>
    /// Gets or sets the workout name
    /// </summary>
    public string WorkoutName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the discipline
    /// </summary>
    public string Discipline { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the duration in minutes
    /// </summary>
    public int? Duration { get; set; }

    /// <summary>
    /// Gets or sets optional notes
    /// </summary>
    public string? Notes { get; set; }
}
