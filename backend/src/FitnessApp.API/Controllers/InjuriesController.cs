using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FitnessApp.Application.Common.Interfaces;
using FitnessApp.Application.DTOs;

namespace FitnessApp.API.Controllers;

/// <summary>
/// Controller for injury management and exercise substitution operations
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/injuries")]
[ApiVersion("1.0")]
[Authorize]
public class InjuriesController : ControllerBase
{
    private readonly IInjuryManagementService _injuryManagementService;
    private readonly ILogger<InjuriesController> _logger;

    // Medical disclaimer
    private const string MedicalDisclaimer = "⚠️ Medical Disclaimer: This app is not a substitute for professional medical advice. " +
        "Please consult a healthcare professional for proper diagnosis and treatment of injuries.";

    public InjuriesController(
        IInjuryManagementService injuryManagementService,
        ILogger<InjuriesController> logger)
    {
        _injuryManagementService = injuryManagementService;
        _logger = logger;
    }

    /// <summary>
    /// Report a new injury
    /// </summary>
    /// <param name="injuryDto">Injury details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created injury with ID and medical disclaimer</returns>
    /// <response code="201">Injury reported successfully</response>
    /// <response code="400">Validation errors in request</response>
    /// <response code="401">User is not authenticated</response>
    [HttpPost]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ReportInjury(
        [FromBody] ReportInjuryDto injuryDto,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();

        _logger.LogInformation("User {UserId} reporting injury to {BodyPart}", userId, injuryDto.BodyPart);

        try
        {
            var injury = await _injuryManagementService.ReportInjuryAsync(userId, injuryDto, cancellationToken);

            return StatusCode(StatusCodes.Status201Created, new
            {
                injury,
                disclaimer = MedicalDisclaimer,
                recommendations = GetInjuryRecommendations(injuryDto)
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to report injury for user {UserId}", userId);
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Update the status of an existing injury
    /// </summary>
    /// <param name="injuryId">Injury identifier</param>
    /// <param name="statusUpdate">New status details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated injury</returns>
    /// <response code="200">Injury status updated successfully</response>
    /// <response code="400">Validation errors in request</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="403">User does not have access to this injury</response>
    /// <response code="404">Injury not found</response>
    [HttpPut("{injuryId}/status")]
    [ProducesResponseType(typeof(InjuryLimitationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateInjuryStatus(
        Guid injuryId,
        [FromBody] UpdateInjuryStatusDto statusUpdate,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();

        _logger.LogInformation("User {UserId} updating injury {InjuryId} status to {Status}", 
            userId, injuryId, statusUpdate.Status);

        try
        {
            var injury = await _injuryManagementService.UpdateInjuryStatusAsync(userId, injuryId, statusUpdate, cancellationToken);
            return Ok(injury);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access to injury {InjuryId} by user {UserId}", injuryId, userId);
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to update injury {InjuryId} for user {UserId}", injuryId, userId);
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Mark an injury as resolved
    /// </summary>
    /// <param name="injuryId">Injury identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated injury</returns>
    /// <response code="200">Injury marked as resolved</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="403">User does not have access to this injury</response>
    /// <response code="404">Injury not found</response>
    [HttpPut("{injuryId}/resolve")]
    [ProducesResponseType(typeof(InjuryLimitationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkInjuryResolved(
        Guid injuryId,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();

        _logger.LogInformation("User {UserId} marking injury {InjuryId} as resolved", userId, injuryId);

        try
        {
            var injury = await _injuryManagementService.MarkInjuryResolvedAsync(userId, injuryId, cancellationToken);
            return Ok(injury);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access to injury {InjuryId} by user {UserId}", injuryId, userId);
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to resolve injury {InjuryId} for user {UserId}", injuryId, userId);
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get all active injuries for the authenticated user
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of active injuries</returns>
    /// <response code="200">Active injuries retrieved successfully</response>
    /// <response code="401">User is not authenticated</response>
    [HttpGet("active")]
    [ProducesResponseType(typeof(IEnumerable<InjuryLimitationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetActiveInjuries(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();

        _logger.LogInformation("User {UserId} retrieving active injuries", userId);

        var injuries = await _injuryManagementService.GetActiveInjuriesAsync(userId, cancellationToken);
        return Ok(injuries);
    }

    /// <summary>
    /// Get all contraindicated exercises for the authenticated user based on their active injuries
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of contraindicated exercises with reasons</returns>
    /// <response code="200">Contraindicated exercises retrieved successfully</response>
    /// <response code="401">User is not authenticated</response>
    [HttpGet("contraindications")]
    [ProducesResponseType(typeof(IEnumerable<ContraindicatedExerciseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetContraindicatedExercises(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();

        _logger.LogInformation("User {UserId} retrieving contraindicated exercises", userId);

        var contraindications = await _injuryManagementService.GetContraindicatedExercisesAsync(userId, cancellationToken);
        
        return Ok(new
        {
            contraindications,
            disclaimer = MedicalDisclaimer,
            message = contraindications.Any() 
                ? "The following exercises should be avoided based on your active injuries. Please consult with Coach Tom for alternatives."
                : "No exercise contraindications found. You're cleared for all exercises!"
        });
    }

    /// <summary>
    /// Get a substitute exercise for a contraindicated exercise
    /// </summary>
    /// <param name="exerciseId">Original exercise identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Substitute exercise details or null if no suitable substitute found</returns>
    /// <response code="200">Substitute exercise retrieved successfully (may be null)</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="404">Original exercise not found</response>
    [HttpGet("substitutes/{exerciseId}")]
    [ProducesResponseType(typeof(SubstituteExerciseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSubstituteExercise(
        Guid exerciseId,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();

        _logger.LogInformation("User {UserId} requesting substitute for exercise {ExerciseId}", userId, exerciseId);

        try
        {
            var substitute = await _injuryManagementService.GetSubstituteExerciseAsync(exerciseId, userId, cancellationToken);
            
            if (substitute == null)
            {
                return Ok(new
                {
                    substitute = (SubstituteExerciseDto?)null,
                    message = "No suitable substitute found. Please consult with Coach Tom for alternative exercises.",
                    disclaimer = MedicalDisclaimer
                });
            }

            return Ok(new
            {
                substitute,
                disclaimer = MedicalDisclaimer
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Exercise {ExerciseId} not found", exerciseId);
            return NotFound(new { message = ex.Message });
        }
    }

    #region Helper Methods

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                         ?? User.FindFirst("sub")?.Value
                         ?? User.FindFirst("userId")?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("User ID not found in claims");
        }

        return userId;
    }

    private List<string> GetInjuryRecommendations(ReportInjuryDto injury)
    {
        var recommendations = new List<string>
        {
            "Monitor your pain levels during and after workouts",
            "Stop immediately if pain worsens during exercise",
            "Consider consulting a healthcare professional if pain persists for more than 2 weeks"
        };

        // Add severity-specific recommendations
        if (injury.Severity?.Contains("Severe", StringComparison.OrdinalIgnoreCase) ?? false)
        {
            recommendations.Insert(0, "⚠️ Severe injury reported - strongly recommend medical evaluation");
        }

        // Add body-part-specific recommendations
        var bodyPartLower = injury.BodyPart.ToLower();
        if (bodyPartLower.Contains("shoulder"))
        {
            recommendations.Add("Avoid overhead movements until cleared");
        }
        else if (bodyPartLower.Contains("knee"))
        {
            recommendations.Add("Reduce impact activities and consider low-impact alternatives");
        }
        else if (bodyPartLower.Contains("back"))
        {
            recommendations.Add("Focus on core stability and avoid heavy loading");
        }

        return recommendations;
    }

    #endregion
}
