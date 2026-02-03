using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FitnessApp.Application.Common.Interfaces;
using FitnessApp.Application.DTOs;
using FitnessApp.Application.Mapping;
using FitnessApp.Domain.Enums;

namespace FitnessApp.API.Controllers;

/// <summary>
/// Controller for training plan operations
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/training/plans")]
[ApiVersion("1.0")]
[Authorize]
public class TrainingPlanController : BaseApiController
{
    private readonly ITrainingPlanRepository _planRepository;
    private readonly ITrainingPlanGenerationService _generationService;
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly ILogger<TrainingPlanController> _logger;

    public TrainingPlanController(
        ITrainingPlanRepository planRepository,
        ITrainingPlanGenerationService generationService,
        IUserProfileRepository userProfileRepository,
        ILogger<TrainingPlanController> logger)
    {
        _planRepository = planRepository;
        _generationService = generationService;
        _userProfileRepository = userProfileRepository;
        _logger = logger;
    }

    /// <summary>
    /// Generate a new training plan for the authenticated user
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The generated training plan summary</returns>
    /// <response code="201">Training plan created successfully</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="404">User profile not found</response>
    /// <response code="409">User already has an active training plan</response>
    [HttpPost("generate")]
    [ProducesResponseType(typeof(TrainingPlanSummaryDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<TrainingPlanSummaryDto>> GeneratePlan(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();

        // Check if user already has an active plan
        var existingPlan = await _planRepository.GetActivePlanByUserIdAsync(userId, cancellationToken);
        if (existingPlan != null)
        {
            return Conflict(new { message = "User already has an active training plan" });
        }

        // Verify user profile exists
        var userProfile = await _userProfileRepository.GetByUserIdAsync(userId, cancellationToken);
        if (userProfile == null)
        {
            return NotFound(new { message = "User profile not found. Please complete your profile first." });
        }

        // Validate profile has sufficient information
        var isValid = await _generationService.ValidatePlanParametersAsync(userProfile, cancellationToken);
        if (!isValid)
        {
            return BadRequest(new { message = "User profile does not have sufficient information for plan generation" });
        }

        try
        {
            // Generate the plan
            var plan = await _generationService.GeneratePlanAsync(userId, cancellationToken);

            _logger.LogInformation("Generated training plan {PlanId} for user {UserId}", plan.Id, userId);

            return CreatedAtAction(
                nameof(GetPlanById),
                new { version = "1", planId = plan.Id },
                plan.ToSummaryDto());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate training plan for user {UserId}", userId);
            return StatusCode(500, new { message = "An error occurred while generating the training plan" });
        }
    }

    /// <summary>
    /// Get the currently active training plan for the authenticated user
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The active training plan summary</returns>
    /// <response code="200">Active plan retrieved successfully</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="404">No active plan found</response>
    [HttpGet("current")]
    [ProducesResponseType(typeof(TrainingPlanSummaryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TrainingPlanSummaryDto>> GetCurrentPlan(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();

        var plan = await _planRepository.GetActivePlanByUserIdAsync(userId, cancellationToken);

        if (plan == null)
        {
            return NotFound(new { message = "No active training plan found" });
        }

        // Load the plan with weeks to calculate progress
        var detailedPlan = await _planRepository.GetPlanWithWeeksAsync(plan.Id, cancellationToken);
        if (detailedPlan == null)
        {
            return NotFound(new { message = "No active training plan found" });
        }

        return Ok(detailedPlan.ToSummaryDto());
    }

    /// <summary>
    /// Get a specific training plan by ID
    /// </summary>
    /// <param name="planId">The plan identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The detailed training plan with all weeks and workouts</returns>
    /// <response code="200">Plan retrieved successfully</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="403">User does not own this plan</response>
    /// <response code="404">Plan not found</response>
    [HttpGet("{planId}")]
    [ProducesResponseType(typeof(TrainingPlanDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TrainingPlanDetailDto>> GetPlanById(Guid planId, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();

        var plan = await _planRepository.GetPlanWithWeeksAsync(planId, cancellationToken);

        if (plan == null)
        {
            return NotFound(new { message = "Training plan not found" });
        }

        // Verify user owns this plan
        if (plan.UserId != userId)
        {
            _logger.LogWarning("User {UserId} attempted to access plan {PlanId} owned by {OwnerId}", userId, planId, plan.UserId);
            return Forbid();
        }

        return Ok(plan.ToDetailDto());
    }

    /// <summary>
    /// Get a specific week from a training plan
    /// </summary>
    /// <param name="planId">The plan identifier</param>
    /// <param name="weekNumber">The week number</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The training week with all workouts</returns>
    /// <response code="200">Week retrieved successfully</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="403">User does not own this plan</response>
    /// <response code="404">Plan or week not found</response>
    [HttpGet("{planId}/weeks/{weekNumber}")]
    [ProducesResponseType(typeof(TrainingWeekDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TrainingWeekDto>> GetPlanWeek(Guid planId, int weekNumber, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();

        var plan = await _planRepository.GetPlanWithWeeksAsync(planId, cancellationToken);

        if (plan == null)
        {
            return NotFound(new { message = "Training plan not found" });
        }

        // Verify user owns this plan
        if (plan.UserId != userId)
        {
            _logger.LogWarning("User {UserId} attempted to access plan {PlanId} owned by {OwnerId}", userId, planId, plan.UserId);
            return Forbid();
        }

        var week = plan.TrainingWeeks?.FirstOrDefault(w => w.WeekNumber == weekNumber);

        if (week == null)
        {
            return NotFound(new { message = $"Week {weekNumber} not found in this plan" });
        }

        return Ok(week.ToDto());
    }

    /// <summary>
    /// Archive or delete a training plan
    /// </summary>
    /// <param name="planId">The plan identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content</returns>
    /// <response code="204">Plan deleted successfully</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="403">User does not own this plan</response>
    /// <response code="404">Plan not found</response>
    [HttpDelete("{planId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePlan(Guid planId, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();

        var plan = await _planRepository.GetByIdAsync(planId, cancellationToken);

        if (plan == null)
        {
            return NotFound(new { message = "Training plan not found" });
        }

        // Verify user owns this plan
        if (plan.UserId != userId)
        {
            _logger.LogWarning("User {UserId} attempted to delete plan {PlanId} owned by {OwnerId}", userId, planId, plan.UserId);
            return Forbid();
        }

        await _planRepository.DeletePlanAsync(planId, cancellationToken);

        _logger.LogInformation("Deleted plan {PlanId} for user {UserId}", planId, userId);

        return NoContent();
    }
}
