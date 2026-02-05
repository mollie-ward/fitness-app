using Microsoft.Extensions.Logging;
using FitnessApp.Application.Common.Interfaces;

namespace FitnessApp.Infrastructure.Services;

/// <summary>
/// Email service for sending authentication-related emails
/// TODO: Implement actual email sending using SendGrid, AWS SES, or similar
/// </summary>
public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }

    public Task SendEmailVerificationAsync(string email, string verificationToken, CancellationToken cancellationToken = default)
    {
        // TODO: Implement actual email sending
        // For now, just log the verification link
        _logger.LogInformation(
            "Email verification link for {Email}: /auth/verify-email?token={Token}",
            email,
            verificationToken);

        // In a real implementation, you would send an email with a link like:
        // https://yourapp.com/verify-email?token={verificationToken}

        return Task.CompletedTask;
    }

    public Task SendPasswordResetEmailAsync(string email, string resetToken, CancellationToken cancellationToken = default)
    {
        // TODO: Implement actual email sending
        // For now, just log the reset link
        _logger.LogInformation(
            "Password reset link for {Email}: /auth/reset-password?token={Token}",
            email,
            resetToken);

        // In a real implementation, you would send an email with a link like:
        // https://yourapp.com/reset-password?token={resetToken}

        return Task.CompletedTask;
    }
}
