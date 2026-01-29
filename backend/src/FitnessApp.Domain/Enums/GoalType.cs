namespace FitnessApp.Domain.Enums;

/// <summary>
/// Types of training goals users can set
/// </summary>
public enum GoalType
{
    /// <summary>
    /// Goal to complete a HYROX race
    /// </summary>
    HyroxRace = 0,

    /// <summary>
    /// Goal to achieve a specific running distance
    /// </summary>
    RunningDistance = 1,

    /// <summary>
    /// Goal to achieve a strength milestone
    /// </summary>
    StrengthMilestone = 2,

    /// <summary>
    /// General fitness improvement goal
    /// </summary>
    GeneralFitness = 3
}
