namespace FitnessApp.Domain.ValueObjects;

/// <summary>
/// Value object representing a user's weekly training schedule availability
/// </summary>
public class ScheduleAvailability
{
    /// <summary>
    /// Gets or sets whether Monday is available for training
    /// </summary>
    public bool Monday { get; set; }

    /// <summary>
    /// Gets or sets whether Tuesday is available for training
    /// </summary>
    public bool Tuesday { get; set; }

    /// <summary>
    /// Gets or sets whether Wednesday is available for training
    /// </summary>
    public bool Wednesday { get; set; }

    /// <summary>
    /// Gets or sets whether Thursday is available for training
    /// </summary>
    public bool Thursday { get; set; }

    /// <summary>
    /// Gets or sets whether Friday is available for training
    /// </summary>
    public bool Friday { get; set; }

    /// <summary>
    /// Gets or sets whether Saturday is available for training
    /// </summary>
    public bool Saturday { get; set; }

    /// <summary>
    /// Gets or sets whether Sunday is available for training
    /// </summary>
    public bool Sunday { get; set; }

    /// <summary>
    /// Gets or sets the minimum number of training sessions per week
    /// </summary>
    public int MinimumSessionsPerWeek { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of training sessions per week
    /// </summary>
    public int MaximumSessionsPerWeek { get; set; }

    /// <summary>
    /// Gets the total number of available days in the week
    /// </summary>
    public int AvailableDaysCount =>
        (Monday ? 1 : 0) +
        (Tuesday ? 1 : 0) +
        (Wednesday ? 1 : 0) +
        (Thursday ? 1 : 0) +
        (Friday ? 1 : 0) +
        (Saturday ? 1 : 0) +
        (Sunday ? 1 : 0);

    /// <summary>
    /// Validates the schedule availability
    /// </summary>
    /// <returns>True if the schedule is valid, false otherwise</returns>
    public bool IsValid()
    {
        // At least one day must be selected
        if (AvailableDaysCount == 0)
            return false;

        // Min sessions must be at least 1
        if (MinimumSessionsPerWeek < 1)
            return false;

        // Max sessions must be at least equal to min sessions
        if (MaximumSessionsPerWeek < MinimumSessionsPerWeek)
            return false;

        // Max sessions cannot exceed available days
        if (MaximumSessionsPerWeek > AvailableDaysCount)
            return false;

        return true;
    }
}
