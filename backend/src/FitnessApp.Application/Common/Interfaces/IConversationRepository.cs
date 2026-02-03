using FitnessApp.Domain.Entities;

namespace FitnessApp.Application.Common.Interfaces;

/// <summary>
/// Repository interface for conversation persistence
/// </summary>
public interface IConversationRepository
{
    /// <summary>
    /// Gets a conversation by ID
    /// </summary>
    /// <param name="conversationId">The conversation identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The conversation if found, null otherwise</returns>
    Task<Conversation?> GetByIdAsync(Guid conversationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a conversation with its messages
    /// </summary>
    /// <param name="conversationId">The conversation identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The conversation with messages if found, null otherwise</returns>
    Task<Conversation?> GetByIdWithMessagesAsync(Guid conversationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the active conversation for a user (or null if none exists)
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The active conversation if found, null otherwise</returns>
    Task<Conversation?> GetActiveConversationByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new conversation
    /// </summary>
    /// <param name="conversation">The conversation to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created conversation</returns>
    Task<Conversation> AddAsync(Conversation conversation, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing conversation
    /// </summary>
    /// <param name="conversation">The conversation to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task UpdateAsync(Conversation conversation, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a conversation
    /// </summary>
    /// <param name="conversationId">The conversation identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task DeleteAsync(Guid conversationId, CancellationToken cancellationToken = default);
}
