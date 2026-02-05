namespace FitnessApp.Domain.Enums;

/// <summary>
/// Represents the recognized intent of a user message
/// </summary>
public enum MessageIntent
{
    /// <summary>
    /// Unknown or unrecognized intent
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// User is asking about workout rationale or exercise explanations
    /// </summary>
    WorkoutRationale = 1,

    /// <summary>
    /// User is reporting an injury or physical limitation
    /// </summary>
    InjuryReport = 2,

    /// <summary>
    /// User wants to make the plan harder or easier
    /// </summary>
    PlanModification = 3,

    /// <summary>
    /// User is reporting a change in their schedule/availability
    /// </summary>
    ScheduleChange = 4,

    /// <summary>
    /// User is seeking motivation or encouragement
    /// </summary>
    Motivation = 5,

    /// <summary>
    /// General fitness question
    /// </summary>
    GeneralQuestion = 6,

    /// <summary>
    /// Out-of-scope query (not fitness-related)
    /// </summary>
    OutOfScope = 7
}
