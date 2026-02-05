using FitnessApp.Application.DTOs;

namespace FitnessApp.Application.Common.Interfaces;

/// <summary>
/// Service interface for managing user injuries and limitations
/// Handles injury reporting, contraindication logic, and exercise substitutions
/// </summary>
public interface IInjuryManagementService
{
    /// <summary>
    /// Reports a new injury for a user
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <param name="injuryDetails">Details of the injury being reported</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created injury with ID</returns>
    Task<InjuryLimitationDto> ReportInjuryAsync(
        Guid userId, 
        ReportInjuryDto injuryDetails, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the status of an existing injury
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <param name="injuryId">The injury identifier</param>
    /// <param name="statusUpdate">New status details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated injury</returns>
    Task<InjuryLimitationDto> UpdateInjuryStatusAsync(
        Guid userId, 
        Guid injuryId, 
        UpdateInjuryStatusDto statusUpdate, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks an injury as resolved
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <param name="injuryId">The injury identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated injury</returns>
    Task<InjuryLimitationDto> MarkInjuryResolvedAsync(
        Guid userId, 
        Guid injuryId, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all active injuries for a user
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of active injuries</returns>
    Task<IEnumerable<InjuryLimitationDto>> GetActiveInjuriesAsync(
        Guid userId, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all exercises contraindicated for a user based on their active injuries
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of contraindicated exercises with reasons</returns>
    Task<IEnumerable<ContraindicatedExerciseDto>> GetContraindicatedExercisesAsync(
        Guid userId, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a substitute exercise for a contraindicated exercise
    /// </summary>
    /// <param name="exerciseId">The original exercise identifier</param>
    /// <param name="userId">The user identifier (for injury constraints)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Substitute exercise details, or null if no suitable substitute found</returns>
    Task<SubstituteExerciseDto?> GetSubstituteExerciseAsync(
        Guid exerciseId, 
        Guid userId, 
        CancellationToken cancellationToken = default);
}
