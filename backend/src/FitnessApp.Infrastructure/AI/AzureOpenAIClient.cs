using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using FitnessApp.Application.Common.Interfaces;
using OpenAI.Chat;

namespace FitnessApp.Infrastructure.AI;

/// <summary>
/// Azure OpenAI implementation of the LLM client
/// </summary>
public class AzureOpenAILLMClient : ILLMClient
{
    private readonly AzureOpenAISettings _settings;
    private readonly ILogger<AzureOpenAILLMClient> _logger;
    private readonly AzureOpenAIClient? _openAIClient;

    public AzureOpenAILLMClient(
        IOptions<AzureOpenAISettings> settings,
        ILogger<AzureOpenAILLMClient> logger)
    {
        _settings = settings.Value;
        _logger = logger;

        if (_settings.Enabled && !string.IsNullOrEmpty(_settings.Endpoint) && !string.IsNullOrEmpty(_settings.ApiKey))
        {
            try
            {
                _openAIClient = new AzureOpenAIClient(
                    new Uri(_settings.Endpoint),
                    new AzureKeyCredential(_settings.ApiKey));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize Azure OpenAI client");
            }
        }
    }

    public async Task<LLMResponse> GetChatCompletionAsync(
        string systemPrompt,
        string userMessage,
        IEnumerable<LLMMessage>? conversationHistory = null,
        CancellationToken cancellationToken = default)
    {
        if (_openAIClient == null || !_settings.Enabled)
        {
            _logger.LogWarning("Azure OpenAI client is not initialized or disabled, returning fallback response");
            return GetFallbackResponse();
        }

        try
        {
            // Build the chat messages list
            var messages = new List<ChatMessage>
            {
                new SystemChatMessage(systemPrompt)
            };

            // Add conversation history if provided
            if (conversationHistory != null)
            {
                foreach (var historyMessage in conversationHistory)
                {
                    if (historyMessage.Role.Equals("user", StringComparison.OrdinalIgnoreCase))
                    {
                        messages.Add(new UserChatMessage(historyMessage.Content));
                    }
                    else if (historyMessage.Role.Equals("assistant", StringComparison.OrdinalIgnoreCase))
                    {
                        messages.Add(new AssistantChatMessage(historyMessage.Content));
                    }
                }
            }

            // Add the current user message
            messages.Add(new UserChatMessage(userMessage));

            // Create chat completion options
            var chatOptions = new ChatCompletionOptions
            {
                Temperature = (float)_settings.Temperature,
                MaxOutputTokenCount = _settings.MaxTokens
            };

            // Get the chat client
            var chatClient = _openAIClient.GetChatClient(_settings.DeploymentName);

            // Make the API call with retry logic
            var response = await RetryAsync(async () =>
            {
                var result = await chatClient.CompleteChatAsync(messages, chatOptions, cancellationToken);
                return result.Value;
            }, _settings.MaxRetries);

            // Extract response content
            var content = response.Content.FirstOrDefault()?.Text ?? string.Empty;
            var usage = response.Usage;

            _logger.LogInformation(
                "Azure OpenAI request completed. Tokens used: {TotalTokens} (Prompt: {PromptTokens}, Completion: {CompletionTokens})",
                usage.InputTokenCount + usage.OutputTokenCount,
                usage.InputTokenCount,
                usage.OutputTokenCount);

            return new LLMResponse
            {
                Content = content,
                TotalTokens = usage.InputTokenCount + usage.OutputTokenCount,
                PromptTokens = usage.InputTokenCount,
                CompletionTokens = usage.OutputTokenCount
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling Azure OpenAI API");
            return GetFallbackResponse();
        }
    }

    private async Task<T> RetryAsync<T>(Func<Task<T>> operation, int maxRetries)
    {
        int attempt = 0;
        while (true)
        {
            try
            {
                return await operation();
            }
            catch (Exception ex) when (attempt < maxRetries)
            {
                attempt++;
                var delay = TimeSpan.FromSeconds(Math.Pow(2, attempt)); // Exponential backoff
                _logger.LogWarning(ex, "Azure OpenAI request failed (attempt {Attempt}/{MaxRetries}). Retrying in {Delay}s", 
                    attempt, maxRetries, delay.TotalSeconds);
                await Task.Delay(delay);
            }
        }
    }

    private LLMResponse GetFallbackResponse()
    {
        return new LLMResponse
        {
            Content = "I apologize, but I'm experiencing technical difficulties right now. " +
                     "Please try again in a few moments, or contact support if the issue persists.",
            TotalTokens = 0,
            PromptTokens = 0,
            CompletionTokens = 0
        };
    }
}
