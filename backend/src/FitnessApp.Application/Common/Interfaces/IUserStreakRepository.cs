using FitnessApp.Domain.Entities;

namespace FitnessApp.Application.Common.Interfaces;

/// <summary>
/// Repository interface for user streak operations
/// </summary>
public interface IUserStreakRepository
{
    /// <summary>
    /// Gets the streak information for a user
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The user streak or null if not found</returns>
    Task<UserStreak?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new user streak record
    /// </summary>
    /// <param name="userStreak">The user streak to add</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The added user streak</returns>
    Task<UserStreak> AddAsync(UserStreak userStreak, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing user streak record
    /// </summary>
    /// <param name="userStreak">The user streak to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task UpdateAsync(UserStreak userStreak, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets or creates a user streak record
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The existing or new user streak</returns>
    Task<UserStreak> GetOrCreateAsync(Guid userId, CancellationToken cancellationToken = default);
}
