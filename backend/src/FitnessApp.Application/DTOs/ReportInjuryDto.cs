using FitnessApp.Domain.Enums;

namespace FitnessApp.Application.DTOs;

/// <summary>
/// Data transfer object for reporting a new injury
/// </summary>
public class ReportInjuryDto
{
    /// <summary>
    /// Body part or region affected (e.g., "Shoulder", "Knee", "Lower Back")
    /// </summary>
    public required string BodyPart { get; set; }

    /// <summary>
    /// Type of injury (Acute or Chronic)
    /// </summary>
    public InjuryType InjuryType { get; set; } = InjuryType.Acute;

    /// <summary>
    /// Severity level (e.g., "Mild", "Moderate", "Severe")
    /// </summary>
    public string? Severity { get; set; }

    /// <summary>
    /// Pain description (e.g., "Sharp", "Dull", "Aching")
    /// </summary>
    public string? PainDescription { get; set; }

    /// <summary>
    /// Movement restrictions (e.g., "Overhead", "Impact", "Heavy Load", "Rotation")
    /// </summary>
    public string? MovementRestrictions { get; set; }
}
