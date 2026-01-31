using FitnessApp.Domain.Entities;
using FitnessApp.Domain.Enums;

namespace FitnessApp.Application.Common.Interfaces;

/// <summary>
/// Repository interface for training plan operations
/// </summary>
public interface ITrainingPlanRepository
{
    /// <summary>
    /// Gets the active training plan for a user
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The active training plan or null if none exists</returns>
    Task<TrainingPlan?> GetActivePlanByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a training plan by its identifier
    /// </summary>
    /// <param name="planId">The plan identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The training plan or null if not found</returns>
    Task<TrainingPlan?> GetByIdAsync(Guid planId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new training plan
    /// </summary>
    /// <param name="plan">The training plan to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created training plan</returns>
    Task<TrainingPlan> CreatePlanAsync(TrainingPlan plan, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing training plan
    /// </summary>
    /// <param name="plan">The training plan to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated training plan</returns>
    Task<TrainingPlan> UpdatePlanAsync(TrainingPlan plan, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a training plan with all its weeks loaded
    /// </summary>
    /// <param name="planId">The plan identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The training plan with weeks or null if not found</returns>
    Task<TrainingPlan?> GetPlanWithWeeksAsync(Guid planId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the current week's workouts for a user
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of workouts for the current week</returns>
    Task<IEnumerable<Workout>> GetCurrentWeekWorkoutsAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a training plan (soft delete)
    /// </summary>
    /// <param name="planId">The plan identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if deleted successfully, false if not found</returns>
    Task<bool> DeletePlanAsync(Guid planId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all training plans for a user
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <param name="includeDeleted">Whether to include soft-deleted plans</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of training plans</returns>
    Task<IEnumerable<TrainingPlan>> GetPlansByUserIdAsync(Guid userId, bool includeDeleted = false, CancellationToken cancellationToken = default);
}
