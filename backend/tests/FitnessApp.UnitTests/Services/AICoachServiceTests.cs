using Xunit;
using FluentAssertions;
using Moq;
using Microsoft.Extensions.Logging;
using FitnessApp.Application.Services;
using FitnessApp.Application.Common.Interfaces;
using FitnessApp.Application.DTOs;
using FitnessApp.Domain.Entities;
using FitnessApp.Domain.Enums;

namespace FitnessApp.UnitTests.Services;

/// <summary>
/// Unit tests for AICoachService
/// Tests chat functionality, intent recognition, and conversation management
/// </summary>
public class AICoachServiceTests
{
    private readonly Mock<IConversationRepository> _conversationRepositoryMock;
    private readonly Mock<IMessageRepository> _messageRepositoryMock;
    private readonly Mock<ILLMClient> _llmClientMock;
    private readonly Mock<IUserProfileRepository> _userProfileRepositoryMock;
    private readonly Mock<ITrainingPlanRepository> _trainingPlanRepositoryMock;
    private readonly Mock<IProgressTrackingService> _progressTrackingServiceMock;
    private readonly Mock<ILogger<AICoachService>> _loggerMock;
    private readonly AICoachService _service;

    public AICoachServiceTests()
    {
        _conversationRepositoryMock = new Mock<IConversationRepository>();
        _messageRepositoryMock = new Mock<IMessageRepository>();
        _llmClientMock = new Mock<ILLMClient>();
        _userProfileRepositoryMock = new Mock<IUserProfileRepository>();
        _trainingPlanRepositoryMock = new Mock<ITrainingPlanRepository>();
        _progressTrackingServiceMock = new Mock<IProgressTrackingService>();
        _loggerMock = new Mock<ILogger<AICoachService>>();

        _service = new AICoachService(
            _conversationRepositoryMock.Object,
            _messageRepositoryMock.Object,
            _llmClientMock.Object,
            _userProfileRepositoryMock.Object,
            _trainingPlanRepositoryMock.Object,
            _progressTrackingServiceMock.Object,
            _loggerMock.Object);
    }

    #region SendMessageAsync Tests

    [Fact]
    public async Task SendMessageAsync_CreatesNewConversation_WhenNoneExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var message = "Hello Coach Tom!";

        _conversationRepositoryMock
            .Setup(x => x.GetActiveConversationByUserIdAsync(userId, default))
            .ReturnsAsync((Conversation?)null);

