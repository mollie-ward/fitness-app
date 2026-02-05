using FitnessApp.Domain.Enums;

namespace FitnessApp.Application.DTOs;

/// <summary>
/// Data transfer object for updating injury status
/// </summary>
public class UpdateInjuryStatusDto
{
    /// <summary>
    /// New status for the injury
    /// </summary>
    public InjuryStatus Status { get; set; }

    /// <summary>
    /// Optional notes about the status update
    /// </summary>
    public string? Notes { get; set; }
}
