using Microsoft.Extensions.Logging;
using FitnessApp.Application.Common.Interfaces;
using FitnessApp.Application.DTOs;
using FitnessApp.Domain.Entities;
using FitnessApp.Domain.Enums;
using System.Text.Json;

namespace FitnessApp.Application.Services;

/// <summary>
/// Service implementation for adaptive plan adjustments
/// Dynamically modifies training plans based on various triggers and user feedback
/// </summary>
public class PlanAdaptationService : IPlanAdaptationService
{
    private readonly ITrainingPlanRepository _planRepository;
    private readonly IWorkoutRepository _workoutRepository;
    private readonly IPlanAdaptationRepository _adaptationRepository;
    private readonly IExerciseRepository _exerciseRepository;
    private readonly ILogger<PlanAdaptationService> _logger;

    // Safety constants
    private const decimal MaxIntensityIncreasePercent = 0.20m; // 20% max week-over-week increase
    private const decimal IntensityIncreaseHarder = 0.125m; // 12.5% average for "harder"
    private const decimal IntensityDecreaseEasier = 0.175m; // 17.5% average for "easier"
    private const decimal MissedWorkoutReentryReduction = 0.25m; // 25% reduction for re-entry
    private const int MinDaysBetweenAdaptations = 7; // Minimum 1 week between adaptations

    public PlanAdaptationService(
        ITrainingPlanRepository planRepository,
        IWorkoutRepository workoutRepository,
        IPlanAdaptationRepository adaptationRepository,
        IExerciseRepository exerciseRepository,
        ILogger<PlanAdaptationService> logger)
    {
        _planRepository = planRepository;
        _workoutRepository = workoutRepository;
        _adaptationRepository = adaptationRepository;
        _exerciseRepository = exerciseRepository;
        _logger = logger;
    }

    public async Task<PlanAdaptationResultDto> AdaptForMissedWorkoutsAsync(
        Guid userId,
        IEnumerable<Guid> missedWorkoutIds,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Adapting plan for user {UserId} due to missed workouts", userId);

        var missedIds = missedWorkoutIds.ToList();
        if (!missedIds.Any())
        {
            throw new ArgumentException("No missed workout IDs provided", nameof(missedWorkoutIds));
        }

        // Get the user's active plan
        var plans = await _planRepository.GetActivePlansByUserIdAsync(userId, cancellationToken);
        var activePlan = plans.FirstOrDefault();
        if (activePlan == null)
        {
            throw new InvalidOperationException($"No active plan found for user {userId}");
        }

        // Check adaptation frequency limit
        await ValidateAdaptationFrequencyAsync(activePlan.Id, cancellationToken);

        // Load plan with full details
        var plan = await _planRepository.GetPlanWithDetailsAsync(activePlan.Id, cancellationToken);
        if (plan == null)
        {
            throw new InvalidOperationException($"Plan {activePlan.Id} not found");
        }

        // Count consecutive missed workouts
        int consecutiveMissed = missedIds.Count;
        
        // Get upcoming workouts (not completed, scheduled for future)
        var upcomingWorkouts = plan.TrainingWeeks
            .SelectMany(w => w.Workouts)
            .Where(w => w.CompletionStatus == CompletionStatus.NotStarted && 
                       w.ScheduledDate >= DateTime.UtcNow.Date)
            .OrderBy(w => w.ScheduledDate)
            .ToList();

        if (!upcomingWorkouts.Any())
        {
            _logger.LogWarning("No upcoming workouts to adapt for plan {PlanId}", plan.Id);
            return CreateFailureResult(plan.Id, AdaptationTrigger.MissedWorkouts, 
                "No upcoming workouts to modify");
        }

        int workoutsModified = 0;
        var changes = new List<string>();

        // Apply re-entry intensity reduction to next 1-2 weeks
        int reentryWeeks = consecutiveMissed >= 4 ? 2 : 1;
        var reentryWorkouts = upcomingWorkouts.Take(reentryWeeks * plan.TrainingDaysPerWeek).ToList();

        foreach (var workout in reentryWorkouts)
        {
            // Reduce intensity level
            if (workout.IntensityLevel > IntensityLevel.Low)
            {
                var oldIntensity = workout.IntensityLevel;
                workout.IntensityLevel = workout.IntensityLevel == IntensityLevel.Maximum 
                    ? IntensityLevel.High 
                    : workout.IntensityLevel == IntensityLevel.High 
                        ? IntensityLevel.Moderate 
                        : IntensityLevel.Low;
                
                changes.Add($"Reduced {workout.WorkoutName} intensity from {oldIntensity} to {workout.IntensityLevel}");
                workoutsModified++;
            }

            // Update description to explain the modification
            workout.Description = $"[Re-entry workout - intensity reduced for safe return to training] {workout.Description}";
        }

        // Save all workout updates
        foreach (var workout in reentryWorkouts)
        {
            await _workoutRepository.UpdateAsync(workout, cancellationToken);
        }

        // Create adaptation record
        var adaptation = new PlanAdaptation
        {
            PlanId = plan.Id,
            Trigger = AdaptationTrigger.MissedWorkouts,
            Type = AdaptationType.Recovery,
            Description = $"Reduced intensity for {reentryWeeks} week(s) after {consecutiveMissed} missed workouts",
            ChangesApplied = JsonSerializer.Serialize(changes),
            AppliedAt = DateTime.UtcNow
        };

        await _adaptationRepository.AddAsync(adaptation, cancellationToken);

        _logger.LogInformation("Successfully adapted plan {PlanId} for missed workouts. Modified {Count} workouts", 
            plan.Id, workoutsModified);

        return new PlanAdaptationResultDto
        {
            AdaptationId = adaptation.Id,
            PlanId = plan.Id,
            Trigger = AdaptationTrigger.MissedWorkouts,
            Type = AdaptationType.Recovery,
            Description = adaptation.Description,
            WorkoutsAffected = workoutsModified,
            AppliedAt = adaptation.AppliedAt,
            Success = true,
            Warnings = consecutiveMissed >= 7 
                ? "Long break detected. Consider consulting with Coach Tom about your training consistency." 
                : null
        };
    }

