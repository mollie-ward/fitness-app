namespace FitnessApp.Domain.Enums;

/// <summary>
/// Represents the role of a participant in a conversation
/// </summary>
public enum MessageRole
{
    /// <summary>
    /// Message from the user
    /// </summary>
    User = 0,

    /// <summary>
    /// Message from the AI assistant (Coach Tom)
    /// </summary>
    Assistant = 1,

    /// <summary>
    /// System message (e.g., context, instructions)
    /// </summary>
    System = 2
}
