namespace FitnessApp.Application.DTOs;

/// <summary>
/// Request DTO for sending a chat message to Coach Tom
/// </summary>
public class ChatMessageRequestDto
{
    /// <summary>
    /// The user's message text
    /// </summary>
    public required string Message { get; set; }

    /// <summary>
    /// Optional conversation ID to continue an existing conversation
    /// </summary>
    public Guid? ConversationId { get; set; }
}
