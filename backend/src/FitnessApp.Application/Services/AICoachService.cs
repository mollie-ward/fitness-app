using Microsoft.Extensions.Logging;
using FitnessApp.Application.Common.Interfaces;
using FitnessApp.Application.DTOs;
using FitnessApp.Domain.Entities;
using FitnessApp.Domain.Enums;

namespace FitnessApp.Application.Services;

/// <summary>
/// Service implementation for AI Coach operations (Coach Tom)
/// </summary>
public class AICoachService : IAICoachService
{
    private readonly IConversationRepository _conversationRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly ILLMClient _llmClient;
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly ITrainingPlanRepository _trainingPlanRepository;
    private readonly IProgressTrackingService _progressTrackingService;
    private readonly ILogger<AICoachService> _logger;

    public AICoachService(
        IConversationRepository conversationRepository,
        IMessageRepository messageRepository,
        ILLMClient llmClient,
        IUserProfileRepository userProfileRepository,
        ITrainingPlanRepository trainingPlanRepository,
        IProgressTrackingService progressTrackingService,
        ILogger<AICoachService> logger)
    {
        _conversationRepository = conversationRepository;
        _messageRepository = messageRepository;
        _llmClient = llmClient;
        _userProfileRepository = userProfileRepository;
        _trainingPlanRepository = trainingPlanRepository;
        _progressTrackingService = progressTrackingService;
        _logger = logger;
    }

    public async Task<ChatMessageResponseDto> SendMessageAsync(
        Guid userId,
        string message,
        Guid? conversationId = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Processing chat message for user {UserId}", userId);

        // Get or create conversation
        Conversation conversation;
        if (conversationId.HasValue)
        {
            conversation = await _conversationRepository.GetByIdWithMessagesAsync(conversationId.Value, cancellationToken)
                ?? throw new KeyNotFoundException($"Conversation {conversationId} not found");

            // Verify ownership
            if (conversation.UserId != userId)
            {
                throw new UnauthorizedAccessException("User does not own this conversation");
            }
        }
        else
        {
            // Get or create active conversation for the user
            conversation = await _conversationRepository.GetActiveConversationByUserIdAsync(userId, cancellationToken);
            if (conversation == null)
            {
                conversation = new Conversation
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    IsActive = true
                };
                conversation = await _conversationRepository.AddAsync(conversation, cancellationToken);
            }
        }

        // Recognize intent
        var intent = RecognizeIntent(message);
        _logger.LogInformation("Recognized intent: {Intent} for message from user {UserId}", intent, userId);

        // Build context for the LLM
        var systemPrompt = await BuildSystemPromptAsync(userId, cancellationToken);

        // Get conversation history for context
        var historyMessages = conversation.Messages
            .OrderByDescending(m => m.Timestamp)
            .Take(10)  // Last 10 messages for context
            .Reverse()
            .Select(m => new LLMMessage
            {
                Role = m.Role == MessageRole.User ? "user" : "assistant",
                Content = m.Content
            })
            .ToList();

        // Get LLM response
        var llmResponse = await _llmClient.GetChatCompletionAsync(
            systemPrompt,
            message,
            historyMessages,
            cancellationToken);

        // Save user message
        var userMessage = new Message
        {
            Id = Guid.NewGuid(),
            ConversationId = conversation.Id,
            Role = MessageRole.User,
            Content = message,
            Intent = intent,
            Timestamp = DateTime.UtcNow
        };
        await _messageRepository.AddAsync(userMessage, cancellationToken);

        // Handle intent-specific actions
        string? triggeredAction = null;
        if (intent != MessageIntent.Unknown && intent != MessageIntent.GeneralQuestion)
        {
            triggeredAction = await HandleIntentAsync(userId, intent, message, cancellationToken);
        }

        // Save assistant response
        var assistantMessage = new Message
        {
            Id = Guid.NewGuid(),
            ConversationId = conversation.Id,
            Role = MessageRole.Assistant,
            Content = llmResponse.Content,
            Timestamp = DateTime.UtcNow,
            TokenCount = llmResponse.TotalTokens,
            TriggeredAction = triggeredAction
        };
        await _messageRepository.AddAsync(assistantMessage, cancellationToken);

