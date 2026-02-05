namespace FitnessApp.Application.Common.Interfaces;

/// <summary>
/// Interface for email sending operations
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Sends an email verification message
    /// </summary>
    /// <param name="email">Recipient email address</param>
    /// <param name="verificationToken">Verification token</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task SendEmailVerificationAsync(string email, string verificationToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a password reset email
    /// </summary>
    /// <param name="email">Recipient email address</param>
    /// <param name="resetToken">Password reset token</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task SendPasswordResetEmailAsync(string email, string resetToken, CancellationToken cancellationToken = default);
}
