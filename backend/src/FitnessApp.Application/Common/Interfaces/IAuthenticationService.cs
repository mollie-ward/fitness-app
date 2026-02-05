using FitnessApp.Application.DTOs;

namespace FitnessApp.Application.Common.Interfaces;

/// <summary>
/// Interface for authentication service operations
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// Registers a new user account
    /// </summary>
    /// <param name="request">Registration details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success message</returns>
    Task<string> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Authenticates a user and returns tokens
    /// </summary>
    /// <param name="request">Login credentials</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Authentication tokens and user information</returns>
    Task<TokenResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Refreshes an access token using a refresh token
    /// </summary>
    /// <param name="request">Refresh token</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>New authentication tokens</returns>
    Task<TokenResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Logs out a user by revoking their refresh token
    /// </summary>
    /// <param name="refreshToken">Refresh token to revoke</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task LogoutAsync(string refreshToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Initiates the password reset process
    /// </summary>
    /// <param name="request">Forgot password request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success message</returns>
    Task<string> ForgotPasswordAsync(ForgotPasswordRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Resets a user's password using a reset token
    /// </summary>
    /// <param name="request">Reset password request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success message</returns>
    Task<string> ResetPasswordAsync(ResetPasswordRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Changes a user's password
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="request">Change password request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success message</returns>
    Task<string> ChangePasswordAsync(Guid userId, ChangePasswordRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifies a user's email address
    /// </summary>
    /// <param name="request">Email verification request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success message</returns>
    Task<string> VerifyEmailAsync(VerifyEmailRequestDto request, CancellationToken cancellationToken = default);
}
