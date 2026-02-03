namespace FitnessApp.Application.DTOs;

/// <summary>
/// DTO for overall all-time statistics
/// </summary>
public class OverallStatsDto
{
    /// <summary>
    /// Gets or sets the total number of training days (all-time)
    /// </summary>
    public int TotalTrainingDays { get; set; }

    /// <summary>
    /// Gets or sets the total number of workouts completed
    /// </summary>
    public int TotalWorkoutsCompleted { get; set; }

    /// <summary>
    /// Gets or sets the overall plan completion percentage
    /// </summary>
    public decimal OverallPlanCompletionPercentage { get; set; }

    /// <summary>
    /// Gets or sets the average weekly completion rate
    /// </summary>
    public decimal AverageWeeklyCompletionRate { get; set; }

    /// <summary>
    /// Gets or sets workouts completed this week
    /// </summary>
    public int WorkoutsCompletedThisWeek { get; set; }

    /// <summary>
    /// Gets or sets workouts completed this month
    /// </summary>
    public int WorkoutsCompletedThisMonth { get; set; }

    /// <summary>
    /// Gets or sets the date of the first workout completion
    /// </summary>
    public DateTime? FirstWorkoutDate { get; set; }

    /// <summary>
    /// Gets or sets the date of the most recent workout completion
    /// </summary>
    public DateTime? LastWorkoutDate { get; set; }
}
