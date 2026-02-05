using FitnessApp.Domain.Common;

namespace FitnessApp.Domain.Entities;

/// <summary>
/// Represents a conversation thread between a user and Coach Tom
/// </summary>
public class Conversation : BaseEntity
{
    /// <summary>
    /// Gets or sets the user identifier who owns this conversation
    /// </summary>
    public required Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the collection of messages in this conversation
    /// </summary>
    public ICollection<Message> Messages { get; set; } = new List<Message>();

    /// <summary>
    /// Gets or sets metadata snapshot about the conversation context
    /// This can include summary information about what was discussed
    /// </summary>
    public string? ConversationContext { get; set; }

    /// <summary>
    /// Gets or sets whether this conversation is active or archived
    /// </summary>
    public bool IsActive { get; set; } = true;
}
