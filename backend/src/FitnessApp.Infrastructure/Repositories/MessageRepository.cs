using Microsoft.EntityFrameworkCore;
using FitnessApp.Application.Common.Interfaces;
using FitnessApp.Domain.Entities;
using FitnessApp.Infrastructure.Persistence;

namespace FitnessApp.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for message persistence
/// </summary>
public class MessageRepository : IMessageRepository
{
    private readonly ApplicationDbContext _context;

    public MessageRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Message>> GetMessagesByConversationIdAsync(
        Guid conversationId, 
        int limit = 50,
        CancellationToken cancellationToken = default)
    {
        if (limit <= 0)
        {
            throw new ArgumentException("Limit must be greater than 0", nameof(limit));
        }

        return await _context.Messages
            .Where(m => m.ConversationId == conversationId)
            .OrderBy(m => m.Timestamp)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<Message> AddAsync(Message message, CancellationToken cancellationToken = default)
    {
        message.CreatedAt = DateTime.UtcNow;
        message.Timestamp = message.Timestamp == default ? DateTime.UtcNow : message.Timestamp;
        await _context.Messages.AddAsync(message, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return message;
    }

    public async Task AddRangeAsync(IEnumerable<Message> messages, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        foreach (var message in messages)
        {
            message.CreatedAt = now;
            message.Timestamp = message.Timestamp == default ? now : message.Timestamp;
        }
        await _context.Messages.AddRangeAsync(messages, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteByConversationIdAsync(Guid conversationId, CancellationToken cancellationToken = default)
    {
        var messages = await _context.Messages
            .Where(m => m.ConversationId == conversationId)
            .ToListAsync(cancellationToken);

        _context.Messages.RemoveRange(messages);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
