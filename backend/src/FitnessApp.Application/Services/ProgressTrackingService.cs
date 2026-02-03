using Microsoft.Extensions.Logging;
using FitnessApp.Application.Common.Interfaces;
using FitnessApp.Application.DTOs;
using FitnessApp.Application.Mapping;
using FitnessApp.Domain.Entities;
using FitnessApp.Domain.Enums;

namespace FitnessApp.Application.Services;

/// <summary>
/// Service implementation for progress tracking operations
/// </summary>
public class ProgressTrackingService : IProgressTrackingService
{
    private readonly IWorkoutRepository _workoutRepository;
    private readonly ITrainingPlanRepository _planRepository;
    private readonly ICompletionHistoryRepository _completionHistoryRepository;
    private readonly IUserStreakRepository _userStreakRepository;
    private readonly ILogger<ProgressTrackingService> _logger;

    private static readonly int[] StreakMilestones = { 7, 14, 30, 60, 90, 180, 365 };

    public ProgressTrackingService(
        IWorkoutRepository workoutRepository,
        ITrainingPlanRepository planRepository,
        ICompletionHistoryRepository completionHistoryRepository,
        IUserStreakRepository userStreakRepository,
        ILogger<ProgressTrackingService> logger)
    {
        _workoutRepository = workoutRepository;
        _planRepository = planRepository;
        _completionHistoryRepository = completionHistoryRepository;
        _userStreakRepository = userStreakRepository;
        _logger = logger;
    }

    public async Task<WorkoutDetailDto> MarkWorkoutCompleteAsync(
        Guid workoutId, 
        Guid userId, 
        WorkoutCompletionDto completionDto, 
        CancellationToken cancellationToken = default)
    {
        // Validate completion timestamp is not in the future
        if (completionDto.CompletedAt > DateTime.UtcNow)
        {
            throw new InvalidOperationException("Cannot complete a workout in the future");
        }

        // Get workout with details
        var workout = await _workoutRepository.GetWorkoutWithExercisesAsync(workoutId, cancellationToken);
        if (workout == null)
        {
            throw new KeyNotFoundException($"Workout with ID {workoutId} not found");
        }

        // Verify ownership by checking the training plan
        var plan = await _planRepository.GetByIdAsync(workout.TrainingWeek!.PlanId, cancellationToken);
        if (plan == null || plan.UserId != userId)
        {
            throw new UnauthorizedAccessException("User does not own this workout");
        }

        // Check if already completed
        if (workout.CompletionStatus == CompletionStatus.Completed)
        {
            throw new InvalidOperationException("Workout is already marked as completed");
        }

        // Prevent completing future workouts based on scheduled date
        if (workout.ScheduledDate.Date > DateTime.UtcNow.Date)
        {
            throw new InvalidOperationException("Cannot complete a workout scheduled for the future");
        }

        // Update workout status
        workout.CompletionStatus = CompletionStatus.Completed;
        workout.CompletedAt = completionDto.CompletedAt;
        await _workoutRepository.UpdateAsync(workout, cancellationToken);

        // Create completion history record
        var completionHistory = new CompletionHistory
        {
            UserId = userId,
            WorkoutId = workoutId,
            CompletedAt = completionDto.CompletedAt,
            Duration = completionDto.Duration,
            Notes = completionDto.Notes
        };
        await _completionHistoryRepository.AddAsync(completionHistory, cancellationToken);

        // Update streak information
        await UpdateStreakAsync(userId, completionDto.CompletedAt, cancellationToken);

        // Recalculate plan progress
        await RecalculatePlanProgressAsync(plan.Id, cancellationToken);

        _logger.LogInformation("Workout {WorkoutId} marked as complete for user {UserId}", workoutId, userId);

        return workout.ToDetailDto();
    }

