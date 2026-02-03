namespace FitnessApp.Application.DTOs;

/// <summary>
/// DTO for streak information
/// </summary>
public class StreakInfoDto
{
    /// <summary>
    /// Gets or sets the current daily streak (consecutive training days)
    /// </summary>
    public int CurrentStreak { get; set; }

    /// <summary>
    /// Gets or sets the longest daily streak (personal best)
    /// </summary>
    public int LongestStreak { get; set; }

    /// <summary>
    /// Gets or sets the current weekly streak (consecutive weeks)
    /// </summary>
    public int CurrentWeeklyStreak { get; set; }

    /// <summary>
    /// Gets or sets the longest weekly streak
    /// </summary>
    public int LongestWeeklyStreak { get; set; }

    /// <summary>
    /// Gets or sets the date of the last workout
    /// </summary>
    public DateTime? LastWorkoutDate { get; set; }

    /// <summary>
    /// Gets or sets the milestone progress (days until next milestone)
    /// </summary>
    public int DaysUntilNextMilestone { get; set; }

    /// <summary>
    /// Gets or sets the next milestone target (7, 14, 30, etc.)
    /// </summary>
    public int NextMilestone { get; set; }
}
