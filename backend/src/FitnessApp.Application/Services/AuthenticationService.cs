using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using FitnessApp.Application.Common.Interfaces;
using FitnessApp.Application.DTOs;
using FitnessApp.Domain.Entities;

namespace FitnessApp.Application.Services;

/// <summary>
/// Service for handling authentication operations
/// </summary>
public class AuthenticationService : IAuthenticationService
{
    private readonly IApplicationDbContext _context;
    private readonly ITokenService _tokenService;
    private readonly IEmailService _emailService;
    private readonly ILogger<AuthenticationService> _logger;

    public AuthenticationService(
        IApplicationDbContext context,
        ITokenService tokenService,
        IEmailService emailService,
        ILogger<AuthenticationService> logger)
    {
        _context = context;
        _tokenService = tokenService;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<string> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken = default)
    {
        // Check if user already exists
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower(), cancellationToken);

        if (existingUser != null)
        {
            throw new InvalidOperationException("A user with this email already exists");
        }

        // Hash the password
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password, workFactor: 12);

        // Create the user
        var user = new User
        {
            Email = request.Email,
            Name = request.Name,
            PasswordHash = passwordHash,
            EmailVerified = false,
            IsActive = true
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        // Generate email verification token
        var verificationToken = GenerateSecureToken();
        var emailVerificationToken = new EmailVerificationToken
        {
            UserId = user.Id,
            Token = verificationToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        _context.EmailVerificationTokens.Add(emailVerificationToken);
        await _context.SaveChangesAsync(cancellationToken);

        // Send verification email
        await _emailService.SendEmailVerificationAsync(user.Email, verificationToken, cancellationToken);

        _logger.LogInformation("User registered successfully: {Email}", user.Email);

        return "Registration successful. Please check your email to verify your account.";
    }

    public async Task<TokenResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
    {
        // Find user by email
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower(), cancellationToken);

        if (user == null || user.PasswordHash == null)
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        // Verify password
        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        // Check if account is active
        if (!user.IsActive)
        {
            throw new UnauthorizedAccessException("This account has been suspended");
        }

        // Update last login time
        user.LastLoginAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        // Generate tokens
        var accessToken = _tokenService.GenerateAccessToken(user.Id, user.Email);
        var refreshTokenValue = _tokenService.GenerateRefreshToken();

        // Store refresh token
        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = refreshTokenValue,
            ExpiresAt = DateTime.UtcNow.AddDays(30)
        };

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User logged in successfully: {Email}", user.Email);

        return new TokenResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshTokenValue,
            ExpiresIn = 1800, // 30 minutes in seconds
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                EmailVerified = user.EmailVerified
            }
        };
    }

    public async Task<TokenResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request, CancellationToken cancellationToken = default)
    {
        // Find the refresh token
        var refreshToken = await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken, cancellationToken);

        if (refreshToken == null || !refreshToken.IsValid)
        {
            throw new UnauthorizedAccessException("Invalid or expired refresh token");
        }

        var user = refreshToken.User!;

        // Check if account is active
        if (!user.IsActive)
        {
            throw new UnauthorizedAccessException("This account has been suspended");
        }

        // Revoke the old refresh token
        refreshToken.IsRevoked = true;
        refreshToken.RevokedAt = DateTime.UtcNow;
        refreshToken.RevokeReason = "Token rotation";

        // Generate new tokens
        var newAccessToken = _tokenService.GenerateAccessToken(user.Id, user.Email);
        var newRefreshTokenValue = _tokenService.GenerateRefreshToken();

        // Store new refresh token
        var newRefreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = newRefreshTokenValue,
            ExpiresAt = DateTime.UtcNow.AddDays(30)
        };

        _context.RefreshTokens.Add(newRefreshToken);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Tokens refreshed for user: {Email}", user.Email);

        return new TokenResponseDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshTokenValue,
            ExpiresIn = 1800, // 30 minutes in seconds
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                EmailVerified = user.EmailVerified
            }
        };
    }

    public async Task LogoutAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        // Find and revoke the refresh token
        var token = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken, cancellationToken);

        if (token != null)
        {
            token.IsRevoked = true;
            token.RevokedAt = DateTime.UtcNow;
            token.RevokeReason = "User logout";
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("User logged out: UserId={UserId}", token.UserId);
        }
    }

    public async Task<string> ForgotPasswordAsync(ForgotPasswordRequestDto request, CancellationToken cancellationToken = default)
    {
        // Find user by email
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower(), cancellationToken);

        // Always return success to prevent email enumeration
        if (user == null)
        {
            _logger.LogWarning("Password reset requested for non-existent email: {Email}", request.Email);
            return "If an account with that email exists, a password reset link has been sent.";
        }

        // Generate reset token
        var resetToken = GenerateSecureToken();
        var passwordResetToken = new PasswordResetToken
        {
            UserId = user.Id,
            Token = resetToken,
            ExpiresAt = DateTime.UtcNow.AddHours(1)
        };

        _context.PasswordResetTokens.Add(passwordResetToken);
        await _context.SaveChangesAsync(cancellationToken);

        // Send reset email
        await _emailService.SendPasswordResetEmailAsync(user.Email, resetToken, cancellationToken);

        _logger.LogInformation("Password reset email sent to: {Email}", user.Email);

        return "If an account with that email exists, a password reset link has been sent.";
    }

    public async Task<string> ResetPasswordAsync(ResetPasswordRequestDto request, CancellationToken cancellationToken = default)
    {
        // Find the reset token
        var resetToken = await _context.PasswordResetTokens
            .Include(prt => prt.User)
            .FirstOrDefaultAsync(prt => prt.Token == request.Token, cancellationToken);

        if (resetToken == null || !resetToken.IsValid)
        {
            throw new InvalidOperationException("Invalid or expired reset token");
        }

        var user = resetToken.User!;

        // Hash the new password
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword, workFactor: 12);

        // Mark token as used
        resetToken.IsUsed = true;
        resetToken.UsedAt = DateTime.UtcNow;

        // Revoke all existing refresh tokens for security
        var refreshTokens = await _context.RefreshTokens
            .Where(rt => rt.UserId == user.Id && !rt.IsRevoked)
            .ToListAsync(cancellationToken);

        foreach (var token in refreshTokens)
        {
            token.IsRevoked = true;
            token.RevokedAt = DateTime.UtcNow;
            token.RevokeReason = "Password reset";
        }

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Password reset successful for user: {Email}", user.Email);

        return "Password has been reset successfully. Please log in with your new password.";
    }

    public async Task<string> ChangePasswordAsync(Guid userId, ChangePasswordRequestDto request, CancellationToken cancellationToken = default)
    {
        // Find the user
        var user = await _context.Users.FindAsync(new object[] { userId }, cancellationToken);

        if (user == null || user.PasswordHash == null)
        {
            throw new InvalidOperationException("User not found");
        }

        // Verify current password
        if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Current password is incorrect");
        }

        // Hash the new password
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword, workFactor: 12);

        // Revoke all existing refresh tokens for security
        var refreshTokens = await _context.RefreshTokens
            .Where(rt => rt.UserId == userId && !rt.IsRevoked)
            .ToListAsync(cancellationToken);

        foreach (var token in refreshTokens)
        {
            token.IsRevoked = true;
            token.RevokedAt = DateTime.UtcNow;
            token.RevokeReason = "Password change";
        }

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Password changed successfully for user: {Email}", user.Email);

        return "Password has been changed successfully. Please log in again.";
    }

    public async Task<string> VerifyEmailAsync(VerifyEmailRequestDto request, CancellationToken cancellationToken = default)
    {
        // Find the verification token
        var verificationToken = await _context.EmailVerificationTokens
            .Include(evt => evt.User)
            .FirstOrDefaultAsync(evt => evt.Token == request.Token, cancellationToken);

        if (verificationToken == null || !verificationToken.IsValid)
        {
            throw new InvalidOperationException("Invalid or expired verification token");
        }

        var user = verificationToken.User!;

        // Mark email as verified
        user.EmailVerified = true;

        // Mark token as used
        verificationToken.IsUsed = true;
        verificationToken.UsedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Email verified successfully for user: {Email}", user.Email);

        return "Email verified successfully. You now have full access to your account.";
    }

    private static string GenerateSecureToken()
    {
        var randomBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }
}
