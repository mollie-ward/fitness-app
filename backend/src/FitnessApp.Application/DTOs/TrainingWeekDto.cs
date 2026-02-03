using FitnessApp.Domain.Enums;

namespace FitnessApp.Application.DTOs;

/// <summary>
/// Data transfer object for training week with workouts
/// </summary>
public class TrainingWeekDto
{
    /// <summary>
    /// Gets or sets the week identifier
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the week number
    /// </summary>
    public int WeekNumber { get; set; }

    /// <summary>
    /// Gets or sets the training phase
    /// </summary>
    public TrainingPhase Phase { get; set; }

    /// <summary>
    /// Gets or sets the weekly volume in minutes
    /// </summary>
    public int? WeeklyVolume { get; set; }

    /// <summary>
    /// Gets or sets the intensity level
    /// </summary>
    public IntensityLevel IntensityLevel { get; set; }

    /// <summary>
    /// Gets or sets the focus area
    /// </summary>
    public string? FocusArea { get; set; }

    /// <summary>
    /// Gets or sets the week start date
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Gets or sets the week end date
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// Gets or sets the collection of workouts
    /// </summary>
    public ICollection<WorkoutSummaryDto> Workouts { get; set; } = new List<WorkoutSummaryDto>();
}
