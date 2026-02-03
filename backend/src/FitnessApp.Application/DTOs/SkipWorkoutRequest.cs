namespace FitnessApp.Application.DTOs;

/// <summary>
/// Request to mark a workout as skipped
/// </summary>
public class SkipWorkoutRequest
{
    /// <summary>
    /// Gets or sets the reason for skipping (optional)
    /// </summary>
    public string? Reason { get; set; }
}
