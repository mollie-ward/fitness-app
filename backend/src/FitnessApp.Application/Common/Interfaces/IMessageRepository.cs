using FitnessApp.Domain.Entities;

namespace FitnessApp.Application.Common.Interfaces;

/// <summary>
/// Repository interface for message persistence
/// </summary>
public interface IMessageRepository
{
    /// <summary>
    /// Gets messages for a conversation
    /// </summary>
    /// <param name="conversationId">The conversation identifier</param>
    /// <param name="limit">Maximum number of messages to retrieve</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of messages ordered by timestamp</returns>
    Task<IEnumerable<Message>> GetMessagesByConversationIdAsync(
        Guid conversationId, 
        int limit = 50,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new message to the repository
    /// </summary>
    /// <param name="message">The message to add</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created message</returns>
    Task<Message> AddAsync(Message message, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds multiple messages in a batch
    /// </summary>
    /// <param name="messages">The messages to add</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task AddRangeAsync(IEnumerable<Message> messages, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes all messages in a conversation
    /// </summary>
    /// <param name="conversationId">The conversation identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task DeleteByConversationIdAsync(Guid conversationId, CancellationToken cancellationToken = default);
}
