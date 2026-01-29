using FitnessApp.Domain.Entities;

namespace FitnessApp.Application.Common.Interfaces;

/// <summary>
/// Repository interface for UserProfile entity operations
/// </summary>
public interface IUserProfileRepository
{
    /// <summary>
    /// Gets a user profile by user ID
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The user profile if found, null otherwise</returns>
    Task<UserProfile?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a user profile by profile ID
    /// </summary>
    /// <param name="profileId">The profile ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The user profile if found, null otherwise</returns>
    Task<UserProfile?> GetByIdAsync(Guid profileId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a user profile with all training goals
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The user profile with goals if found, null otherwise</returns>
    Task<UserProfile?> GetProfileWithGoalsAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a user profile with all injury limitations
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The user profile with injuries if found, null otherwise</returns>
    Task<UserProfile?> GetProfileWithInjuriesAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new user profile
    /// </summary>
    /// <param name="profile">The user profile to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created user profile</returns>
    Task<UserProfile> CreateAsync(UserProfile profile, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing user profile
    /// </summary>
    /// <param name="profile">The user profile to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated user profile</returns>
    Task<UserProfile> UpdateAsync(UserProfile profile, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a user profile by user ID
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if deleted, false if not found</returns>
    Task<bool> DeleteAsync(Guid userId, CancellationToken cancellationToken = default);
}
