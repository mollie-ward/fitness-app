using FitnessApp.Domain.Enums;

namespace FitnessApp.Application.DTOs;

/// <summary>
/// Data transfer object for detailed training plan with all weeks
/// </summary>
public class TrainingPlanDetailDto
{
    /// <summary>
    /// Gets or sets the plan identifier
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the plan name
    /// </summary>
    public string PlanName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the plan start date
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Gets or sets the plan end date
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// Gets or sets the total number of weeks
    /// </summary>
    public int TotalWeeks { get; set; }

    /// <summary>
    /// Gets or sets the training days per week
    /// </summary>
    public int TrainingDaysPerWeek { get; set; }

    /// <summary>
    /// Gets or sets the current week number
    /// </summary>
    public int CurrentWeek { get; set; }

    /// <summary>
    /// Gets or sets the plan status
    /// </summary>
    public PlanStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the primary goal identifier
    /// </summary>
    public Guid? PrimaryGoalId { get; set; }

    /// <summary>
    /// Gets or sets the creation date
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the progress percentage (0-100)
    /// </summary>
    public decimal ProgressPercentage { get; set; }

    /// <summary>
    /// Gets or sets the collection of training weeks
    /// </summary>
    public ICollection<TrainingWeekDto> TrainingWeeks { get; set; } = new List<TrainingWeekDto>();
}
