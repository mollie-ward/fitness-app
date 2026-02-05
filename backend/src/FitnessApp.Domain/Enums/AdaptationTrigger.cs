namespace FitnessApp.Domain.Enums;

/// <summary>
/// Triggers that cause plan adaptations
/// </summary>
public enum AdaptationTrigger
{
    /// <summary>
    /// Adaptation triggered by missed workouts
    /// </summary>
    MissedWorkouts = 0,

    /// <summary>
    /// Adaptation requested by user
    /// </summary>
    UserRequest = 1,

    /// <summary>
    /// Adaptation triggered by injury report
    /// </summary>
    Injury = 2,

    /// <summary>
    /// Adaptation triggered by schedule change
    /// </summary>
    ScheduleChange = 3,

    /// <summary>
    /// Adaptation triggered by goal timeline change
    /// </summary>
    TimelineChange = 4,

    /// <summary>
    /// Adaptation triggered by perceived difficulty pattern
    /// </summary>
    PerceivedDifficulty = 5
}
