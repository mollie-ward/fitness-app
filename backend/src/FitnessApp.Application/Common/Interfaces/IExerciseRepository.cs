using FitnessApp.Domain.Entities;
using FitnessApp.Domain.Enums;

namespace FitnessApp.Application.Common.Interfaces;

/// <summary>
/// Repository interface for exercise operations
/// </summary>
public interface IExerciseRepository
{
    /// <summary>
    /// Gets an exercise by its identifier
    /// </summary>
    /// <param name="exerciseId">The exercise identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The exercise or null if not found</returns>
    Task<Exercise?> GetByIdAsync(Guid exerciseId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets exercises matching the specified criteria
    /// </summary>
    /// <param name="discipline">The primary discipline</param>
    /// <param name="difficultyLevel">The difficulty level (optional)</param>
    /// <param name="sessionType">The session type (optional)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of matching exercises</returns>
    Task<IEnumerable<Exercise>> GetExercisesByCriteriaAsync(
        Discipline? discipline = null,
        DifficultyLevel? difficultyLevel = null,
        SessionType? sessionType = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets exercises excluding those contraindicated for specific injury types
    /// </summary>
    /// <param name="injuryTypes">Injury types to exclude exercises for</param>
    /// <param name="discipline">The primary discipline (optional)</param>
    /// <param name="difficultyLevel">The difficulty level (optional)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of safe exercises</returns>
    Task<IEnumerable<Exercise>> GetSafeExercisesAsync(
        IEnumerable<InjuryType> injuryTypes,
        Discipline? discipline = null,
        DifficultyLevel? difficultyLevel = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all exercises
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of all exercises</returns>
    Task<IEnumerable<Exercise>> GetAllAsync(CancellationToken cancellationToken = default);
}
