namespace FitnessApp.Domain.Enums;

/// <summary>
/// Status of a training plan
/// </summary>
public enum PlanStatus
{
    /// <summary>
    /// Plan is currently active and being followed
    /// </summary>
    Active = 0,

    /// <summary>
    /// Plan has been completed successfully
    /// </summary>
    Completed = 1,

    /// <summary>
    /// Plan was abandoned before completion
    /// </summary>
    Abandoned = 2,

    /// <summary>
    /// Plan is temporarily paused
    /// </summary>
    Paused = 3
}
