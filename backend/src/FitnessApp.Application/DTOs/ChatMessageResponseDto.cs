namespace FitnessApp.Application.DTOs;

/// <summary>
/// Response DTO for a chat message from Coach Tom
/// </summary>
public class ChatMessageResponseDto
{
    /// <summary>
    /// The assistant's response text
    /// </summary>
    public required string Response { get; set; }

    /// <summary>
    /// The conversation ID for this exchange
    /// </summary>
    public required Guid ConversationId { get; set; }

    /// <summary>
    /// Timestamp when the response was generated
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Any action that was triggered (e.g., plan modification)
    /// </summary>
    public string? TriggeredAction { get; set; }
}
