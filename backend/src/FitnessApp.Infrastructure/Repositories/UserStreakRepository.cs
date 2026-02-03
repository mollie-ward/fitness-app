using Microsoft.EntityFrameworkCore;
using FitnessApp.Application.Common.Interfaces;
using FitnessApp.Domain.Entities;
using FitnessApp.Infrastructure.Persistence;

namespace FitnessApp.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for user streak operations
/// </summary>
public class UserStreakRepository : IUserStreakRepository
{
    private readonly ApplicationDbContext _context;

    public UserStreakRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UserStreak?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.UserStreaks
            .FirstOrDefaultAsync(us => us.UserId == userId, cancellationToken);
    }

    public async Task<UserStreak> AddAsync(UserStreak userStreak, CancellationToken cancellationToken = default)
    {
        await _context.UserStreaks.AddAsync(userStreak, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return userStreak;
    }

    public async Task UpdateAsync(UserStreak userStreak, CancellationToken cancellationToken = default)
    {
        _context.UserStreaks.Update(userStreak);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<UserStreak> GetOrCreateAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var streak = await GetByUserIdAsync(userId, cancellationToken);
        if (streak != null)
        {
            return streak;
        }

        var newStreak = new UserStreak
        {
            UserId = userId,
            CurrentStreak = 0,
            LongestStreak = 0,
            CurrentWeeklyStreak = 0,
            LongestWeeklyStreak = 0,
            LastWorkoutDate = null
        };

        return await AddAsync(newStreak, cancellationToken);
    }
}