    public async Task<PlanAdaptationResultDto> AdaptForIntensityChangeAsync(
        Guid userId,
        IntensityAdjustmentDto adjustment,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Adapting plan intensity for user {UserId} - Direction: {Direction}", 
            userId, adjustment.Direction);

        // Get the user's active plan
        var plans = await _planRepository.GetActivePlansByUserIdAsync(userId, cancellationToken);
        var activePlan = plans.FirstOrDefault();
        if (activePlan == null)
        {
            throw new InvalidOperationException($"No active plan found for user {userId}");
        }

        // Check adaptation frequency limit
        await ValidateAdaptationFrequencyAsync(activePlan.Id, cancellationToken);

        // Load plan with full details
        var plan = await _planRepository.GetPlanWithDetailsAsync(activePlan.Id, cancellationToken);
        if (plan == null)
        {
            throw new InvalidOperationException($"Plan {activePlan.Id} not found");
        }

        // Get future workouts only (preserve past completions)
        var futureWorkouts = plan.TrainingWeeks
            .SelectMany(w => w.Workouts)
            .Where(w => w.ScheduledDate >= DateTime.UtcNow.Date && 
                       w.CompletionStatus == CompletionStatus.NotStarted)
            .ToList();

        if (!futureWorkouts.Any())
        {
            return CreateFailureResult(plan.Id, AdaptationTrigger.UserRequest, 
                "No future workouts to modify");
        }

        int workoutsModified = 0;
        var changes = new List<string>();

        foreach (var workout in futureWorkouts)
        {
            if (adjustment.Direction == IntensityDirection.Harder)
            {
                // Increase intensity if not already at maximum
                if (workout.IntensityLevel < IntensityLevel.Maximum)
                {
                    var oldIntensity = workout.IntensityLevel;
                    workout.IntensityLevel = workout.IntensityLevel == IntensityLevel.Low 
                        ? IntensityLevel.Moderate 
                        : workout.IntensityLevel == IntensityLevel.Moderate 
                            ? IntensityLevel.High 
                            : IntensityLevel.Maximum;
                    
                    changes.Add($"Increased {workout.WorkoutName} from {oldIntensity} to {workout.IntensityLevel}");
                    workoutsModified++;
                }
            }
            else // Easier
            {
                // Decrease intensity if not already at minimum
                if (workout.IntensityLevel > IntensityLevel.Low)
                {
                    var oldIntensity = workout.IntensityLevel;
                    workout.IntensityLevel = workout.IntensityLevel == IntensityLevel.Maximum 
                        ? IntensityLevel.High 
                        : workout.IntensityLevel == IntensityLevel.High 
                            ? IntensityLevel.Moderate 
                            : IntensityLevel.Low;
                    
                    changes.Add($"Decreased {workout.WorkoutName} from {oldIntensity} to {workout.IntensityLevel}");
                    workoutsModified++;
                }
            }

            // Update workout if modified
            if (workoutsModified > 0)
            {
                await _workoutRepository.UpdateAsync(workout, cancellationToken);
            }
        }