        // Update conversation
        conversation.UpdatedAt = DateTime.UtcNow;
        await _conversationRepository.UpdateAsync(conversation, cancellationToken);

        _logger.LogInformation(
            "Chat response generated for user {UserId}. Tokens: {Tokens}, Intent: {Intent}",
            userId, llmResponse.TotalTokens, intent);

        return new ChatMessageResponseDto
        {
            Response = llmResponse.Content,
            ConversationId = conversation.Id,
            Timestamp = DateTime.UtcNow,
            TriggeredAction = triggeredAction
        };
    }

    public async Task<ConversationHistoryDto> GetConversationHistoryAsync(
        Guid conversationId,
        Guid userId,
        int limit = 50,
        CancellationToken cancellationToken = default)
    {
        var conversation = await _conversationRepository.GetByIdWithMessagesAsync(conversationId, cancellationToken)
            ?? throw new KeyNotFoundException($"Conversation {conversationId} not found");

        // Verify ownership
        if (conversation.UserId != userId)
        {
            throw new UnauthorizedAccessException("User does not own this conversation");
        }

        var messages = conversation.Messages
            .OrderBy(m => m.Timestamp)
            .Take(limit)
            .Select(m => new MessageDto
            {
                Id = m.Id,
                Role = m.Role,
                Content = m.Content,
                Timestamp = m.Timestamp,
                Intent = m.Intent,
                TriggeredAction = m.TriggeredAction
            });

        return new ConversationHistoryDto
        {
            ConversationId = conversation.Id,
            UserId = conversation.UserId,
            Messages = messages,
            TotalMessages = conversation.Messages.Count,
            CreatedAt = conversation.CreatedAt,
            UpdatedAt = conversation.UpdatedAt
        };
    }

    public async Task ClearConversationAsync(
        Guid conversationId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var conversation = await _conversationRepository.GetByIdAsync(conversationId, cancellationToken)
            ?? throw new KeyNotFoundException($"Conversation {conversationId} not found");

        // Verify ownership
        if (conversation.UserId != userId)
        {
            throw new UnauthorizedAccessException("User does not own this conversation");
        }

        // Delete all messages
        await _messageRepository.DeleteByConversationIdAsync(conversationId, cancellationToken);

        // Mark conversation as inactive
        conversation.IsActive = false;
        await _conversationRepository.UpdateAsync(conversation, cancellationToken);

        _logger.LogInformation("Conversation {ConversationId} cleared for user {UserId}", conversationId, userId);
    }

    public Task<string> GetCoachAvatarUrlAsync()
    {
        // Return static avatar URL (could be configured or stored in blob storage)
        return Task.FromResult("/assets/coach-tom-avatar.png");
    }

    #region Private Helper Methods

    /// <summary>
    /// Builds the system prompt with Coach Tom's personality and user context
    /// </summary>
    private async Task<string> BuildSystemPromptAsync(Guid userId, CancellationToken cancellationToken)
    {
        // Get user profile
        var profile = await _userProfileRepository.GetByUserIdAsync(userId, cancellationToken);

        var systemPrompt = @"You are Coach Tom, a knowledgeable and supportive fitness coach specializing in HYROX and endurance training.

**Your Personality:**
- Expert in fitness, particularly HYROX events and endurance training
- Supportive and motivational without being pushy
- Adaptable communication style (beginner-friendly to technical as needed)
- Honest and realistic about expectations
- Empathetic to challenges

**Your Guidelines:**
- Explain workout rationale in accessible language
- Provide motivation without being forceful
- Acknowledge challenges without judgment
- Set realistic expectations
- Ask clarifying questions when needed

**Safety Disclaimers:**
- You are NOT a medical professional and cannot diagnose or treat medical conditions
- Always recommend consulting healthcare professionals for injuries or medical concerns
- Never guarantee specific outcomes
- Encourage safe training practices

";

        if (profile != null)
        {
            systemPrompt += $@"
**Current User Context:**
- Name: {profile.Name}
- Fitness Levels: HYROX={profile.HyroxLevel}, Running={profile.RunningLevel}, Strength={profile.StrengthLevel}
";

            if (profile.TrainingGoals?.Any() == true)
            {
                systemPrompt += $"- Goals: {string.Join(", ", profile.TrainingGoals.Select(g => g.Description))}\n";
            }

            if (profile.InjuryLimitations?.Any() == true)
            {
                var activeInjuries = profile.InjuryLimitations.Where(i => i.Status == InjuryStatus.Active);
                if (activeInjuries.Any())
                {
                    systemPrompt += $"- Active Injuries/Limitations: {string.Join(", ", activeInjuries.Select(i => $"{i.BodyPart} ({i.InjuryType})"))}\n";
                }
            }

            // Get current plan info
            var activePlan = await _trainingPlanRepository.GetActivePlanByUserIdAsync(userId, cancellationToken);
            if (activePlan != null)
            {
                systemPrompt += $@"- Current Plan: {activePlan.PlanName}
- Training Days/Week: {activePlan.TrainingDaysPerWeek}
- Plan Status: {activePlan.Status}
- Current Week: {activePlan.CurrentWeek} of {activePlan.TotalWeeks}
";
            }
        }

        systemPrompt += @"

When answering questions:
1. Reference the user's specific context when relevant
2. Be concise but thorough
3. Include disclaimers for medical/injury-related questions
4. Offer encouragement and motivation naturally
5. If unsure, ask clarifying questions";

        return systemPrompt;
    }

    /// <summary>
    /// Recognizes the intent of a user message
    /// </summary>
    private MessageIntent RecognizeIntent(string message)
    {
        var lowerMessage = message.ToLowerInvariant();

        // Workout rationale patterns
        if (lowerMessage.Contains("why") && (
            lowerMessage.Contains("workout") ||
            lowerMessage.Contains("exercise") ||
            lowerMessage.Contains("training")))
        {
            return MessageIntent.WorkoutRationale;
        }

        // Injury report patterns
        if (lowerMessage.Contains("injury") ||
            lowerMessage.Contains("injured") ||
            lowerMessage.Contains("hurt") ||
            lowerMessage.Contains("pain") ||
            lowerMessage.Contains("sore"))
        {
            return MessageIntent.InjuryReport;
        }

        // Plan modification patterns
        if ((lowerMessage.Contains("too") && (lowerMessage.Contains("hard") || lowerMessage.Contains("easy"))) ||
            lowerMessage.Contains("make it harder") ||
            lowerMessage.Contains("make it easier") ||
            lowerMessage.Contains("struggling") ||
            lowerMessage.Contains("modify"))
        {
            return MessageIntent.PlanModification;
        }

        // Schedule change patterns
        if (lowerMessage.Contains("schedule") ||
            (lowerMessage.Contains("can only") && lowerMessage.Contains("day")))
        {
            return MessageIntent.ScheduleChange;
        }

        // Motivation patterns
        if (lowerMessage.Contains("motivat") ||
            lowerMessage.Contains("tired") ||
            lowerMessage.Contains("don't want") ||
            lowerMessage.Contains("feeling down"))
        {
            return MessageIntent.Motivation;
        }

        // Out of scope patterns
        if (!lowerMessage.Contains("workout") &&
            !lowerMessage.Contains("training") &&
            !lowerMessage.Contains("exercise") &&
            !lowerMessage.Contains("fitness") &&
            !lowerMessage.Contains("hyrox") &&
            !lowerMessage.Contains("plan"))
        {
            return MessageIntent.OutOfScope;
        }

        return MessageIntent.GeneralQuestion;
    }

    /// <summary>
    /// Handles intent-specific actions (e.g., updating profile, modifying plan)
    /// </summary>
    private async Task<string?> HandleIntentAsync(
        Guid userId,
        MessageIntent intent,
        string message,
        CancellationToken cancellationToken)
    {
        try
        {
            switch (intent)
            {
                case MessageIntent.InjuryReport:
                    _logger.LogInformation("Injury reported by user {UserId}, manual review recommended", userId);
                    return "injury_noted";

                case MessageIntent.PlanModification:
                    _logger.LogInformation("Plan modification requested by user {UserId}, manual review recommended", userId);
                    return "modification_requested";

                case MessageIntent.ScheduleChange:
                    _logger.LogInformation("Schedule change requested by user {UserId}, manual review recommended", userId);
                    return "schedule_change_requested";

                default:
                    return null;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling intent {Intent} for user {UserId}", intent, userId);
            return null;
        }
    }

    #endregion
}
