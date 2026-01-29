using FitnessApp.Domain.Enums;

namespace FitnessApp.Application.DTOs;

/// <summary>
/// Data transfer object for injury limitations
/// </summary>
public class InjuryLimitationDto
{
    /// <summary>
    /// Gets or sets the injury identifier
    /// </summary>
    public Guid? Id { get; set; }

    /// <summary>
    /// Gets or sets the body part affected by the injury
    /// </summary>
    public required string BodyPart { get; set; }

    /// <summary>
    /// Gets or sets the type of injury
    /// </summary>
    public InjuryType InjuryType { get; set; }

    /// <summary>
    /// Gets or sets the date when the injury was reported
    /// </summary>
    public DateTime ReportedDate { get; set; }

    /// <summary>
    /// Gets or sets the current status of the injury
    /// </summary>
    public InjuryStatus Status { get; set; } = InjuryStatus.Active;

    /// <summary>
    /// Gets or sets any movement restrictions caused by this injury
    /// </summary>
    public string? MovementRestrictions { get; set; }
}
