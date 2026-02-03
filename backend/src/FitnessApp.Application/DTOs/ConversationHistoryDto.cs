namespace FitnessApp.Application.DTOs;

/// <summary>
/// DTO representing a conversation history
/// </summary>
public class ConversationHistoryDto
{
    /// <summary>
    /// The conversation identifier
    /// </summary>
    public Guid ConversationId { get; set; }

    /// <summary>
    /// The user identifier who owns this conversation
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// List of messages in the conversation
    /// </summary>
    public IEnumerable<MessageDto> Messages { get; set; } = new List<MessageDto>();

    /// <summary>
    /// Total number of messages in the conversation
    /// </summary>
    public int TotalMessages { get; set; }

    /// <summary>
    /// When the conversation was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// When the conversation was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
