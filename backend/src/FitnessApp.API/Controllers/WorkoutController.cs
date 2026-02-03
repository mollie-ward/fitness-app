using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FitnessApp.Application.Common.Interfaces;
using FitnessApp.Application.DTOs;
using FitnessApp.Application.Mapping;
using FitnessApp.Domain.Enums;

namespace FitnessApp.API.Controllers;

/// <summary>
/// Controller for workout operations
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/training/workouts")]
[ApiVersion("1.0")]
[Authorize]
public class WorkoutController : ControllerBase
{
    private readonly IWorkoutRepository _workoutRepository;
    private readonly ITrainingPlanRepository _planRepository;
    private readonly ILogger<WorkoutController> _logger;

    public WorkoutController(
        IWorkoutRepository workoutRepository,
        ITrainingPlanRepository planRepository,
        ILogger<WorkoutController> logger)
    {
        _workoutRepository = workoutRepository;
        _planRepository = planRepository;
        _logger = logger;
    }

    /// <summary>
    /// Get today's scheduled workout for the authenticated user
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Today's workout with exercises</returns>
    /// <response code="200">Workout retrieved successfully</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="404">No workout scheduled for today</response>
    [HttpGet("today")]
    [ProducesResponseType(typeof(WorkoutDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WorkoutDetailDto>> GetTodaysWorkout(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();

        var workout = await _workoutRepository.GetTodaysWorkoutAsync(userId, cancellationToken);

        if (workout == null)
        {
            return NotFound(new { message = "No workout scheduled for today" });
        }

        // Load workout with exercises
        var detailedWorkout = await _workoutRepository.GetWorkoutWithExercisesAsync(workout.Id, cancellationToken);
        if (detailedWorkout == null)
        {
            return NotFound(new { message = "No workout scheduled for today" });
        }

        return Ok(detailedWorkout.ToDetailDto());
    }

    /// <summary>
    /// Get upcoming workouts for the next 7 days
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of upcoming workouts</returns>
    /// <response code="200">Workouts retrieved successfully</response>
    /// <response code="401">User is not authenticated</response>
    [HttpGet("upcoming")]
    [ProducesResponseType(typeof(IEnumerable<WorkoutSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<WorkoutSummaryDto>>> GetUpcomingWorkouts(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();

        var workouts = await _workoutRepository.GetUpcomingWorkoutsAsync(userId, 7, cancellationToken);

        var workoutDtos = workouts.Select(w => w.ToSummaryDto()).ToList();

        return Ok(workoutDtos);
    }

    /// <summary>
    /// Get detailed workout information by ID
    /// </summary>
    /// <param name="workoutId">The workout identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Detailed workout with all exercises</returns>
    /// <response code="200">Workout retrieved successfully</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="403">User does not own this workout</response>
    /// <response code="404">Workout not found</response>
    [HttpGet("{workoutId}")]
    [ProducesResponseType(typeof(WorkoutDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WorkoutDetailDto>> GetWorkoutById(Guid workoutId, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();

        var workout = await _workoutRepository.GetWorkoutWithExercisesAsync(workoutId, cancellationToken);

        if (workout == null)
        {
            return NotFound(new { message = "Workout not found" });
        }

        // Verify user owns this workout
        if (!await UserOwnsWorkoutAsync(userId, workoutId, cancellationToken))
        {
            _logger.LogWarning("User {UserId} attempted to access workout {WorkoutId} they don't own", userId, workoutId);
            return Forbid();
        }

        return Ok(workout.ToDetailDto());
    }

    /// <summary>
    /// Mark a workout as completed
    /// </summary>
    /// <param name="workoutId">The workout identifier</param>
    /// <param name="request">Completion request with optional timestamp</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated workout</returns>
    /// <response code="200">Workout marked as completed</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="403">User does not own this workout</response>
    /// <response code="404">Workout not found</response>
    [HttpPut("{workoutId}/complete")]
    [ProducesResponseType(typeof(WorkoutDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WorkoutDetailDto>> CompleteWorkout(
        Guid workoutId,
        [FromBody] CompleteWorkoutRequest? request,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();

        // Verify workout exists
        var workout = await _workoutRepository.GetWorkoutByIdAsync(workoutId, cancellationToken);
        if (workout == null)
        {
            return NotFound(new { message = "Workout not found" });
        }

        // Verify user owns this workout
        if (!await UserOwnsWorkoutAsync(userId, workoutId, cancellationToken))
        {
            _logger.LogWarning("User {UserId} attempted to complete workout {WorkoutId} they don't own", userId, workoutId);
            return Forbid();
        }

        try
        {
            // Update the workout status
            var updatedWorkout = await _workoutRepository.UpdateWorkoutStatusAsync(workoutId, CompletionStatus.Completed, cancellationToken);

            // If a specific completion time was provided, update it
            if (request?.CompletedAt.HasValue == true)
            {
                updatedWorkout.CompletedAt = request.CompletedAt.Value;
                await _workoutRepository.UpdateWorkoutAsync(updatedWorkout, cancellationToken);
            }

            _logger.LogInformation("User {UserId} completed workout {WorkoutId}", userId, workoutId);

            // Load with exercises for response
            var detailedWorkout = await _workoutRepository.GetWorkoutWithExercisesAsync(workoutId, cancellationToken);

            return Ok(detailedWorkout!.ToDetailDto());
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Failed to complete workout {WorkoutId}", workoutId);
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Mark a workout as skipped
    /// </summary>
    /// <param name="workoutId">The workout identifier</param>
    /// <param name="request">Skip request with optional reason</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated workout</returns>
    /// <response code="200">Workout marked as skipped</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="403">User does not own this workout</response>
    /// <response code="404">Workout not found</response>
    [HttpPut("{workoutId}/skip")]
    [ProducesResponseType(typeof(WorkoutDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WorkoutDetailDto>> SkipWorkout(
        Guid workoutId,
        [FromBody] SkipWorkoutRequest? request,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();

        // Verify workout exists
        var workout = await _workoutRepository.GetWorkoutByIdAsync(workoutId, cancellationToken);
        if (workout == null)
        {
            return NotFound(new { message = "Workout not found" });
        }

        // Verify user owns this workout
        if (!await UserOwnsWorkoutAsync(userId, workoutId, cancellationToken))
        {
            _logger.LogWarning("User {UserId} attempted to skip workout {WorkoutId} they don't own", userId, workoutId);
            return Forbid();
        }

        try
        {
            // Update the workout status
            var updatedWorkout = await _workoutRepository.UpdateWorkoutStatusAsync(workoutId, CompletionStatus.Skipped, cancellationToken);

            _logger.LogInformation("User {UserId} skipped workout {WorkoutId}. Reason: {Reason}", userId, workoutId, request?.Reason ?? "Not provided");

            // Load with exercises for response
            var detailedWorkout = await _workoutRepository.GetWorkoutWithExercisesAsync(workoutId, cancellationToken);

            return Ok(detailedWorkout!.ToDetailDto());
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Failed to skip workout {WorkoutId}", workoutId);
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get workouts for a date range (calendar view)
    /// </summary>
    /// <param name="startDate">Start date (ISO 8601 format)</param>
    /// <param name="endDate">End date (ISO 8601 format)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of workouts in the date range</returns>
    /// <response code="200">Workouts retrieved successfully</response>
    /// <response code="400">Invalid date range</response>
    /// <response code="401">User is not authenticated</response>
    [HttpGet("calendar")]
    [ProducesResponseType(typeof(IEnumerable<WorkoutSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<WorkoutSummaryDto>>> GetWorkoutsForCalendar(
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();

        // Validate date range
        if (!startDate.HasValue || !endDate.HasValue)
        {
            return BadRequest(new { message = "Both startDate and endDate are required" });
        }

        if (startDate.Value > endDate.Value)
        {
            return BadRequest(new { message = "startDate must be before or equal to endDate" });
        }

        // Limit range to prevent excessive queries
        var daysDifference = (endDate.Value - startDate.Value).Days;
        if (daysDifference > 90)
        {
            return BadRequest(new { message = "Date range cannot exceed 90 days" });
        }

        var workouts = await _workoutRepository.GetWorkoutsByDateRangeAsync(userId, startDate.Value, endDate.Value, cancellationToken);

        var workoutDtos = workouts.Select(w => w.ToSummaryDto()).ToList();

        return Ok(workoutDtos);
    }

    /// <summary>
    /// Verifies that the user owns the workout by checking plan ownership
    /// </summary>
    private async Task<bool> UserOwnsWorkoutAsync(Guid userId, Guid workoutId, CancellationToken cancellationToken)
    {
        var workout = await _workoutRepository.GetWorkoutByIdAsync(workoutId, cancellationToken);
        if (workout?.TrainingWeek == null)
        {
            // Load the week if not already loaded
            var workoutWithWeek = await _workoutRepository.GetWorkoutWithExercisesAsync(workoutId, cancellationToken);
            if (workoutWithWeek?.TrainingWeek == null)
            {
                return false;
            }
            workout = workoutWithWeek;
        }

        var plan = await _planRepository.GetByIdAsync(workout.TrainingWeek.PlanId, cancellationToken);
        return plan?.UserId == userId;
    }

    /// <summary>
    /// Gets the current user ID from JWT claims
    /// </summary>
    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                         ?? User.FindFirst("sub")?.Value
                         ?? User.FindFirst("userId")?.Value;
        
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("User ID not found in token");
        }

        return userId;
    }
}
