using Microsoft.EntityFrameworkCore;
using FitnessApp.Application.Common.Interfaces;
using FitnessApp.Domain.Entities;

namespace FitnessApp.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for UserProfile entity operations
/// </summary>
public class UserProfileRepository : IUserProfileRepository
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the UserProfileRepository
    /// </summary>
    /// <param name="context">The database context</param>
    public UserProfileRepository(IApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc />
    public async Task<UserProfile?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.UserProfiles
            .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<UserProfile?> GetByIdAsync(Guid profileId, CancellationToken cancellationToken = default)
    {
        return await _context.UserProfiles
            .FirstOrDefaultAsync(p => p.Id == profileId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<UserProfile?> GetProfileWithGoalsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.UserProfiles
            .Include(p => p.TrainingGoals)
            .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<UserProfile?> GetProfileWithInjuriesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.UserProfiles
            .Include(p => p.InjuryLimitations)
            .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<UserProfile> CreateAsync(UserProfile profile, CancellationToken cancellationToken = default)
    {
        if (profile == null)
            throw new ArgumentNullException(nameof(profile));

        // Check if a profile already exists for this user
        var existingProfile = await GetByUserIdAsync(profile.UserId, cancellationToken);
        if (existingProfile != null)
            throw new InvalidOperationException($"A profile already exists for user {profile.UserId}");

        _context.UserProfiles.Add(profile);
        await _context.SaveChangesAsync(cancellationToken);
        return profile;
    }

    /// <inheritdoc />
    public async Task<UserProfile> UpdateAsync(UserProfile profile, CancellationToken cancellationToken = default)
    {
        if (profile == null)
            throw new ArgumentNullException(nameof(profile));

        var existingProfile = await GetByIdAsync(profile.Id, cancellationToken);
        if (existingProfile == null)
            throw new InvalidOperationException($"Profile with ID {profile.Id} not found");

        _context.UserProfiles.Update(profile);
        await _context.SaveChangesAsync(cancellationToken);
        return profile;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var profile = await GetByUserIdAsync(userId, cancellationToken);
        if (profile == null)
            return false;

        _context.UserProfiles.Remove(profile);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    /// <inheritdoc />
    public async Task<UserProfile?> GetCompleteProfileAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.UserProfiles
            .Include(p => p.TrainingGoals)
            .Include(p => p.InjuryLimitations)
            .Include(p => p.TrainingBackground)
            .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<TrainingGoal?> GetGoalByIdAsync(Guid goalId, CancellationToken cancellationToken = default)
    {
        return await _context.TrainingGoals
            .FirstOrDefaultAsync(g => g.Id == goalId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<InjuryLimitation?> GetInjuryByIdAsync(Guid injuryId, CancellationToken cancellationToken = default)
    {
        return await _context.InjuryLimitations
            .FirstOrDefaultAsync(i => i.Id == injuryId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<TrainingGoal> AddGoalAsync(TrainingGoal goal, CancellationToken cancellationToken = default)
    {
        if (goal == null)
            throw new ArgumentNullException(nameof(goal));

        _context.TrainingGoals.Add(goal);
        await _context.SaveChangesAsync(cancellationToken);
        return goal;
    }

    /// <inheritdoc />
    public async Task<TrainingGoal> UpdateGoalAsync(TrainingGoal goal, CancellationToken cancellationToken = default)
    {
        if (goal == null)
            throw new ArgumentNullException(nameof(goal));

        var existingGoal = await GetGoalByIdAsync(goal.Id, cancellationToken);
        if (existingGoal == null)
            throw new InvalidOperationException($"Goal with ID {goal.Id} not found");

        _context.TrainingGoals.Update(goal);
        await _context.SaveChangesAsync(cancellationToken);
        return goal;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteGoalAsync(Guid goalId, CancellationToken cancellationToken = default)
    {
        var goal = await GetGoalByIdAsync(goalId, cancellationToken);
        if (goal == null)
            return false;

        _context.TrainingGoals.Remove(goal);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    /// <inheritdoc />
    public async Task<InjuryLimitation> AddInjuryAsync(InjuryLimitation injury, CancellationToken cancellationToken = default)
    {
        if (injury == null)
            throw new ArgumentNullException(nameof(injury));

        _context.InjuryLimitations.Add(injury);
        await _context.SaveChangesAsync(cancellationToken);
        return injury;
    }

    /// <inheritdoc />
    public async Task<InjuryLimitation> UpdateInjuryAsync(InjuryLimitation injury, CancellationToken cancellationToken = default)
    {
        if (injury == null)
            throw new ArgumentNullException(nameof(injury));

        var existingInjury = await GetInjuryByIdAsync(injury.Id, cancellationToken);
        if (existingInjury == null)
            throw new InvalidOperationException($"Injury with ID {injury.Id} not found");

        _context.InjuryLimitations.Update(injury);
        await _context.SaveChangesAsync(cancellationToken);
        return injury;
    }
}
