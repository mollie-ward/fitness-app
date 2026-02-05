using FitnessApp.Domain.Common;

namespace FitnessApp.Domain.Entities;

/// <summary>
/// Represents a refresh token for JWT token rotation
/// </summary>
public class RefreshToken : BaseEntity
{
    /// <summary>
    /// The user ID this refresh token belongs to
    /// </summary>
    public required Guid UserId { get; set; }

    /// <summary>
    /// The actual refresh token value
    /// </summary>
    public required string Token { get; set; }

    /// <summary>
    /// Date and time when the token expires
    /// </summary>
    public required DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Indicates whether the token has been revoked
    /// </summary>
    public bool IsRevoked { get; set; }

    /// <summary>
    /// Date and time when the token was revoked
    /// </summary>
    public DateTime? RevokedAt { get; set; }

    /// <summary>
    /// Reason for revocation (e.g., "User logout", "Token rotation")
    /// </summary>
    public string? RevokeReason { get; set; }

    /// <summary>
    /// Navigation property to the user
    /// </summary>
    public User? User { get; set; }

    /// <summary>
    /// Checks if the token is currently valid
    /// </summary>
    public bool IsValid => !IsRevoked && ExpiresAt > DateTime.UtcNow;
}
