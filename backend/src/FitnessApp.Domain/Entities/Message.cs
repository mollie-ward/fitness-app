using FitnessApp.Domain.Common;
using FitnessApp.Domain.Enums;

namespace FitnessApp.Domain.Entities;

/// <summary>
/// Represents a single message in a conversation
/// </summary>
public class Message : BaseEntity
{
    /// <summary>
    /// Gets or sets the conversation identifier this message belongs to
    /// </summary>
    public required Guid ConversationId { get; set; }

    /// <summary>
    /// Gets or sets the navigation property to the parent conversation
    /// </summary>
    public Conversation? Conversation { get; set; }

    /// <summary>
    /// Gets or sets the role of the message sender
    /// </summary>
    public required MessageRole Role { get; set; }

    /// <summary>
    /// Gets or sets the content/text of the message
    /// </summary>
    public required string Content { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the message was sent
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the recognized intent of the message (nullable for non-user messages)
    /// </summary>
    public MessageIntent? Intent { get; set; }

    /// <summary>
    /// Gets or sets the action that was triggered by this message (if any)
    /// E.g., "plan_modified", "injury_updated", "schedule_changed"
    /// </summary>
    public string? TriggeredAction { get; set; }

    /// <summary>
    /// Gets or sets the number of tokens used for this message (for cost tracking)
    /// </summary>
    public int? TokenCount { get; set; }
}
