using FitnessApp.Application.DTOs;

namespace FitnessApp.Application.Common.Interfaces;

/// <summary>
/// Service interface for progress tracking operations
/// </summary>
public interface IProgressTrackingService
{
    /// <summary>
    /// Marks a workout as completed
    /// </summary>
    /// <param name="workoutId">The workout identifier</param>
    /// <param name="userId">The user identifier</param>
    /// <param name="completionDto">Completion details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated workout detail</returns>
    Task<WorkoutDetailDto> MarkWorkoutCompleteAsync(
        Guid workoutId, 
        Guid userId, 
        WorkoutCompletionDto completionDto, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Undoes a workout completion
    /// </summary>
    /// <param name="workoutId">The workout identifier</param>
    /// <param name="userId">The user identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated workout detail</returns>
    Task<WorkoutDetailDto> UndoWorkoutCompletionAsync(
        Guid workoutId, 
        Guid userId, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets completion statistics for a specific date range
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <param name="startDate">Start date of the range</param>
    /// <param name="endDate">End date of the range</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Completion statistics</returns>
    Task<CompletionStatsDto> GetCompletionStatsAsync(
        Guid userId, 
        DateTime startDate, 
        DateTime endDate, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets overall all-time statistics
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Overall statistics</returns>
    Task<OverallStatsDto> GetOverallStatsAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets streak information for a user
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Streak information</returns>
    Task<StreakInfoDto> GetStreakInfoAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets historical completion data
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <param name="startDate">Start date of the range</param>
    /// <param name="endDate">End date of the range</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of completion history records</returns>
    Task<IEnumerable<CompletionHistoryDto>> GetCompletionHistoryAsync(
        Guid userId, 
        DateTime startDate, 
        DateTime endDate, 
        CancellationToken cancellationToken = default);
}