    public async Task<WorkoutDetailDto> UndoWorkoutCompletionAsync(
        Guid workoutId, 
        Guid userId, 
        CancellationToken cancellationToken = default)
    {
        // Get workout with details
        var workout = await _workoutRepository.GetWorkoutWithExercisesAsync(workoutId, cancellationToken);
        if (workout == null)
        {
            throw new KeyNotFoundException($"Workout with ID {workoutId} not found");
        }

        // Verify ownership
        var plan = await _planRepository.GetByIdAsync(workout.TrainingWeek!.PlanId, cancellationToken);
        if (plan == null || plan.UserId != userId)
        {
            throw new UnauthorizedAccessException("User does not own this workout");
        }

        // Check if it was completed
        if (workout.CompletionStatus != CompletionStatus.Completed)
        {
            throw new InvalidOperationException("Workout is not marked as completed");
        }

        // Update workout status
        workout.CompletionStatus = CompletionStatus.NotStarted;
        workout.CompletedAt = null;
        await _workoutRepository.UpdateAsync(workout, cancellationToken);

        // Remove completion history record
        var completionHistory = await _completionHistoryRepository.GetByWorkoutIdAsync(workoutId, cancellationToken);
        if (completionHistory != null)
        {
            await _completionHistoryRepository.DeleteAsync(completionHistory, cancellationToken);
        }

        // Recalculate streak based on all remaining completions
        await RecalculateStreakAsync(userId, cancellationToken);

        // Recalculate plan progress
        await RecalculatePlanProgressAsync(plan.Id, cancellationToken);

        _logger.LogInformation("Workout {WorkoutId} completion undone for user {UserId}", workoutId, userId);

        return workout.ToDetailDto();
    }

    public async Task<CompletionStatsDto> GetCompletionStatsAsync(
        Guid userId, 
        DateTime startDate, 
        DateTime endDate, 
        CancellationToken cancellationToken = default)
    {
        // Get all workouts scheduled in the period for the user
        var userPlans = await _planRepository.GetActivePlansByUserIdAsync(userId, cancellationToken);
        var allWorkouts = new List<Workout>();
        
        foreach (var plan in userPlans)
        {
            var planWorkouts = await _workoutRepository.GetWorkoutsByPlanAndDateRangeAsync(
                plan.Id, 
                startDate, 
                endDate, 
                cancellationToken);
            allWorkouts.AddRange(planWorkouts);
        }

        var totalScheduled = allWorkouts.Count;
        var completedCount = allWorkouts.Count(w => w.CompletionStatus == CompletionStatus.Completed);
        var completionPercentage = totalScheduled > 0 ? (decimal)completedCount / totalScheduled * 100 : 0;

        return new CompletionStatsDto
        {
            CompletedCount = completedCount,
            TotalScheduled = totalScheduled,
            CompletionPercentage = Math.Round(completionPercentage, 2),
            PeriodStart = startDate,
            PeriodEnd = endDate
        };
    }

    public async Task<OverallStatsDto> GetOverallStatsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        // Get all completion history
        var allCompletions = await _completionHistoryRepository.GetByUserIdAsync(userId, cancellationToken);
        var completionsList = allCompletions.ToList();

        // Get distinct training days
        var distinctDates = await _completionHistoryRepository.GetDistinctCompletionDatesAsync(userId, cancellationToken);
        var datesList = distinctDates.ToList();

        // Get stats for current week and month
        var now = DateTime.UtcNow;
        var weekStart = now.Date.AddDays(-(int)now.DayOfWeek);
        var monthStart = new DateTime(now.Year, now.Month, 1);

        var thisWeekCompletions = completionsList.Count(c => c.CompletedAt >= weekStart);
        var thisMonthCompletions = completionsList.Count(c => c.CompletedAt >= monthStart);

        // Calculate overall plan completion
        var userPlans = await _planRepository.GetActivePlansByUserIdAsync(userId, cancellationToken);
        var totalWorkouts = 0;
        var completedWorkouts = 0;

        foreach (var plan in userPlans)
        {
            var planWithDetails = await _planRepository.GetPlanWithDetailsAsync(plan.Id, cancellationToken);
            if (planWithDetails != null)
            {
                foreach (var week in planWithDetails.TrainingWeeks)
                {
                    totalWorkouts += week.Workouts.Count;
                    completedWorkouts += week.Workouts.Count(w => w.CompletionStatus == CompletionStatus.Completed);
                }
            }
        }

        var overallPercentage = totalWorkouts > 0 ? (decimal)completedWorkouts / totalWorkouts * 100 : 0;

        // Calculate average weekly completion rate
        var weeksWithData = datesList.Any() 
            ? (int)Math.Ceiling((DateTime.UtcNow - datesList.Min()).TotalDays / 7.0)
            : 0;
        var avgWeeklyRate = weeksWithData > 0 ? (decimal)completionsList.Count / weeksWithData : 0;

