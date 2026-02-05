using FitnessApp.Domain.Enums;

namespace FitnessApp.Application.DTOs;

/// <summary>
/// DTO for requesting intensity adjustment
/// </summary>
public class IntensityAdjustmentDto
{
    /// <summary>
    /// Direction of adjustment (Harder or Easier)
    /// </summary>
    public required IntensityDirection Direction { get; set; }

    /// <summary>
    /// Optional reason for the adjustment
    /// </summary>
    public string? Reason { get; set; }
}
