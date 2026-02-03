using FitnessApp.Application.DTOs;

namespace FitnessApp.Application.Common.Interfaces;

/// <summary>
/// Service interface for AI Coach operations
/// </summary>
public interface IAICoachService
{
    /// <summary>
    /// Sends a message to Coach Tom and gets a response
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <param name="message">The user's message</param>
    /// <param name="conversationId">Optional conversation ID to continue existing conversation</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The assistant's response with conversation ID</returns>
    Task<ChatMessageResponseDto> SendMessageAsync(
        Guid userId, 
        string message, 
        Guid? conversationId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the conversation history for a user
    /// </summary>
    /// <param name="conversationId">The conversation identifier</param>
    /// <param name="userId">The user identifier (for authorization)</param>
    /// <param name="limit">Maximum number of messages to retrieve</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of messages in the conversation</returns>
    Task<ConversationHistoryDto> GetConversationHistoryAsync(
        Guid conversationId,
        Guid userId,
        int limit = 50,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Clears a conversation history
    /// </summary>
    /// <param name="conversationId">The conversation identifier</param>
    /// <param name="userId">The user identifier (for authorization)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task ClearConversationAsync(
        Guid conversationId,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the avatar URL for Coach Tom
    /// </summary>
    /// <returns>Avatar URL</returns>
    Task<string> GetCoachAvatarUrlAsync();
}