        return new OverallStatsDto
        {
            TotalTrainingDays = datesList.Count,
            TotalWorkoutsCompleted = completionsList.Count,
            OverallPlanCompletionPercentage = Math.Round(overallPercentage, 2),
            AverageWeeklyCompletionRate = Math.Round(avgWeeklyRate, 2),
            WorkoutsCompletedThisWeek = thisWeekCompletions,
            WorkoutsCompletedThisMonth = thisMonthCompletions,
            FirstWorkoutDate = completionsList.Any() ? completionsList.Min(c => c.CompletedAt) : null,
            LastWorkoutDate = completionsList.Any() ? completionsList.Max(c => c.CompletedAt) : null
        };
    }

    public async Task<StreakInfoDto> GetStreakInfoAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var userStreak = await _userStreakRepository.GetByUserIdAsync(userId, cancellationToken);
        
        if (userStreak == null)
        {
            // Return empty streak info if no streak exists
            return new StreakInfoDto
            {
                CurrentStreak = 0,
                LongestStreak = 0,
                CurrentWeeklyStreak = 0,
                LongestWeeklyStreak = 0,
                LastWorkoutDate = null,
                DaysUntilNextMilestone = 7,
                NextMilestone = 7
            };
        }

        // Calculate next milestone
        var nextMilestone = StreakMilestones.FirstOrDefault(m => m > userStreak.CurrentStreak);
        if (nextMilestone == 0)
        {
            nextMilestone = userStreak.CurrentStreak + 100; // If beyond all milestones, next is +100
        }

        return new StreakInfoDto
        {
            CurrentStreak = userStreak.CurrentStreak,
            LongestStreak = userStreak.LongestStreak,
            CurrentWeeklyStreak = userStreak.CurrentWeeklyStreak,
            LongestWeeklyStreak = userStreak.LongestWeeklyStreak,
            LastWorkoutDate = userStreak.LastWorkoutDate,
            DaysUntilNextMilestone = nextMilestone - userStreak.CurrentStreak,
            NextMilestone = nextMilestone
        };
    }

    public async Task<IEnumerable<CompletionHistoryDto>> GetCompletionHistoryAsync(
        Guid userId, 
        DateTime startDate, 
        DateTime endDate, 
        CancellationToken cancellationToken = default)
    {
        var completions = await _completionHistoryRepository.GetByUserAndDateRangeAsync(
            userId, 
            startDate, 
            endDate, 
            cancellationToken);

        return completions.Select(c => new CompletionHistoryDto
        {
            CompletedAt = c.CompletedAt,
            WorkoutId = c.WorkoutId,
            WorkoutName = c.Workout?.WorkoutName ?? "Unknown",
            Discipline = c.Workout?.Discipline.ToString() ?? "Unknown",
            Duration = c.Duration,
            Notes = c.Notes
        });
    }

    private async Task UpdateStreakAsync(Guid userId, DateTime completionDate, CancellationToken cancellationToken)
    {
        var userStreak = await _userStreakRepository.GetOrCreateAsync(userId, cancellationToken);
        var completionDateOnly = completionDate.Date;

        // Check if this is the first workout or if streak should continue
        if (userStreak.LastWorkoutDate == null)
        {
            // First workout
            userStreak.CurrentStreak = 1;
            userStreak.LongestStreak = 1;
            userStreak.LastWorkoutDate = completionDateOnly;
        }
        else
        {
            var lastWorkoutDateOnly = userStreak.LastWorkoutDate.Value.Date;
            var daysSinceLastWorkout = (completionDateOnly - lastWorkoutDateOnly).Days;

            if (daysSinceLastWorkout == 0)
            {
                // Same day - multiple workouts allowed, don't change streak
                return;
            }
            else if (daysSinceLastWorkout == 1)
            {
                // Consecutive day - increment streak
                userStreak.CurrentStreak++;
                userStreak.LastWorkoutDate = completionDateOnly;
                
                // Update longest streak if necessary
                if (userStreak.CurrentStreak > userStreak.LongestStreak)
                {
                    userStreak.LongestStreak = userStreak.CurrentStreak;
                }
            }
            else
            {
                // Streak broken - reset to 1
                userStreak.CurrentStreak = 1;
                userStreak.LastWorkoutDate = completionDateOnly;
            }
        }

        // Update weekly streak
        await UpdateWeeklyStreakAsync(userStreak, userId, cancellationToken);

        await _userStreakRepository.UpdateAsync(userStreak, cancellationToken);
    }

    private async Task UpdateWeeklyStreakAsync(UserStreak userStreak, Guid userId, CancellationToken cancellationToken)
    {
        // Get all distinct completion dates
        var allDates = await _completionHistoryRepository.GetDistinctCompletionDatesAsync(userId, cancellationToken);
        var datesList = allDates.OrderByDescending(d => d).ToList();

        if (!datesList.Any())
        {
            userStreak.CurrentWeeklyStreak = 0;
            return;
        }

        // Calculate weekly streak (minimum 3 workouts per week)
        const int minWorkoutsPerWeek = 3;
        var weeklyStreak = 0;
        var currentWeekStart = DateTime.UtcNow.Date.AddDays(-(int)DateTime.UtcNow.DayOfWeek);
        
        while (true)
        {
            var weekEnd = currentWeekStart.AddDays(7);
            var workoutsInWeek = datesList.Count(d => d >= currentWeekStart && d < weekEnd);
            
            if (workoutsInWeek >= minWorkoutsPerWeek)
            {
                weeklyStreak++;
                currentWeekStart = currentWeekStart.AddDays(-7);
            }
            else
            {
                break;
            }

            // Safety check - don't go back more than 2 years
            if (currentWeekStart < DateTime.UtcNow.AddYears(-2))
            {
                break;
            }
        }

        userStreak.CurrentWeeklyStreak = weeklyStreak;
        if (weeklyStreak > userStreak.LongestWeeklyStreak)
        {
            userStreak.LongestWeeklyStreak = weeklyStreak;
        }
    }

    private async Task RecalculateStreakAsync(Guid userId, CancellationToken cancellationToken)
    {
        var userStreak = await _userStreakRepository.GetOrCreateAsync(userId, cancellationToken);
        
        // Get all completion dates in descending order
        var allDates = await _completionHistoryRepository.GetDistinctCompletionDatesAsync(userId, cancellationToken);
        var datesList = allDates.OrderByDescending(d => d).ToList();

        if (!datesList.Any())
        {
            // No completions - reset streak
            userStreak.CurrentStreak = 0;
            userStreak.LastWorkoutDate = null;
            userStreak.CurrentWeeklyStreak = 0;
            await _userStreakRepository.UpdateAsync(userStreak, cancellationToken);
            return;
        }

        // Calculate current streak from most recent date backwards
        var currentStreak = 1;
        userStreak.LastWorkoutDate = datesList[0];

        for (int i = 1; i < datesList.Count; i++)
        {
            var daysDiff = (datesList[i - 1] - datesList[i]).Days;
            if (daysDiff == 1)
            {
                currentStreak++;
            }
            else
            {
                break;
            }
        }

        userStreak.CurrentStreak = currentStreak;

        // Recalculate longest streak
        var longestStreak = 1;
        var tempStreak = 1;
        for (int i = 1; i < datesList.Count; i++)
        {
            var daysDiff = (datesList[i - 1] - datesList[i]).Days;
            if (daysDiff == 1)
            {
                tempStreak++;
                if (tempStreak > longestStreak)
                {
                    longestStreak = tempStreak;
                }
            }
            else
            {
                tempStreak = 1;
            }
        }

        userStreak.LongestStreak = Math.Max(longestStreak, userStreak.LongestStreak);

        // Update weekly streak
        await UpdateWeeklyStreakAsync(userStreak, userId, cancellationToken);

        await _userStreakRepository.UpdateAsync(userStreak, cancellationToken);
    }

    private async Task RecalculatePlanProgressAsync(Guid planId, CancellationToken cancellationToken)
    {
        var plan = await _planRepository.GetPlanWithDetailsAsync(planId, cancellationToken);
        if (plan == null)
        {
            return;
        }

        // Count total workouts and completed workouts
        var totalWorkouts = 0;
        var completedWorkouts = 0;

        foreach (var week in plan.TrainingWeeks)
        {
            totalWorkouts += week.Workouts.Count;
            completedWorkouts += week.Workouts.Count(w => w.CompletionStatus == CompletionStatus.Completed);
        }

        // Update plan metadata with completion percentage
        if (plan.PlanMetadata != null && totalWorkouts > 0)
        {
            var completionPercentage = (decimal)completedWorkouts / totalWorkouts * 100;
            // Store in notes or additional field if needed
            _logger.LogInformation("Plan {PlanId} completion: {Completed}/{Total} ({Percentage}%)", 
                planId, completedWorkouts, totalWorkouts, Math.Round(completionPercentage, 2));
        }
    }
}
