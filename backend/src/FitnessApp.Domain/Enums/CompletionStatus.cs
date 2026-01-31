namespace FitnessApp.Domain.Enums;

/// <summary>
/// Completion status of a workout
/// </summary>
public enum CompletionStatus
{
    /// <summary>
    /// Workout has not been started
    /// </summary>
    NotStarted = 0,

    /// <summary>
    /// Workout is currently in progress
    /// </summary>
    InProgress = 1,

    /// <summary>
    /// Workout has been completed
    /// </summary>
    Completed = 2,

    /// <summary>
    /// Workout was intentionally skipped
    /// </summary>
    Skipped = 3,

    /// <summary>
    /// Workout was missed (not completed on scheduled date)
    /// </summary>
    Missed = 4
}
