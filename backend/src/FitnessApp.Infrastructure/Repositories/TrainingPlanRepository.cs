using Microsoft.EntityFrameworkCore;
using FitnessApp.Application.Common.Interfaces;
using FitnessApp.Domain.Entities;
using FitnessApp.Domain.Enums;

namespace FitnessApp.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for TrainingPlan entity operations
/// </summary>
public class TrainingPlanRepository : ITrainingPlanRepository
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the TrainingPlanRepository
    /// </summary>
    /// <param name="context">The database context</param>
    public TrainingPlanRepository(IApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc />
    public async Task<TrainingPlan?> GetActivePlanByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.TrainingPlans
            .FirstOrDefaultAsync(p => p.UserId == userId && p.Status == PlanStatus.Active, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<TrainingPlan?> GetByIdAsync(Guid planId, CancellationToken cancellationToken = default)
    {
        return await _context.TrainingPlans
            .FirstOrDefaultAsync(p => p.Id == planId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<TrainingPlan> CreatePlanAsync(TrainingPlan plan, CancellationToken cancellationToken = default)
    {
        if (plan == null)
            throw new ArgumentNullException(nameof(plan));

        _context.TrainingPlans.Add(plan);
        await _context.SaveChangesAsync(cancellationToken);
        return plan;
    }

    /// <inheritdoc />
    public async Task<TrainingPlan> UpdatePlanAsync(TrainingPlan plan, CancellationToken cancellationToken = default)
    {
        if (plan == null)
            throw new ArgumentNullException(nameof(plan));

        var existingPlan = await GetByIdAsync(plan.Id, cancellationToken);
        if (existingPlan == null)
            throw new InvalidOperationException($"Training plan with ID {plan.Id} not found");

        _context.TrainingPlans.Update(plan);
        await _context.SaveChangesAsync(cancellationToken);
        return plan;
    }

    /// <inheritdoc />
    public async Task<TrainingPlan?> GetPlanWithWeeksAsync(Guid planId, CancellationToken cancellationToken = default)
    {
        var plan = await _context.TrainingPlans
            .Include(p => p.TrainingWeeks)
                .ThenInclude(w => w.Workouts)
            .Include(p => p.PlanMetadata)
            .FirstOrDefaultAsync(p => p.Id == planId, cancellationToken);

        if (plan != null)
        {
            // Order in memory after loading
            plan.TrainingWeeks = plan.TrainingWeeks.OrderBy(w => w.WeekNumber).ToList();
            foreach (var week in plan.TrainingWeeks)
            {
                week.Workouts = week.Workouts.OrderBy(wo => wo.DayOfWeek).ToList();
            }
        }

        return plan;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Workout>> GetCurrentWeekWorkoutsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var activePlan = await _context.TrainingPlans
            .Where(p => p.UserId == userId && p.Status == PlanStatus.Active)
            .Include(p => p.TrainingWeeks)
                .ThenInclude(w => w.Workouts)
            .FirstOrDefaultAsync(cancellationToken);

        if (activePlan == null)
            return Enumerable.Empty<Workout>();

        var currentWeek = activePlan.TrainingWeeks
            .FirstOrDefault(w => w.WeekNumber == activePlan.CurrentWeek);

        return currentWeek?.Workouts.OrderBy(w => w.DayOfWeek).ToList() ?? Enumerable.Empty<Workout>();
    }

    /// <inheritdoc />
    public async Task<bool> DeletePlanAsync(Guid planId, CancellationToken cancellationToken = default)
    {
        var plan = await _context.TrainingPlans
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(p => p.Id == planId, cancellationToken);

        if (plan == null)
            return false;

        plan.IsDeleted = true;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TrainingPlan>> GetPlansByUserIdAsync(Guid userId, bool includeDeleted = false, CancellationToken cancellationToken = default)
    {
        var query = _context.TrainingPlans.Where(p => p.UserId == userId);

        if (includeDeleted)
        {
            query = query.IgnoreQueryFilters();
        }

        return await query
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TrainingPlan>> GetActivePlansByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.TrainingPlans
            .Where(p => p.UserId == userId && p.Status == PlanStatus.Active)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<TrainingPlan?> GetPlanWithDetailsAsync(Guid planId, CancellationToken cancellationToken = default)
    {
        return await GetPlanWithWeeksAsync(planId, cancellationToken);
    }
}
