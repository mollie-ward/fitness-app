namespace FitnessApp.Domain.Enums;

/// <summary>
/// Status of a training goal
/// </summary>
public enum GoalStatus
{
    /// <summary>
    /// Goal is active and being worked towards
    /// </summary>
    Active = 0,

    /// <summary>
    /// Goal has been completed
    /// </summary>
    Completed = 1,

    /// <summary>
    /// Goal has been abandoned
    /// </summary>
    Abandoned = 2
}
