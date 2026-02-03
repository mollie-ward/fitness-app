using Microsoft.EntityFrameworkCore;
using FitnessApp.Application.Common.Interfaces;
using FitnessApp.Domain.Entities;
using FitnessApp.Domain.Enums;

namespace FitnessApp.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Workout entity operations
/// </summary>
public class WorkoutRepository : IWorkoutRepository
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the WorkoutRepository
    /// </summary>
    /// <param name="context">The database context</param>
    public WorkoutRepository(IApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc />
    public async Task<Workout?> GetWorkoutByIdAsync(Guid workoutId, CancellationToken cancellationToken = default)
    {
        return await _context.Workouts
            .FirstOrDefaultAsync(w => w.Id == workoutId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Workout>> GetWorkoutsByDateRangeAsync(Guid userId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        // Get active plan IDs for the user first
        var activePlanIds = await _context.TrainingPlans
            .Where(p => p.UserId == userId)
            .Select(p => p.Id)
            .ToListAsync(cancellationToken);

        if (!activePlanIds.Any())
            return Enumerable.Empty<Workout>();

        return await _context.Workouts
            .Include(w => w.TrainingWeek)
            .Where(w => activePlanIds.Contains(w.TrainingWeek!.PlanId) &&
                       w.ScheduledDate >= startDate &&
                       w.ScheduledDate <= endDate)
            .OrderBy(w => w.ScheduledDate)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Workout> UpdateWorkoutStatusAsync(Guid workoutId, CompletionStatus status, CancellationToken cancellationToken = default)
    {
        var workout = await GetWorkoutByIdAsync(workoutId, cancellationToken);
        
        if (workout == null)
            throw new InvalidOperationException($"Workout with ID {workoutId} not found");

        workout.CompletionStatus = status;
        
        if (status == CompletionStatus.Completed)
        {
            workout.CompletedAt = DateTime.UtcNow;
        }
        else
        {
            workout.CompletedAt = null;
        }

        _context.Workouts.Update(workout);
        await _context.SaveChangesAsync(cancellationToken);
        return workout;
    }

    /// <inheritdoc />
    public async Task<Workout?> GetTodaysWorkoutAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow.Date;

        // Get active plan IDs for the user first
        var activePlanIds = await _context.TrainingPlans
            .Where(p => p.UserId == userId)
            .Select(p => p.Id)
            .ToListAsync(cancellationToken);

        if (!activePlanIds.Any())
            return null;
        
        return await _context.Workouts
            .Include(w => w.TrainingWeek)
            .Where(w => activePlanIds.Contains(w.TrainingWeek!.PlanId) &&
                       w.ScheduledDate.Date == today)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Workout>> GetUpcomingWorkoutsAsync(Guid userId, int days, CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow.Date;
        var endDate = today.AddDays(days);

        // Get active plan IDs for the user first
        var activePlanIds = await _context.TrainingPlans
            .Where(p => p.UserId == userId)
            .Select(p => p.Id)
            .ToListAsync(cancellationToken);

        if (!activePlanIds.Any())
            return Enumerable.Empty<Workout>();

        return await _context.Workouts
            .Include(w => w.TrainingWeek)
            .Where(w => activePlanIds.Contains(w.TrainingWeek!.PlanId) &&
                       w.ScheduledDate.Date >= today &&
                       w.ScheduledDate.Date <= endDate)
            .OrderBy(w => w.ScheduledDate)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Workout?> GetWorkoutWithExercisesAsync(Guid workoutId, CancellationToken cancellationToken = default)
    {
        return await _context.Workouts
            .Include(w => w.TrainingWeek)
            .Include(w => w.WorkoutExercises.OrderBy(we => we.OrderInWorkout))
                .ThenInclude(we => we.Exercise)
            .FirstOrDefaultAsync(w => w.Id == workoutId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Workout> CreateWorkoutAsync(Workout workout, CancellationToken cancellationToken = default)
    {
        if (workout == null)
            throw new ArgumentNullException(nameof(workout));

        _context.Workouts.Add(workout);
        await _context.SaveChangesAsync(cancellationToken);
        return workout;
    }

    /// <inheritdoc />
    public async Task<Workout> UpdateWorkoutAsync(Workout workout, CancellationToken cancellationToken = default)
    {
        if (workout == null)
            throw new ArgumentNullException(nameof(workout));

        var existingWorkout = await GetWorkoutByIdAsync(workout.Id, cancellationToken);
        if (existingWorkout == null)
            throw new InvalidOperationException($"Workout with ID {workout.Id} not found");

        _context.Workouts.Update(workout);
        await _context.SaveChangesAsync(cancellationToken);
        return workout;
    }

    /// <inheritdoc />
    public async Task UpdateAsync(Workout workout, CancellationToken cancellationToken = default)
    {
        await UpdateWorkoutAsync(workout, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Workout>> GetWorkoutsByPlanAndDateRangeAsync(Guid planId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _context.Workouts
            .Include(w => w.TrainingWeek)
            .Where(w => w.TrainingWeek!.PlanId == planId &&
                       w.ScheduledDate >= startDate &&
                       w.ScheduledDate <= endDate)
            .OrderBy(w => w.ScheduledDate)
            .ToListAsync(cancellationToken);
    }
}