        var newConversation = new Conversation
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _conversationRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Conversation>(), default))
            .ReturnsAsync(newConversation);

        _userProfileRepositoryMock
            .Setup(x => x.GetByUserIdAsync(userId, default))
            .ReturnsAsync((UserProfile?)null);

        _llmClientMock
            .Setup(x => x.GetChatCompletionAsync(
                It.IsAny<string>(),
                message,
                It.IsAny<IEnumerable<LLMMessage>>(),
                default))
            .ReturnsAsync(new LLMResponse
            {
                Content = "Hello! I'm Coach Tom, ready to help with your training!",
                TotalTokens = 50,
                PromptTokens = 20,
                CompletionTokens = 30
            });

        _messageRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Message>(), default))
            .ReturnsAsync((Message m, CancellationToken ct) => m);

        // Act
        var result = await _service.SendMessageAsync(userId, message, null);

        // Assert
        result.Should().NotBeNull();
        result.ConversationId.Should().Be(newConversation.Id);
        result.Response.Should().Contain("Coach Tom");
        
        _conversationRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<Conversation>(), default),
            Times.Once);
        
        _messageRepositoryMock.Verify(
            x => x.AddAsync(It.Is<Message>(m => m.Role == MessageRole.User), default),
            Times.Once);
        
        _messageRepositoryMock.Verify(
            x => x.AddAsync(It.Is<Message>(m => m.Role == MessageRole.Assistant), default),
            Times.Once);
    }

    [Fact]
    public async Task SendMessageAsync_UsesExistingConversation_WhenConversationIdProvided()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var conversationId = Guid.NewGuid();
        var message = "Why do I need to do burpees?";

        var conversation = new Conversation
        {
            Id = conversationId,
            UserId = userId,
            IsActive = true,
            Messages = new List<Message>(),
            CreatedAt = DateTime.UtcNow
        };

        _conversationRepositoryMock
            .Setup(x => x.GetByIdWithMessagesAsync(conversationId, default))
            .ReturnsAsync(conversation);

        _userProfileRepositoryMock
            .Setup(x => x.GetByUserIdAsync(userId, default))
            .ReturnsAsync((UserProfile?)null);

        _llmClientMock
            .Setup(x => x.GetChatCompletionAsync(
                It.IsAny<string>(),
                message,
                It.IsAny<IEnumerable<LLMMessage>>(),
                default))
            .ReturnsAsync(new LLMResponse
            {
                Content = "Burpees are a full-body exercise that builds cardiovascular endurance and strength...",
                TotalTokens = 100,
                PromptTokens = 40,
                CompletionTokens = 60
            });

        _messageRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Message>(), default))
            .ReturnsAsync((Message m, CancellationToken ct) => m);

        // Act
        var result = await _service.SendMessageAsync(userId, message, conversationId);

        // Assert
        result.Should().NotBeNull();
        result.ConversationId.Should().Be(conversationId);
        result.Response.Should().Contain("Burpees");
        
        _conversationRepositoryMock.Verify(
            x => x.GetByIdWithMessagesAsync(conversationId, default),
            Times.Once);
    }

    [Fact]
    public async Task SendMessageAsync_ThrowsUnauthorized_WhenUserDoesNotOwnConversation()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var conversationId = Guid.NewGuid();
        var message = "Test message";

        var conversation = new Conversation
        {
            Id = conversationId,
            UserId = otherUserId,  // Different user
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _conversationRepositoryMock
            .Setup(x => x.GetByIdWithMessagesAsync(conversationId, default))
            .ReturnsAsync(conversation);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _service.SendMessageAsync(userId, message, conversationId));
    }

    [Fact]
    public async Task SendMessageAsync_RecognizesWorkoutRationaleIntent()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var message = "Why do I need to do this workout?";

        SetupBasicMessageFlow(userId, out var conversationId);

        // Act
        var result = await _service.SendMessageAsync(userId, message, null);

        // Assert
        _messageRepositoryMock.Verify(
            x => x.AddAsync(
                It.Is<Message>(m => 
                    m.Role == MessageRole.User && 
                    m.Intent == MessageIntent.WorkoutRationale),
                default),
            Times.Once);
    }

    [Fact]
    public async Task SendMessageAsync_RecognizesInjuryReportIntent()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var message = "I have pain in my knee";

        SetupBasicMessageFlow(userId, out var conversationId);

        // Act
        var result = await _service.SendMessageAsync(userId, message, null);

        // Assert
        _messageRepositoryMock.Verify(
            x => x.AddAsync(
                It.Is<Message>(m => 
                    m.Role == MessageRole.User && 
                    m.Intent == MessageIntent.InjuryReport),
                default),
            Times.Once);
        
        _messageRepositoryMock.Verify(
            x => x.AddAsync(
                It.Is<Message>(m => 
                    m.Role == MessageRole.Assistant && 
                    m.TriggeredAction == "injury_noted"),
                default),
            Times.Once);
        
        result.TriggeredAction.Should().Be("injury_noted");
    }

    [Fact]
    public async Task SendMessageAsync_RecognizesPlanModificationIntent()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var message = "This workout is too hard for me";

        SetupBasicMessageFlow(userId, out var conversationId);

        // Act
        var result = await _service.SendMessageAsync(userId, message, null);

        // Assert
        _messageRepositoryMock.Verify(
            x => x.AddAsync(
                It.Is<Message>(m => 
                    m.Role == MessageRole.User && 
                    m.Intent == MessageIntent.PlanModification),
                default),
            Times.Once);
        
        _messageRepositoryMock.Verify(
            x => x.AddAsync(
                It.Is<Message>(m => 
                    m.Role == MessageRole.Assistant && 
                    m.TriggeredAction == "modification_requested"),
                default),
            Times.Once);
        
        result.TriggeredAction.Should().Be("modification_requested");
    }

    [Fact]
    public async Task SendMessageAsync_RecognizesScheduleChangeIntent()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var message = "I need to change my training schedule";

        SetupBasicMessageFlow(userId, out var conversationId);

        // Act
        var result = await _service.SendMessageAsync(userId, message, null);

        // Assert
        _messageRepositoryMock.Verify(
            x => x.AddAsync(
                It.Is<Message>(m => 
                    m.Role == MessageRole.User && 
                    m.Intent == MessageIntent.ScheduleChange),
                default),
            Times.Once);
        
        _messageRepositoryMock.Verify(
            x => x.AddAsync(
                It.Is<Message>(m => 
                    m.Role == MessageRole.Assistant && 
                    m.TriggeredAction == "schedule_change_requested"),
                default),
            Times.Once);
    }

    [Fact]
    public async Task SendMessageAsync_RecognizesMotivationIntent()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var message = "I'm feeling really tired and unmotivated today";

        SetupBasicMessageFlow(userId, out var conversationId);

        // Act
        var result = await _service.SendMessageAsync(userId, message, null);

        // Assert
        _messageRepositoryMock.Verify(
            x => x.AddAsync(
                It.Is<Message>(m => 
                    m.Role == MessageRole.User && 
                    m.Intent == MessageIntent.Motivation),
                default),
            Times.Once);
    }

    #endregion

    #region GetConversationHistoryAsync Tests

    [Fact]
    public async Task GetConversationHistoryAsync_ReturnsHistory_WhenConversationExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var conversationId = Guid.NewGuid();

        var messages = new List<Message>
        {
            new Message
            {
                Id = Guid.NewGuid(),
                ConversationId = conversationId,
                Role = MessageRole.User,
                Content = "Hello",
                Timestamp = DateTime.UtcNow.AddMinutes(-5),
                CreatedAt = DateTime.UtcNow.AddMinutes(-5)
            },
            new Message
            {
                Id = Guid.NewGuid(),
                ConversationId = conversationId,
                Role = MessageRole.Assistant,
                Content = "Hi! How can I help?",
                Timestamp = DateTime.UtcNow.AddMinutes(-4),
                CreatedAt = DateTime.UtcNow.AddMinutes(-4)
            }
        };

        var conversation = new Conversation
        {
            Id = conversationId,
            UserId = userId,
            Messages = messages,
            CreatedAt = DateTime.UtcNow.AddMinutes(-10)
        };

        _conversationRepositoryMock
            .Setup(x => x.GetByIdWithMessagesAsync(conversationId, default))
            .ReturnsAsync(conversation);

        // Act
        var result = await _service.GetConversationHistoryAsync(conversationId, userId);

        // Assert
        result.Should().NotBeNull();
        result.ConversationId.Should().Be(conversationId);
        result.UserId.Should().Be(userId);
        result.Messages.Should().HaveCount(2);
        result.TotalMessages.Should().Be(2);
    }

    [Fact]
    public async Task GetConversationHistoryAsync_ThrowsNotFound_WhenConversationDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var conversationId = Guid.NewGuid();

        _conversationRepositoryMock
            .Setup(x => x.GetByIdWithMessagesAsync(conversationId, default))
            .ReturnsAsync((Conversation?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _service.GetConversationHistoryAsync(conversationId, userId));
    }

    [Fact]
    public async Task GetConversationHistoryAsync_ThrowsUnauthorized_WhenUserDoesNotOwnConversation()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var conversationId = Guid.NewGuid();

        var conversation = new Conversation
        {
            Id = conversationId,
            UserId = otherUserId,
            Messages = new List<Message>(),
            CreatedAt = DateTime.UtcNow
        };

        _conversationRepositoryMock
            .Setup(x => x.GetByIdWithMessagesAsync(conversationId, default))
            .ReturnsAsync(conversation);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _service.GetConversationHistoryAsync(conversationId, userId));
    }

    #endregion

    #region ClearConversationAsync Tests

    [Fact]
    public async Task ClearConversationAsync_ClearsMessages_AndMarksInactive()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var conversationId = Guid.NewGuid();

        var conversation = new Conversation
        {
            Id = conversationId,
            UserId = userId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _conversationRepositoryMock
            .Setup(x => x.GetByIdAsync(conversationId, default))
            .ReturnsAsync(conversation);

        // Act
        await _service.ClearConversationAsync(conversationId, userId);

        // Assert
        _messageRepositoryMock.Verify(
            x => x.DeleteByConversationIdAsync(conversationId, default),
            Times.Once);
        
        _conversationRepositoryMock.Verify(
            x => x.UpdateAsync(
                It.Is<Conversation>(c => c.Id == conversationId && !c.IsActive),
                default),
            Times.Once);
    }

    [Fact]
    public async Task ClearConversationAsync_ThrowsUnauthorized_WhenUserDoesNotOwnConversation()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var conversationId = Guid.NewGuid();

        var conversation = new Conversation
        {
            Id = conversationId,
            UserId = otherUserId,
            CreatedAt = DateTime.UtcNow
        };

        _conversationRepositoryMock
            .Setup(x => x.GetByIdAsync(conversationId, default))
            .ReturnsAsync(conversation);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _service.ClearConversationAsync(conversationId, userId));
    }

    #endregion

    #region GetCoachAvatarUrlAsync Tests

    [Fact]
    public async Task GetCoachAvatarUrlAsync_ReturnsAvatarUrl()
    {
        // Act
        var result = await _service.GetCoachAvatarUrlAsync();

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().Contain("avatar");
    }

    #endregion

    #region Helper Methods

    private void SetupBasicMessageFlow(Guid userId, out Guid conversationId)
    {
        conversationId = Guid.NewGuid();
        
        _conversationRepositoryMock
            .Setup(x => x.GetActiveConversationByUserIdAsync(userId, default))
            .ReturnsAsync((Conversation?)null);

        var newConversation = new Conversation
        {
            Id = conversationId,
            UserId = userId,
            IsActive = true,
            Messages = new List<Message>(),
            CreatedAt = DateTime.UtcNow
        };

        _conversationRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Conversation>(), default))
            .ReturnsAsync(newConversation);

        _userProfileRepositoryMock
            .Setup(x => x.GetByUserIdAsync(userId, default))
            .ReturnsAsync((UserProfile?)null);

        _llmClientMock
            .Setup(x => x.GetChatCompletionAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<IEnumerable<LLMMessage>>(),
                default))
            .ReturnsAsync(new LLMResponse
            {
                Content = "Coach Tom's response",
                TotalTokens = 50,
                PromptTokens = 20,
                CompletionTokens = 30
            });

        _messageRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Message>(), default))
            .ReturnsAsync((Message m, CancellationToken ct) => m);

        _conversationRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Conversation>(), default))
            .Returns(Task.CompletedTask);
    }

    #endregion
}
