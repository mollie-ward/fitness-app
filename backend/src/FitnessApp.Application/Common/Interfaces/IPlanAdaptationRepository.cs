using FitnessApp.Domain.Entities;

namespace FitnessApp.Application.Common.Interfaces;

/// <summary>
/// Repository interface for plan adaptation entities
/// </summary>
public interface IPlanAdaptationRepository
{
    /// <summary>
    /// Adds a new plan adaptation record
    /// </summary>
    /// <param name="adaptation">The adaptation to add</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The added adaptation</returns>
    Task<PlanAdaptation> AddAsync(PlanAdaptation adaptation, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an adaptation by its identifier
    /// </summary>
    /// <param name="id">The adaptation identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The adaptation if found, null otherwise</returns>
    Task<PlanAdaptation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all adaptations for a specific plan
    /// </summary>
    /// <param name="planId">The plan identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of adaptations</returns>
    Task<IEnumerable<PlanAdaptation>> GetByPlanIdAsync(Guid planId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the most recent adaptation for a plan
    /// </summary>
    /// <param name="planId">The plan identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The most recent adaptation if any, null otherwise</returns>
    Task<PlanAdaptation?> GetMostRecentByPlanIdAsync(Guid planId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing adaptation
    /// </summary>
    /// <param name="adaptation">The adaptation to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task</returns>
    Task UpdateAsync(PlanAdaptation adaptation, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an adaptation (for reverting)
    /// </summary>
    /// <param name="adaptation">The adaptation to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task</returns>
    Task DeleteAsync(PlanAdaptation adaptation, CancellationToken cancellationToken = default);
}
