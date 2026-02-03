namespace FitnessApp.Application.DTOs;

/// <summary>
/// Request to mark a workout as completed
/// </summary>
public class CompleteWorkoutRequest
{
    /// <summary>
    /// Gets or sets the completion timestamp (defaults to current time if not provided)
    /// </summary>
    public DateTime? CompletedAt { get; set; }
}
