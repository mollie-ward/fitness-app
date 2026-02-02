using FitnessApp.Domain.Entities;

namespace FitnessApp.Application.Common.Interfaces;

/// <summary>
/// Service interface for generating personalized training plans
/// </summary>
public interface ITrainingPlanGenerationService
{
    /// <summary>
    /// Generates a new training plan for a user based on their profile
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The generated training plan</returns>
    Task<TrainingPlan> GeneratePlanAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Regenerates a training plan with modifications
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <param name="modifications">Plan modification parameters (reserved for future use)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The regenerated training plan</returns>
    Task<TrainingPlan> RegeneratePlanAsync(Guid userId, object? modifications = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates that a user profile has sufficient information for plan generation
    /// </summary>
    /// <param name="userProfile">The user profile to validate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if the profile is valid for plan generation, false otherwise</returns>
    Task<bool> ValidatePlanParametersAsync(UserProfile userProfile, CancellationToken cancellationToken = default);
}
