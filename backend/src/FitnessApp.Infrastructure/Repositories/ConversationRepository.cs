using Microsoft.EntityFrameworkCore;
using FitnessApp.Application.Common.Interfaces;
using FitnessApp.Domain.Entities;
using FitnessApp.Infrastructure.Persistence;

namespace FitnessApp.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for conversation persistence
/// </summary>
public class ConversationRepository : IConversationRepository
{
    private readonly ApplicationDbContext _context;

    public ConversationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Conversation?> GetByIdAsync(Guid conversationId, CancellationToken cancellationToken = default)
    {
        return await _context.Conversations
            .FirstOrDefaultAsync(c => c.Id == conversationId, cancellationToken);
    }

    public async Task<Conversation?> GetByIdWithMessagesAsync(Guid conversationId, CancellationToken cancellationToken = default)
    {
        return await _context.Conversations
            .Include(c => c.Messages.OrderBy(m => m.Timestamp))
            .FirstOrDefaultAsync(c => c.Id == conversationId, cancellationToken);
    }

    public async Task<Conversation?> GetActiveConversationByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Conversations
            .Include(c => c.Messages.OrderBy(m => m.Timestamp))
            .Where(c => c.UserId == userId && c.IsActive)
            .OrderByDescending(c => c.UpdatedAt ?? c.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Conversation> AddAsync(Conversation conversation, CancellationToken cancellationToken = default)
    {
        conversation.CreatedAt = DateTime.UtcNow;
        await _context.Conversations.AddAsync(conversation, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return conversation;
    }

    public async Task UpdateAsync(Conversation conversation, CancellationToken cancellationToken = default)
    {
        conversation.UpdatedAt = DateTime.UtcNow;
        _context.Conversations.Update(conversation);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid conversationId, CancellationToken cancellationToken = default)
    {
        var conversation = await GetByIdAsync(conversationId, cancellationToken);
        if (conversation != null)
        {
            _context.Conversations.Remove(conversation);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