        // Validate safety - ensure no extreme intensity spikes
        await ValidateIntensityProgressionAsync(plan.Id, cancellationToken);

        // Create adaptation record
        var adaptation = new PlanAdaptation
        {
            PlanId = plan.Id,
            Trigger = AdaptationTrigger.UserRequest,
            Type = AdaptationType.Intensity,
            Description = $"Adjusted intensity {(adjustment.Direction == IntensityDirection.Harder ? "higher" : "lower")} for future workouts",
            ChangesApplied = JsonSerializer.Serialize(changes),
            AppliedAt = DateTime.UtcNow
        };

        await _adaptationRepository.AddAsync(adaptation, cancellationToken);

        _logger.LogInformation("Successfully adapted intensity for plan {PlanId}. Modified {Count} workouts", 
            plan.Id, workoutsModified);

        return new PlanAdaptationResultDto
        {
            AdaptationId = adaptation.Id,
            PlanId = plan.Id,
            Trigger = AdaptationTrigger.UserRequest,
            Type = AdaptationType.Intensity,
            Description = adaptation.Description,
            WorkoutsAffected = workoutsModified,
            AppliedAt = adaptation.AppliedAt,
            Success = true
        };
    }

    public async Task<PlanAdaptationResultDto> AdaptForScheduleChangeAsync(
        Guid userId,
        ScheduleAvailabilityDto newSchedule,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Adapting plan schedule for user {UserId}", userId);

        // Get the user's active plan
        var plans = await _planRepository.GetActivePlansByUserIdAsync(userId, cancellationToken);
        var activePlan = plans.FirstOrDefault();
        if (activePlan == null)
        {
            throw new InvalidOperationException($"No active plan found for user {userId}");
        }

        // Check adaptation frequency limit
        await ValidateAdaptationFrequencyAsync(activePlan.Id, cancellationToken);

        // Load plan with full details
        var plan = await _planRepository.GetPlanWithDetailsAsync(activePlan.Id, cancellationToken);
        if (plan == null)
        {
            throw new InvalidOperationException($"Plan {activePlan.Id} not found");
        }

        // Count available days in new schedule
        int availableDays = CountAvailableDays(newSchedule);
        if (availableDays < 2)
        {
            throw new InvalidOperationException("At least 2 training days per week are required");
        }

        // Get available day indices (0 = Sunday, 1 = Monday, etc.)
        var availableDayIndices = GetAvailableDayIndices(newSchedule);

        // Get future workouts
        var futureWorkouts = plan.TrainingWeeks
            .SelectMany(w => w.Workouts)
            .Where(w => w.ScheduledDate >= DateTime.UtcNow.Date && 
                       w.CompletionStatus == CompletionStatus.NotStarted)
            .OrderBy(w => w.ScheduledDate)
            .ToList();

        if (!futureWorkouts.Any())
        {
            return CreateFailureResult(plan.Id, AdaptationTrigger.ScheduleChange, 
                "No future workouts to reschedule");
        }

        // Redistribute workouts
        int workoutsModified = 0;
        var changes = new List<string>();
        var currentDate = DateTime.UtcNow.Date;
        int dayIndex = 0;

        // Prioritize key workouts
        var keyWorkouts = futureWorkouts.Where(w => w.IsKeyWorkout).ToList();
        var regularWorkouts = futureWorkouts.Where(w => !w.IsKeyWorkout).ToList();

        // Schedule key workouts first
        foreach (var workout in keyWorkouts)
        {
            var newDate = FindNextAvailableDay(currentDate, availableDayIndices, ref dayIndex);
            if (workout.ScheduledDate != newDate)
            {
                var oldDate = workout.ScheduledDate;
                workout.ScheduledDate = newDate;
                changes.Add($"Rescheduled {workout.WorkoutName} from {oldDate:yyyy-MM-dd} to {newDate:yyyy-MM-dd}");
                workoutsModified++;
            }
            await _workoutRepository.UpdateAsync(workout, cancellationToken);
        }

        // Schedule regular workouts
        foreach (var workout in regularWorkouts)
        {
            var newDate = FindNextAvailableDay(currentDate, availableDayIndices, ref dayIndex);
            if (workout.ScheduledDate != newDate)
            {
                var oldDate = workout.ScheduledDate;
                workout.ScheduledDate = newDate;
                changes.Add($"Rescheduled {workout.WorkoutName} from {oldDate:yyyy-MM-dd} to {newDate:yyyy-MM-dd}");
                workoutsModified++;
            }
            await _workoutRepository.UpdateAsync(workout, cancellationToken);
        }

        // Create adaptation record
        var adaptation = new PlanAdaptation
        {
            PlanId = plan.Id,
            Trigger = AdaptationTrigger.ScheduleChange,
            Type = AdaptationType.Schedule,
            Description = $"Redistributed workouts across {availableDays} available training days",
            ChangesApplied = JsonSerializer.Serialize(changes),
            AppliedAt = DateTime.UtcNow
        };

        await _adaptationRepository.AddAsync(adaptation, cancellationToken);

        _logger.LogInformation("Successfully adapted schedule for plan {PlanId}. Modified {Count} workouts", 
            plan.Id, workoutsModified);

        var warnings = availableDays < plan.TrainingDaysPerWeek 
            ? $"Reduced training frequency may impact goal timeline. Original plan: {plan.TrainingDaysPerWeek} days/week, New: {availableDays} days/week" 
            : null;

        return new PlanAdaptationResultDto
        {
            AdaptationId = adaptation.Id,
            PlanId = plan.Id,
            Trigger = AdaptationTrigger.ScheduleChange,
            Type = AdaptationType.Schedule,
            Description = adaptation.Description,
            WorkoutsAffected = workoutsModified,
            AppliedAt = adaptation.AppliedAt,
            Success = true,
            Warnings = warnings
        };
    }

    public async Task<PlanAdaptationResultDto> AdaptForInjuryAsync(
        Guid userId,
        Guid injuryId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Adapting plan for user {UserId} due to injury {InjuryId}", userId, injuryId);

        // Get the user's active plan
        var plans = await _planRepository.GetActivePlansByUserIdAsync(userId, cancellationToken);
        var activePlan = plans.FirstOrDefault();
        if (activePlan == null)
        {
            throw new InvalidOperationException($"No active plan found for user {userId}");
        }

        // Load plan with full details
        var plan = await _planRepository.GetPlanWithDetailsAsync(activePlan.Id, cancellationToken);
        if (plan == null)
        {
            throw new InvalidOperationException($"Plan {activePlan.Id} not found");
        }

        // Get future workouts
        var futureWorkouts = plan.TrainingWeeks
            .SelectMany(w => w.Workouts)
            .Where(w => w.ScheduledDate >= DateTime.UtcNow.Date && 
                       w.CompletionStatus == CompletionStatus.NotStarted)
            .ToList();

        if (!futureWorkouts.Any())
        {
            return CreateFailureResult(plan.Id, AdaptationTrigger.Injury, 
                "No future workouts to modify");
        }

        int workoutsModified = 0;
        var changes = new List<string>();

        // For each workout, check exercises and mark for review
        foreach (var workout in futureWorkouts)
        {
            // Add injury accommodation note to description
            var injuryNote = "[Adapted for injury - exercises may be modified or substituted]";
            if (!workout.Description?.Contains(injuryNote) ?? true)
            {
                workout.Description = $"{injuryNote} {workout.Description}";
                changes.Add($"Added injury accommodation note to {workout.WorkoutName}");
                workoutsModified++;
                await _workoutRepository.UpdateAsync(workout, cancellationToken);
            }
        }

        // Create adaptation record
        var adaptation = new PlanAdaptation
        {
            PlanId = plan.Id,
            Trigger = AdaptationTrigger.Injury,
            Type = AdaptationType.Injury,
            Description = $"Adapted plan to accommodate injury (ID: {injuryId}). Workouts marked for exercise modification.",
            ChangesApplied = JsonSerializer.Serialize(changes),
            AppliedAt = DateTime.UtcNow
        };

        await _adaptationRepository.AddAsync(adaptation, cancellationToken);

        _logger.LogInformation("Successfully adapted plan {PlanId} for injury. Modified {Count} workouts", 
            plan.Id, workoutsModified);

        return new PlanAdaptationResultDto
        {
            AdaptationId = adaptation.Id,
            PlanId = plan.Id,
            Trigger = AdaptationTrigger.Injury,
            Type = AdaptationType.Injury,
            Description = adaptation.Description,
            WorkoutsAffected = workoutsModified,
            AppliedAt = adaptation.AppliedAt,
            Success = true,
            Warnings = "Please review modified workouts and consult Coach Tom for exercise alternatives."
        };
    }

    public async Task<PlanAdaptationResultDto> AdaptForGoalTimelineChangeAsync(
        Guid userId,
        GoalTimelineChangeDto timelineChange,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Adapting plan timeline for user {UserId} - Goal: {GoalId}", 
            userId, timelineChange.GoalId);

        // Get the user's active plan
        var plans = await _planRepository.GetActivePlansByUserIdAsync(userId, cancellationToken);
        var activePlan = plans.FirstOrDefault();
        if (activePlan == null)
        {
            throw new InvalidOperationException($"No active plan found for user {userId}");
        }

        // Check adaptation frequency limit
        await ValidateAdaptationFrequencyAsync(activePlan.Id, cancellationToken);

        // Load plan with full details
        var plan = await _planRepository.GetPlanWithDetailsAsync(activePlan.Id, cancellationToken);
        if (plan == null)
        {
            throw new InvalidOperationException($"Plan {activePlan.Id} not found");
        }

        // Validate the new timeline
        var currentEndDate = plan.EndDate;
        var newEndDate = timelineChange.NewTargetDate;
        var isExtension = newEndDate > currentEndDate;
        var isCompression = newEndDate < currentEndDate;

        if (isCompression)
        {
            // Validate minimum safe duration
            var remainingWeeks = (newEndDate - DateTime.UtcNow.Date).TotalDays / 7;
            if (remainingWeeks < 4)
            {
                throw new InvalidOperationException(
                    "Timeline compression rejected: minimum 4 weeks required for safe training progression");
            }
        }

        var changes = new List<string>();
        
        // Update plan end date
        var oldEndDate = plan.EndDate;
        plan.EndDate = newEndDate;
        changes.Add($"Updated plan end date from {oldEndDate:yyyy-MM-dd} to {newEndDate:yyyy-MM-dd}");

        // Recalculate total weeks
        var newTotalWeeks = (int)Math.Ceiling((newEndDate - plan.StartDate).TotalDays / 7);
        plan.TotalWeeks = newTotalWeeks;
        changes.Add($"Adjusted total weeks from {(oldEndDate - plan.StartDate).TotalDays / 7:F0} to {newTotalWeeks}");

        await _planRepository.UpdatePlanAsync(plan, cancellationToken);

        // Create adaptation record
        var adaptation = new PlanAdaptation
        {
            PlanId = plan.Id,
            Trigger = AdaptationTrigger.TimelineChange,
            Type = AdaptationType.Timeline,
            Description = $"Timeline {(isExtension ? "extended" : "compressed")} - new end date: {newEndDate:yyyy-MM-dd}",
            ChangesApplied = JsonSerializer.Serialize(changes),
            AppliedAt = DateTime.UtcNow
        };

        await _adaptationRepository.AddAsync(adaptation, cancellationToken);

        _logger.LogInformation("Successfully adapted timeline for plan {PlanId}", plan.Id);

        var warnings = isCompression 
            ? "Timeline compressed - training intensity progression may be steeper. Monitor for overtraining signs." 
            : null;

        return new PlanAdaptationResultDto
        {
            AdaptationId = adaptation.Id,
            PlanId = plan.Id,
            Trigger = AdaptationTrigger.TimelineChange,
            Type = AdaptationType.Timeline,
            Description = adaptation.Description,
            WorkoutsAffected = 0, // Timeline change doesn't directly modify individual workouts
            AppliedAt = adaptation.AppliedAt,
            Success = true,
            Warnings = warnings
        };
    }

    public async Task<PlanAdaptationResultDto> AdaptForPerceivedDifficultyAsync(
        Guid userId,
        string feedbackPattern,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Adapting plan for user {UserId} based on difficulty feedback: {Pattern}", 
            userId, feedbackPattern);

        // Determine direction based on feedback pattern
        var direction = feedbackPattern.ToLowerInvariant().Contains("easy") 
            ? IntensityDirection.Harder 
            : IntensityDirection.Easier;

        // Use existing intensity adjustment logic
        var adjustment = new IntensityAdjustmentDto
        {
            Direction = direction,
            Reason = $"Based on perceived difficulty pattern: {feedbackPattern}"
        };

        var result = await AdaptForIntensityChangeAsync(userId, adjustment, cancellationToken);
        
        // Update trigger to reflect it came from perceived difficulty
        result.Trigger = AdaptationTrigger.PerceivedDifficulty;

        return result;
    }

    #region Helper Methods

    private async Task ValidateAdaptationFrequencyAsync(Guid planId, CancellationToken cancellationToken)
    {
        var recentAdaptation = await _adaptationRepository.GetMostRecentByPlanIdAsync(planId, cancellationToken);
        
        if (recentAdaptation != null)
        {
            var daysSinceLastAdaptation = (DateTime.UtcNow - recentAdaptation.AppliedAt).TotalDays;
            if (daysSinceLastAdaptation < MinDaysBetweenAdaptations)
            {
                throw new InvalidOperationException(
                    $"Adaptation frequency limit exceeded. Last adaptation was {daysSinceLastAdaptation:F0} days ago. Minimum {MinDaysBetweenAdaptations} days required.");
            }
        }
    }

    private async Task ValidateIntensityProgressionAsync(Guid planId, CancellationToken cancellationToken)
    {
        var plan = await _planRepository.GetPlanWithDetailsAsync(planId, cancellationToken);
        if (plan == null) return;

        // Get workouts in chronological order
        var workouts = plan.TrainingWeeks
            .SelectMany(w => w.Workouts)
            .OrderBy(w => w.ScheduledDate)
            .ToList();

        // Check for intensity spikes (simplified check)
        for (int i = 1; i < workouts.Count; i++)
        {
            var current = workouts[i];
            var previous = workouts[i - 1];
            
            // Flag if jumping more than 2 intensity levels
            var intensityJump = Math.Abs((int)current.IntensityLevel - (int)previous.IntensityLevel);
            if (intensityJump > 2)
            {
                _logger.LogWarning("Large intensity jump detected between {Date1} and {Date2}", 
                    previous.ScheduledDate, current.ScheduledDate);
            }
        }
    }

    private PlanAdaptationResultDto CreateFailureResult(
        Guid planId, 
        AdaptationTrigger trigger, 
        string reason)
    {
        return new PlanAdaptationResultDto
        {
            AdaptationId = Guid.Empty,
            PlanId = planId,
            Trigger = trigger,
            Type = AdaptationType.Intensity,
            Description = $"Adaptation not applied: {reason}",
            WorkoutsAffected = 0,
            AppliedAt = DateTime.UtcNow,
            Success = false,
            Warnings = reason
        };
    }

    private int CountAvailableDays(ScheduleAvailabilityDto schedule)
    {
        int count = 0;
        if (schedule.Monday) count++;
        if (schedule.Tuesday) count++;
        if (schedule.Wednesday) count++;
        if (schedule.Thursday) count++;
        if (schedule.Friday) count++;
        if (schedule.Saturday) count++;
        if (schedule.Sunday) count++;
        return count;
    }

    private List<int> GetAvailableDayIndices(ScheduleAvailabilityDto schedule)
    {
        var indices = new List<int>();
        if (schedule.Sunday) indices.Add(0);
        if (schedule.Monday) indices.Add(1);
        if (schedule.Tuesday) indices.Add(2);
        if (schedule.Wednesday) indices.Add(3);
        if (schedule.Thursday) indices.Add(4);
        if (schedule.Friday) indices.Add(5);
        if (schedule.Saturday) indices.Add(6);
        return indices;
    }

    private DateTime FindNextAvailableDay(DateTime startDate, List<int> availableDayIndices, ref int currentIndex)
    {
        var date = startDate;
        while (true)
        {
            int dayOfWeek = (int)date.DayOfWeek;
            if (availableDayIndices.Contains(dayOfWeek))
            {
                currentIndex++;
                return date;
            }
            date = date.AddDays(1);
            
            // Safety check - don't go more than 14 days out
            if ((date - startDate).TotalDays > 14)
            {
                break;
            }
        }
        return date;
    }

    #endregion
}
