using FitnessApp.Domain.Common;

namespace FitnessApp.Domain.Entities;

/// <summary>
/// Represents an email verification token
/// </summary>
public class EmailVerificationToken : BaseEntity
{
    /// <summary>
    /// The user ID this token belongs to
    /// </summary>
    public required Guid UserId { get; set; }

    /// <summary>
    /// The verification token value
    /// </summary>
    public required string Token { get; set; }

    /// <summary>
    /// Date and time when the token expires
    /// </summary>
    public required DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Indicates whether the token has been used
    /// </summary>
    public bool IsUsed { get; set; }

    /// <summary>
    /// Date and time when the token was used
    /// </summary>
    public DateTime? UsedAt { get; set; }

    /// <summary>
    /// Navigation property to the user
    /// </summary>
    public User? User { get; set; }

    /// <summary>
    /// Checks if the token is currently valid
    /// </summary>
    public bool IsValid => !IsUsed && ExpiresAt > DateTime.UtcNow;
}
