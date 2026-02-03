using FitnessApp.Domain.Entities;

namespace FitnessApp.Application.Common.Interfaces;

/// <summary>
/// Repository interface for completion history operations
/// </summary>
public interface ICompletionHistoryRepository
{
    /// <summary>
    /// Adds a completion history record
    /// </summary>
    /// <param name="completionHistory">The completion history to add</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The added completion history</returns>
    Task<CompletionHistory> AddAsync(CompletionHistory completionHistory, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets completion history for a specific workout
    /// </summary>
    /// <param name="workoutId">The workout identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The completion history or null if not found</returns>
    Task<CompletionHistory?> GetByWorkoutIdAsync(Guid workoutId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets completion history for a user within a date range
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <param name="startDate">Start date of the range</param>
    /// <param name="endDate">End date of the range</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of completion history records</returns>
    Task<IEnumerable<CompletionHistory>> GetByUserAndDateRangeAsync(
        Guid userId, 
        DateTime startDate, 
        DateTime endDate, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all completion history for a user
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of all completion history records</returns>
    Task<IEnumerable<CompletionHistory>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a completion history record
    /// </summary>
    /// <param name="completionHistory">The completion history to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task DeleteAsync(CompletionHistory completionHistory, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets distinct completion dates for a user (for calculating training days)
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of distinct dates when user completed workouts</returns>
    Task<IEnumerable<DateTime>> GetDistinctCompletionDatesAsync(Guid userId, CancellationToken cancellationToken = default);
}
