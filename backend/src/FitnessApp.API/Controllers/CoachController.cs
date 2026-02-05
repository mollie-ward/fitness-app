using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FitnessApp.Application.Common.Interfaces;
using FitnessApp.Application.DTOs;

namespace FitnessApp.API.Controllers;

/// <summary>
/// Controller for AI Coach (Coach Tom) operations
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/coach")]
[ApiVersion("1.0")]
[Authorize]
public class CoachController : BaseApiController
{
    private readonly IAICoachService _aiCoachService;
    private readonly ILogger<CoachController> _logger;

    public CoachController(
        IAICoachService aiCoachService,
        ILogger<CoachController> logger)
    {
        _aiCoachService = aiCoachService;
        _logger = logger;
    }

    /// <summary>
    /// Send a message to Coach Tom and get a response
    /// </summary>
    /// <param name="request">The chat message request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Coach Tom's response</returns>
    /// <response code="200">Message processed successfully</response>
    /// <response code="400">Invalid request</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="429">Rate limit exceeded</response>
    [HttpPost("chat")]
    [ProducesResponseType(typeof(ChatMessageResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<ChatMessageResponseDto>> SendMessage(
        [FromBody] ChatMessageRequestDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Message))
            {
                return BadRequest(new { message = "Message cannot be empty" });
            }

            var userId = GetCurrentUserId();
            var response = await _aiCoachService.SendMessageAsync(
                userId,
                request.Message,
                request.ConversationId,
                cancellationToken);

            _logger.LogInformation(
                "Chat message processed for user {UserId}, conversation {ConversationId}",
                userId, response.ConversationId);

            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access attempt in chat");
            return Forbid();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Conversation not found");
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing chat message");
            return StatusCode(500, new { message = "An error occurred processing your message" });
        }
    }

    /// <summary>
    /// Get conversation history
    /// </summary>
    /// <param name="conversationId">The conversation identifier</param>
    /// <param name="limit">Maximum number of messages to retrieve (default: 50)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Conversation history</returns>
    /// <response code="200">History retrieved successfully</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="403">User does not own this conversation</response>
    /// <response code="404">Conversation not found</response>
    [HttpGet("conversations/{conversationId}/history")]
    [ProducesResponseType(typeof(ConversationHistoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ConversationHistoryDto>> GetConversationHistory(
        Guid conversationId,
        [FromQuery] int limit = 50,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = GetCurrentUserId();
            var history = await _aiCoachService.GetConversationHistoryAsync(
                conversationId,
                userId,
                limit,
                cancellationToken);

            return Ok(history);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access to conversation {ConversationId}", conversationId);
            return Forbid();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Conversation {ConversationId} not found", conversationId);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving conversation history for {ConversationId}", conversationId);
            return StatusCode(500, new { message = "An error occurred retrieving conversation history" });
        }
    }

    /// <summary>
    /// Clear a conversation history
    /// </summary>
    /// <param name="conversationId">The conversation identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content on success</returns>
    /// <response code="204">Conversation cleared successfully</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="403">User does not own this conversation</response>
    /// <response code="404">Conversation not found</response>
    [HttpDelete("conversations/{conversationId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ClearConversation(
        Guid conversationId,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            await _aiCoachService.ClearConversationAsync(conversationId, userId, cancellationToken);

            _logger.LogInformation("Conversation {ConversationId} cleared for user {UserId}", conversationId, userId);
            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized attempt to clear conversation {ConversationId}", conversationId);
            return Forbid();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Conversation {ConversationId} not found for deletion", conversationId);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing conversation {ConversationId}", conversationId);
            return StatusCode(500, new { message = "An error occurred clearing the conversation" });
        }
    }

    /// <summary>
    /// Get Coach Tom's avatar URL
    /// </summary>
    /// <returns>Avatar information</returns>
    /// <response code="200">Avatar URL retrieved successfully</response>
    [HttpGet("avatar")]
    [AllowAnonymous]  // Allow anonymous access to avatar
    [ProducesResponseType(typeof(CoachAvatarDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<CoachAvatarDto>> GetAvatar()
    {
        var avatarUrl = await _aiCoachService.GetCoachAvatarUrlAsync();

        return Ok(new CoachAvatarDto
        {
            AvatarUrl = avatarUrl,
            Name = "Coach Tom",
            Description = "Your HYROX and endurance training coach"
        });
    }
}
