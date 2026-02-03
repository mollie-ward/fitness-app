using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FitnessApp.Application.Common.Interfaces;
using FitnessApp.Application.DTOs;

namespace FitnessApp.API.Controllers;

/// <summary>
/// Controller for progress tracking operations
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/progress")]
[ApiVersion("1.0")]
[Authorize]
public class ProgressController : BaseApiController
{
    private readonly IProgressTrackingService _progressTrackingService;
    private readonly ILogger<ProgressController> _logger;

    public ProgressController(
        IProgressTrackingService progressTrackingService,
        ILogger<ProgressController> logger)
    {
        _progressTrackingService = progressTrackingService;
        _logger = logger;
    }

    /// <summary>
    /// Mark a workout as complete
    /// </summary>
    /// <param name="workoutId">The workout identifier</param>
    /// <param name="completionDto">Completion details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated workout details</returns>
    /// <response code="200">Workout marked as complete successfully</response>
    /// <response code="400">Invalid request or validation error</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="403">User does not own this workout</response>
    /// <response code="404">Workout not found</response>
    [HttpPut("workouts/{workoutId}/complete")]
    [ProducesResponseType(typeof(WorkoutDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WorkoutDetailDto>> CompleteWorkout(
        Guid workoutId,
        [FromBody] WorkoutCompletionDto completionDto,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _progressTrackingService.MarkWorkoutCompleteAsync(
                workoutId, 
                userId, 
                completionDto, 
                cancellationToken);

            _logger.LogInformation("Workout {WorkoutId} marked complete by user {UserId}", workoutId, userId);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Workout {WorkoutId} not found", workoutId);
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access attempt for workout {WorkoutId}", workoutId);
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation for workout {WorkoutId}", workoutId);
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Undo workout completion
    /// </summary>
    /// <param name="workoutId">The workout identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated workout details</returns>
    /// <response code="200">Workout completion undone successfully</response>
    /// <response code="400">Invalid request or validation error</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="403">User does not own this workout</response>
    /// <response code="404">Workout not found</response>
    [HttpPut("workouts/{workoutId}/undo")]
    [ProducesResponseType(typeof(WorkoutDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WorkoutDetailDto>> UndoCompletion(
        Guid workoutId,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _progressTrackingService.UndoWorkoutCompletionAsync(
                workoutId, 
                userId, 
                cancellationToken);

            _logger.LogInformation("Workout {WorkoutId} completion undone by user {UserId}", workoutId, userId);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Workout {WorkoutId} not found", workoutId);
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access attempt for workout {WorkoutId}", workoutId);
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation for workout {WorkoutId}", workoutId);
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get weekly completion statistics
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Weekly statistics</returns>
    /// <response code="200">Statistics retrieved successfully</response>
    /// <response code="401">User is not authenticated</response>
    [HttpGet("stats/weekly")]
    [ProducesResponseType(typeof(CompletionStatsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<CompletionStatsDto>> GetWeeklyStats(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var now = DateTime.UtcNow;
        var weekStart = now.Date.AddDays(-(int)now.DayOfWeek);
        var weekEnd = weekStart.AddDays(7).AddSeconds(-1);

        var stats = await _progressTrackingService.GetCompletionStatsAsync(
            userId, 
            weekStart, 
            weekEnd, 
            cancellationToken);

        return Ok(stats);
    }

    /// <summary>
    /// Get monthly completion statistics
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Monthly statistics</returns>
    /// <response code="200">Statistics retrieved successfully</response>
    /// <response code="401">User is not authenticated</response>
    [HttpGet("stats/monthly")]
    [ProducesResponseType(typeof(CompletionStatsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<CompletionStatsDto>> GetMonthlyStats(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var now = DateTime.UtcNow;
        var monthStart = new DateTime(now.Year, now.Month, 1);
        var monthEnd = monthStart.AddMonths(1).AddSeconds(-1);

        var stats = await _progressTrackingService.GetCompletionStatsAsync(
            userId, 
            monthStart, 
            monthEnd, 
            cancellationToken);

        return Ok(stats);
    }

    /// <summary>
    /// Get overall all-time statistics
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Overall statistics</returns>
    /// <response code="200">Statistics retrieved successfully</response>
    /// <response code="401">User is not authenticated</response>
    [HttpGet("stats/overall")]
    [ProducesResponseType(typeof(OverallStatsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<OverallStatsDto>> GetOverallStats(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var stats = await _progressTrackingService.GetOverallStatsAsync(userId, cancellationToken);
        return Ok(stats);
    }

    /// <summary>
    /// Get current streak information
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Streak information</returns>
    /// <response code="200">Streak information retrieved successfully</response>
    /// <response code="401">User is not authenticated</response>
    [HttpGet("streak")]
    [ProducesResponseType(typeof(StreakInfoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<StreakInfoDto>> GetStreak(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var streak = await _progressTrackingService.GetStreakInfoAsync(userId, cancellationToken);
        return Ok(streak);
    }

    /// <summary>
    /// Get historical completion data
    /// </summary>
    /// <param name="startDate">Start date of the range (optional, defaults to 90 days ago)</param>
    /// <param name="endDate">End date of the range (optional, defaults to now)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Historical completion data</returns>
    /// <response code="200">History retrieved successfully</response>
    /// <response code="400">Invalid date range</response>
    /// <response code="401">User is not authenticated</response>
    [HttpGet("history")]
    [ProducesResponseType(typeof(IEnumerable<CompletionHistoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<CompletionHistoryDto>>> GetHistory(
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        
        var effectiveStartDate = startDate ?? DateTime.UtcNow.AddDays(-90);
        var effectiveEndDate = endDate ?? DateTime.UtcNow;

        if (effectiveStartDate > effectiveEndDate)
        {
            return BadRequest(new { message = "Start date must be before end date" });
        }

        if ((effectiveEndDate - effectiveStartDate).TotalDays > 365)
        {
            return BadRequest(new { message = "Date range cannot exceed 365 days" });
        }

        var history = await _progressTrackingService.GetCompletionHistoryAsync(
            userId, 
            effectiveStartDate, 
            effectiveEndDate, 
            cancellationToken);

        return Ok(history);
    }
}
