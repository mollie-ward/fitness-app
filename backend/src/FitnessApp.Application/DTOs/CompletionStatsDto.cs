namespace FitnessApp.Application.DTOs;

/// <summary>
/// DTO for completion statistics (weekly or monthly)
/// </summary>
public class CompletionStatsDto
{
    /// <summary>
    /// Gets or sets the number of workouts completed in the period
    /// </summary>
    public int CompletedCount { get; set; }

    /// <summary>
    /// Gets or sets the total number of scheduled workouts in the period
    /// </summary>
    public int TotalScheduled { get; set; }

    /// <summary>
    /// Gets or sets the completion percentage
    /// </summary>
    public decimal CompletionPercentage { get; set; }

    /// <summary>
    /// Gets or sets the start date of the period
    /// </summary>
    public DateTime PeriodStart { get; set; }

    /// <summary>
    /// Gets or sets the end date of the period
    /// </summary>
    public DateTime PeriodEnd { get; set; }
}
