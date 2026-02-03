using FitnessApp.Domain.Enums;

namespace FitnessApp.Application.DTOs;

/// <summary>
/// DTO representing a single message in a conversation
/// </summary>
public class MessageDto
{
    /// <summary>
    /// The message identifier
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The role of the message sender
    /// </summary>
    public MessageRole Role { get; set; }

    /// <summary>
    /// The content of the message
    /// </summary>
    public required string Content { get; set; }

    /// <summary>
    /// Timestamp when the message was sent
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// The recognized intent (if applicable)
    /// </summary>
    public MessageIntent? Intent { get; set; }

    /// <summary>
    /// Any action that was triggered by this message
    /// </summary>
    public string? TriggeredAction { get; set; }
}
