using FitnessApp.Domain.Entities;
using FitnessApp.Domain.Enums;

namespace FitnessApp.Application.Common.Interfaces;

/// <summary>
/// Repository interface for workout operations
/// </summary>
public interface IWorkoutRepository
{
    /// <summary>
    /// Gets a workout by its identifier
    /// </summary>
    /// <param name="workoutId">The workout identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The workout or null if not found</returns>
    Task<Workout?> GetWorkoutByIdAsync(Guid workoutId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets workouts by date range for a user
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <param name="startDate">The start date</param>
    /// <param name="endDate">The end date</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of workouts in the date range</returns>
    Task<IEnumerable<Workout>> GetWorkoutsByDateRangeAsync(Guid userId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the completion status of a workout
    /// </summary>
    /// <param name="workoutId">The workout identifier</param>
    /// <param name="status">The new completion status</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated workout</returns>
    Task<Workout> UpdateWorkoutStatusAsync(Guid workoutId, CompletionStatus status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets today's workout for a user
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Today's workout or null if none exists</returns>
    Task<Workout?> GetTodaysWorkoutAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets upcoming workouts for a user
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <param name="days">Number of days to look ahead</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of upcoming workouts</returns>
    Task<IEnumerable<Workout>> GetUpcomingWorkoutsAsync(Guid userId, int days, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a workout with all its exercises loaded
    /// </summary>
    /// <param name="workoutId">The workout identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The workout with exercises or null if not found</returns>
    Task<Workout?> GetWorkoutWithExercisesAsync(Guid workoutId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new workout
    /// </summary>
    /// <param name="workout">The workout to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created workout</returns>
    Task<Workout> CreateWorkoutAsync(Workout workout, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing workout
    /// </summary>
    /// <param name="workout">The workout to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated workout</returns>
    Task<Workout> UpdateWorkoutAsync(Workout workout, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing workout (alias method for consistency)
    /// </summary>
    /// <param name="workout">The workout to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing the async operation</returns>
    Task UpdateAsync(Workout workout, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets workouts by date range for a training plan
    /// </summary>
    /// <param name="planId">The plan identifier</param>
    /// <param name="startDate">The start date</param>
    /// <param name="endDate">The end date</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of workouts in the date range</returns>
    Task<IEnumerable<Workout>> GetWorkoutsByPlanAndDateRangeAsync(Guid planId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
}
