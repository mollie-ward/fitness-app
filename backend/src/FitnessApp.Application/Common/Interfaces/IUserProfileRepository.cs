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

    /// <summary>
    /// Gets a complete user profile with all related entities
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The complete user profile if found, null otherwise</returns>
    Task<UserProfile?> GetCompleteProfileAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a specific training goal by ID
    /// </summary>
    /// <param name="goalId">The goal ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The training goal if found, null otherwise</returns>
    Task<TrainingGoal?> GetGoalByIdAsync(Guid goalId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a specific injury limitation by ID
    /// </summary>
    /// <param name="injuryId">The injury ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The injury limitation if found, null otherwise</returns>
    Task<InjuryLimitation?> GetInjuryByIdAsync(Guid injuryId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a training goal to a user profile
    /// </summary>
    /// <param name="goal">The training goal to add</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The added training goal</returns>
    Task<TrainingGoal> AddGoalAsync(TrainingGoal goal, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a training goal
    /// </summary>
    /// <param name="goal">The training goal to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated training goal</returns>
    Task<TrainingGoal> UpdateGoalAsync(TrainingGoal goal, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a training goal
    /// </summary>
    /// <param name="goalId">The goal ID to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if deleted, false if not found</returns>
    Task<bool> DeleteGoalAsync(Guid goalId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds an injury limitation to a user profile
    /// </summary>
    /// <param name="injury">The injury limitation to add</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The added injury limitation</returns>
    Task<InjuryLimitation> AddInjuryAsync(InjuryLimitation injury, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an injury limitation
    /// </summary>
    /// <param name="injury">The injury limitation to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated injury limitation</returns>
    Task<InjuryLimitation> UpdateInjuryAsync(InjuryLimitation injury, CancellationToken cancellationToken = default);
}
