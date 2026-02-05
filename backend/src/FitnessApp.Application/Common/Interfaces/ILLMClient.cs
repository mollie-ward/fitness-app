namespace FitnessApp.Application.Common.Interfaces;

/// <summary>
/// Interface for interacting with Large Language Model services
/// </summary>
public interface ILLMClient
{
    /// <summary>
    /// Sends a chat completion request to the LLM
    /// </summary>
    /// <param name="systemPrompt">The system instructions/personality definition</param>
    /// <param name="userMessage">The user's message</param>
    /// <param name="conversationHistory">Previous messages for context</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The LLM's response text and token count</returns>
    Task<LLMResponse> GetChatCompletionAsync(
        string systemPrompt,
        string userMessage,
        IEnumerable<LLMMessage>? conversationHistory = null,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Represents a message in the LLM conversation history
/// </summary>
public class LLMMessage
{
    /// <summary>
    /// The role of the message sender (user or assistant)
    /// </summary>
    public required string Role { get; set; }

    /// <summary>
    /// The content of the message
    /// </summary>
    public required string Content { get; set; }
}

/// <summary>
/// Represents the response from an LLM
/// </summary>
public class LLMResponse
{
    /// <summary>
    /// The generated response text
    /// </summary>
    public required string Content { get; set; }

    /// <summary>
    /// The total number of tokens used (prompt + completion)
    /// </summary>
    public int TotalTokens { get; set; }

    /// <summary>
    /// The number of tokens in the prompt
    /// </summary>
    public int PromptTokens { get; set; }

    /// <summary>
    /// The number of tokens in the completion
    /// </summary>
    public int CompletionTokens { get; set; }
}
