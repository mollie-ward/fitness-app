namespace FitnessApp.Domain.Enums;

/// <summary>
/// Current status of an injury
/// </summary>
public enum InjuryStatus
{
    /// <summary>
    /// Injury is currently active
    /// </summary>
    Active = 0,

    /// <summary>
    /// Injury is improving
    /// </summary>
    Improving = 1,

    /// <summary>
    /// Injury is resolved
    /// </summary>
    Resolved = 2
}
