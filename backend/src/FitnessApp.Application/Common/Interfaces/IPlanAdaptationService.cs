using FitnessApp.Application.DTOs;

namespace FitnessApp.Application.Common.Interfaces;

/// <summary>
/// Service interface for adaptive plan adjustments
/// Handles dynamic plan modifications based on various triggers
/// </summary>
public interface IPlanAdaptationService
{
    /// <summary>
    /// Adapts the plan when user misses consecutive workouts
    /// Reduces intensity for re-entry and optionally extends timeline
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <param name="missedWorkoutIds">Collection of missed workout identifiers</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Adaptation result with details of changes</returns>
    Task<PlanAdaptationResultDto> AdaptForMissedWorkoutsAsync(
        Guid userId, 
        IEnumerable<Guid> missedWorkoutIds, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adjusts plan intensity based on user request (harder or easier)
    /// Modifies future workouts while preserving past completions
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <param name="direction">Direction of intensity change (Harder or Easier)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Adaptation result with details of changes</returns>
    Task<PlanAdaptationResultDto> AdaptForIntensityChangeAsync(
        Guid userId, 
        IntensityAdjustmentDto adjustment, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Redistributes workouts when user changes available training days
    /// Prioritizes key workouts and maintains training stimulus
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <param name="newSchedule">New schedule availability</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Adaptation result with details of changes</returns>
    Task<PlanAdaptationResultDto> AdaptForScheduleChangeAsync(
        Guid userId, 
        ScheduleAvailabilityDto newSchedule, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Modifies plan to accommodate injury restrictions
    /// Removes contraindicated exercises and substitutes alternatives
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <param name="injuryId">The injury identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Adaptation result with details of changes</returns>
    Task<PlanAdaptationResultDto> AdaptForInjuryAsync(
        Guid userId, 
        Guid injuryId, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Recalculates plan periodization when goal timeline changes
    /// Adjusts progression rate and phase durations
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <param name="timelineChange">Timeline change details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Adaptation result with details of changes</returns>
    Task<PlanAdaptationResultDto> AdaptForGoalTimelineChangeAsync(
        Guid userId, 
        GoalTimelineChangeDto timelineChange, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adapts plan based on perceived difficulty feedback pattern
    /// Adjusts intensity when user consistently reports "too easy" or "too hard"
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <param name="feedbackPattern">Pattern of perceived difficulty feedback</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Adaptation result with details of changes</returns>
    Task<PlanAdaptationResultDto> AdaptForPerceivedDifficultyAsync(
        Guid userId, 
        string feedbackPattern, 
        CancellationToken cancellationToken = default);
}
