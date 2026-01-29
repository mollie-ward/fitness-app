namespace FitnessApp.Application.DTOs;

/// <summary>
/// Data transfer object for schedule availability
/// </summary>
public class ScheduleAvailabilityDto
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
}
