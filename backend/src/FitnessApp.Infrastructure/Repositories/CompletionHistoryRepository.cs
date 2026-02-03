using Microsoft.EntityFrameworkCore;
using FitnessApp.Application.Common.Interfaces;
using FitnessApp.Domain.Entities;
using FitnessApp.Infrastructure.Persistence;

namespace FitnessApp.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for completion history operations
/// </summary>
public class CompletionHistoryRepository : ICompletionHistoryRepository
{
    private readonly ApplicationDbContext _context;

    public CompletionHistoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CompletionHistory> AddAsync(CompletionHistory completionHistory, CancellationToken cancellationToken = default)
    {
        await _context.CompletionHistories.AddAsync(completionHistory, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return completionHistory;
    }

    public async Task<CompletionHistory?> GetByWorkoutIdAsync(Guid workoutId, CancellationToken cancellationToken = default)
    {
        return await _context.CompletionHistories
            .Include(ch => ch.Workout)
            .FirstOrDefaultAsync(ch => ch.WorkoutId == workoutId, cancellationToken);
    }

    public async Task<IEnumerable<CompletionHistory>> GetByUserAndDateRangeAsync(
        Guid userId, 
        DateTime startDate, 
        DateTime endDate, 
        CancellationToken cancellationToken = default)
    {
        return await _context.CompletionHistories
            .Include(ch => ch.Workout)
            .Where(ch => ch.UserId == userId && 
                         ch.CompletedAt >= startDate && 
                         ch.CompletedAt <= endDate)
            .OrderBy(ch => ch.CompletedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<CompletionHistory>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.CompletionHistories
            .Include(ch => ch.Workout)
            .Where(ch => ch.UserId == userId)
            .OrderBy(ch => ch.CompletedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task DeleteAsync(CompletionHistory completionHistory, CancellationToken cancellationToken = default)
    {
        _context.CompletionHistories.Remove(completionHistory);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<DateTime>> GetDistinctCompletionDatesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.CompletionHistories
            .Where(ch => ch.UserId == userId)
            .Select(ch => ch.CompletedAt.Date)
            .Distinct()
            .OrderBy(d => d)
            .ToListAsync(cancellationToken);
    }
}
