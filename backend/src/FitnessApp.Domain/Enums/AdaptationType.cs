namespace FitnessApp.Domain.Enums;

/// <summary>
/// Type of plan adaptation applied
/// </summary>
public enum AdaptationType
{
    /// <summary>
    /// Intensity adjustment (harder or easier)
    /// </summary>
    Intensity = 0,

    /// <summary>
    /// Schedule redistribution
    /// </summary>
    Schedule = 1,

    /// <summary>
    /// Goal timeline modification
    /// </summary>
    Timeline = 2,

    /// <summary>
    /// Injury accommodation
    /// </summary>
    Injury = 3,

    /// <summary>
    /// Recovery/re-entry adjustment
    /// </summary>
    Recovery = 4
}
