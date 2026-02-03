namespace FitnessApp.Application.DTOs;

/// <summary>
/// DTO for workout completion request
/// </summary>
public class WorkoutCompletionDto
{
    /// <summary>
    /// Gets or sets the completion timestamp
    /// </summary>
    public DateTime CompletedAt { get; set; }

    /// <summary>
    /// Gets or sets the actual duration taken to complete the workout in minutes
    /// </summary>
    public int? Duration { get; set; }

    /// <summary>
    /// Gets or sets optional user notes about the workout
    /// </summary>
    public string? Notes { get; set; }
}
