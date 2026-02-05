namespace FitnessApp.Application.DTOs;

/// <summary>
/// DTO for requesting goal timeline change
/// </summary>
public class GoalTimelineChangeDto
{
    /// <summary>
    /// Goal identifier
    /// </summary>
    public required Guid GoalId { get; set; }

    /// <summary>
    /// New target date for the goal
    /// </summary>
    public required DateTime NewTargetDate { get; set; }

    /// <summary>
    /// Optional reason for the change
    /// </summary>
    public string? Reason { get; set; }
}
