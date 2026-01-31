using FitnessApp.Application.Common.Interfaces;
using FitnessApp.Domain.Entities;
using FitnessApp.Domain.Enums;
using FitnessApp.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace FitnessApp.Application.Services;

/// <summary>
/// Service for generating personalized training plans based on user profiles
/// Implements evidence-based training science principles including periodization and progressive overload
/// </summary>
public class TrainingPlanGenerationService : ITrainingPlanGenerationService
{
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly ITrainingPlanRepository _trainingPlanRepository;
    private readonly IExerciseRepository _exerciseRepository;
    private readonly ILogger<TrainingPlanGenerationService> _logger;

    // Algorithm version for tracking changes
    private const string AlgorithmVersion = "1.0.0";

    // Training science constants
    private const int MinPlanDurationWeeks = 4;
    private const int MaxPlanDurationWeeks = 52;
    private const int DefaultPlanDurationWeeks = 12;
    private const decimal ProgressiveOverloadIncreasePercent = 0.10m; // 10% weekly increase
    private const int DeloadWeekFrequency = 4; // Deload every 4th week

    public TrainingPlanGenerationService(
        IUserProfileRepository userProfileRepository,
        ITrainingPlanRepository trainingPlanRepository,
        IExerciseRepository exerciseRepository,
        ILogger<TrainingPlanGenerationService> logger)
    {
        _userProfileRepository = userProfileRepository;
        _trainingPlanRepository = trainingPlanRepository;
        _exerciseRepository = exerciseRepository;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<TrainingPlan> GeneratePlanAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting plan generation for user {UserId}", userId);

        // Phase 1: Profile Analysis
        var userProfile = await AnalyzeUserProfileAsync(userId, cancellationToken);
        
        if (!await ValidatePlanParametersAsync(userProfile, cancellationToken))
        {
            throw new InvalidOperationException("User profile does not have sufficient information for plan generation");
        }

        // Determine plan parameters
        var planDuration = DeterminePlanDuration(userProfile);
        var trainingDaysPerWeek = DetermineTrainingDaysPerWeek(userProfile);
        var injuryTypes = GetActiveInjuryTypes(userProfile);
        var primaryGoal = GetPrimaryGoal(userProfile);

        _logger.LogInformation("Plan parameters: Duration={Duration} weeks, DaysPerWeek={DaysPerWeek}", 
            planDuration, trainingDaysPerWeek);

        // Phase 2: Periodization Design
        var phases = DesignPeriodization(planDuration);

        // Create the training plan entity
        var trainingPlan = new TrainingPlan
        {
            UserId = userId,
            PlanName = GeneratePlanName(userProfile, primaryGoal),
            StartDate = DateTime.UtcNow.Date,
            EndDate = DateTime.UtcNow.Date.AddDays(planDuration * 7),
            TotalWeeks = planDuration,
            TrainingDaysPerWeek = trainingDaysPerWeek,
            PrimaryGoalId = primaryGoal?.Id,
            Status = PlanStatus.Active,
            CurrentWeek = 1,
            IsDeleted = false,
            TrainingWeeks = new List<TrainingWeek>(),
            PlanMetadata = new PlanMetadata
            {
                PlanId = Guid.NewGuid(), // Will be set properly when saved
                AlgorithmVersion = AlgorithmVersion,
                GenerationParameters = JsonSerializer.Serialize(new
                {
                    PlanDuration = planDuration,
                    TrainingDaysPerWeek = trainingDaysPerWeek,
                    InjuryTypes = injuryTypes.Select(i => i.ToString()).ToList(),
                    PrimaryGoalType = primaryGoal?.GoalType.ToString()
                })
            }
        };

        // Phase 3-6: Generate weeks with workouts
        await GenerateTrainingWeeksAsync(trainingPlan, userProfile, phases, injuryTypes, cancellationToken);

        // Phase 7: Validation
        ValidatePlanCoherence(trainingPlan);

        // Persist to database
        var savedPlan = await _trainingPlanRepository.CreatePlanAsync(trainingPlan, cancellationToken);
        
        _logger.LogInformation("Successfully generated plan {PlanId} for user {UserId}", savedPlan.Id, userId);

        return savedPlan;
    }

    /// <inheritdoc/>
    public async Task<TrainingPlan> RegeneratePlanAsync(Guid userId, object? modifications = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Regenerating plan for user {UserId}", userId);

        // Archive existing active plan
        var existingPlan = await _trainingPlanRepository.GetActivePlanByUserIdAsync(userId, cancellationToken);
        if (existingPlan != null)
        {
            existingPlan.Status = PlanStatus.Abandoned;
            await _trainingPlanRepository.UpdatePlanAsync(existingPlan, cancellationToken);
        }

        // Generate new plan
        return await GeneratePlanAsync(userId, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<bool> ValidatePlanParametersAsync(UserProfile userProfile, CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask; // Async signature for future expansion

        // Check required fields
        if (userProfile.ScheduleAvailability == null)
        {
            _logger.LogWarning("User {UserId} has no schedule availability", userProfile.UserId);
            return false;
        }

        if (!userProfile.ScheduleAvailability.IsValid())
        {
            _logger.LogWarning("User {UserId} has invalid schedule availability", userProfile.UserId);
            return false;
        }

        if (userProfile.TrainingGoals == null || !userProfile.TrainingGoals.Any())
        {
            _logger.LogWarning("User {UserId} has no training goals", userProfile.UserId);
            return false;
        }

        return true;
    }

    #region Phase 1: Profile Analysis

    private async Task<UserProfile> AnalyzeUserProfileAsync(Guid userId, CancellationToken cancellationToken)
    {
        var profile = await _userProfileRepository.GetCompleteProfileAsync(userId, cancellationToken);
        
        if (profile == null)
        {
            throw new InvalidOperationException($"User profile not found for user {userId}");
        }

        return profile;
    }

    private int DeterminePlanDuration(UserProfile userProfile)
    {
        var primaryGoal = GetPrimaryGoal(userProfile);
        
        if (primaryGoal?.TargetDate != null)
        {
            // Calculate weeks until target date
            var weeksUntilTarget = (int)Math.Ceiling((primaryGoal.TargetDate.Value - DateTime.UtcNow.Date).TotalDays / 7);
            return Math.Clamp(weeksUntilTarget, MinPlanDurationWeeks, MaxPlanDurationWeeks);
        }

        // Default duration for open-ended goals
        return DefaultPlanDurationWeeks;
    }

    private int DetermineTrainingDaysPerWeek(UserProfile userProfile)
    {
        var schedule = userProfile.ScheduleAvailability!;
        
        // Use the maximum sessions per week, but cap at available days
        return Math.Min(schedule.MaximumSessionsPerWeek, schedule.AvailableDaysCount);
    }

    private List<InjuryType> GetActiveInjuryTypes(UserProfile userProfile)
    {
        return userProfile.InjuryLimitations?
            .Where(i => i.Status == InjuryStatus.Active)
            .Select(i => i.InjuryType)
            .ToList() ?? new List<InjuryType>();
    }

    private TrainingGoal? GetPrimaryGoal(UserProfile userProfile)
    {
        return userProfile.TrainingGoals?
            .Where(g => g.Status == GoalStatus.Active)
            .OrderBy(g => g.Priority)
            .FirstOrDefault();
    }

    #endregion

    #region Phase 2: Periodization Design

    /// <summary>
    /// Designs the periodization phases based on plan duration
    /// Implements evidence-based periodization principles:
    /// - Short plans (4-8 weeks): Foundation → Build → Peak
    /// - Medium plans (8-16 weeks): Foundation → Build → Intensity → Peak → Taper
    /// - Long plans (16+ weeks): Multiple build cycles with recovery weeks
    /// </summary>
    private List<PeriodizationPhase> DesignPeriodization(int totalWeeks)
    {
        var phases = new List<PeriodizationPhase>();

        if (totalWeeks <= 8)
        {
            // Short plan: Foundation → Build → Peak
            phases.Add(new PeriodizationPhase(TrainingPhase.Foundation, 1, totalWeeks / 3));
            phases.Add(new PeriodizationPhase(TrainingPhase.Build, phases.Last().EndWeek + 1, totalWeeks * 2 / 3));
            phases.Add(new PeriodizationPhase(TrainingPhase.Peak, phases.Last().EndWeek + 1, totalWeeks));
        }
        else if (totalWeeks <= 16)
        {
            // Medium plan: Foundation → Build → Intensity → Peak → Taper
            phases.Add(new PeriodizationPhase(TrainingPhase.Foundation, 1, totalWeeks / 5));
            phases.Add(new PeriodizationPhase(TrainingPhase.Build, phases.Last().EndWeek + 1, totalWeeks * 2 / 5));
            phases.Add(new PeriodizationPhase(TrainingPhase.Intensity, phases.Last().EndWeek + 1, totalWeeks * 3 / 5));
            phases.Add(new PeriodizationPhase(TrainingPhase.Peak, phases.Last().EndWeek + 1, totalWeeks - 2));
            phases.Add(new PeriodizationPhase(TrainingPhase.Taper, phases.Last().EndWeek + 1, totalWeeks));
        }
        else
        {
            // Long plan: Multiple build cycles
            var cycleLength = 4;
            var currentWeek = 1;

            phases.Add(new PeriodizationPhase(TrainingPhase.Foundation, 1, Math.Min(4, totalWeeks / 4)));
            currentWeek = phases.Last().EndWeek + 1;

            while (currentWeek + cycleLength + 2 <= totalWeeks)
            {
                phases.Add(new PeriodizationPhase(TrainingPhase.Build, currentWeek, currentWeek + cycleLength - 1));
                currentWeek += cycleLength;
            }

            if (currentWeek + 2 <= totalWeeks)
            {
                phases.Add(new PeriodizationPhase(TrainingPhase.Peak, currentWeek, totalWeeks - 1));
                phases.Add(new PeriodizationPhase(TrainingPhase.Taper, totalWeeks, totalWeeks));
            }
            else
            {
                phases.Add(new PeriodizationPhase(TrainingPhase.Peak, currentWeek, totalWeeks));
            }
        }

        return phases;
    }

    #endregion

    #region Phase 3-6: Weekly Structure and Workout Assignment

    private async Task GenerateTrainingWeeksAsync(
        TrainingPlan plan,
        UserProfile userProfile,
        List<PeriodizationPhase> phases,
        List<InjuryType> injuryTypes,
        CancellationToken cancellationToken)
    {
        var availableDays = GetAvailableTrainingDays(userProfile.ScheduleAvailability!);
        var disciplinePriorities = GetDisciplinePriorities(userProfile);

        for (int weekNum = 1; weekNum <= plan.TotalWeeks; weekNum++)
        {
            var phase = GetPhaseForWeek(phases, weekNum);
            var isDeloadWeek = weekNum % DeloadWeekFrequency == 0 && weekNum != plan.TotalWeeks;
            
            var week = new TrainingWeek
            {
                PlanId = plan.Id,
                WeekNumber = weekNum,
                Phase = phase.Phase,
                IntensityLevel = DetermineIntensityLevel(phase.Phase, isDeloadWeek),
                FocusArea = DetermineFocusArea(phase.Phase, userProfile),
                StartDate = plan.StartDate.AddDays((weekNum - 1) * 7),
                EndDate = plan.StartDate.AddDays(weekNum * 7 - 1),
                Workouts = new List<Workout>()
            };

            // Generate workouts for this week
            await GenerateWorkoutsForWeekAsync(
                week, 
                userProfile, 
                plan.TrainingDaysPerWeek, 
                availableDays,
                disciplinePriorities,
                injuryTypes,
                weekNum,
                isDeloadWeek,
                cancellationToken);

            // Calculate weekly volume
            week.WeeklyVolume = week.Workouts.Sum(w => w.EstimatedDuration ?? 0);

            plan.TrainingWeeks.Add(week);
        }
    }

    private async Task GenerateWorkoutsForWeekAsync(
        TrainingWeek week,
        UserProfile userProfile,
        int workoutsPerWeek,
        List<WorkoutDay> availableDays,
        Dictionary<Discipline, int> disciplinePriorities,
        List<InjuryType> injuryTypes,
        int weekNumber,
        bool isDeloadWeek,
        CancellationToken cancellationToken)
    {
        // Allocate workouts to available days
        var selectedDays = SelectTrainingDays(availableDays, workoutsPerWeek);
        
        // Determine discipline distribution for the week
        var disciplineAllocation = AllocateDisciplines(disciplinePriorities, workoutsPerWeek);

        for (int i = 0; i < selectedDays.Count; i++)
        {
            var discipline = disciplineAllocation[i];
            var sessionType = DetermineSessionType(week.Phase, discipline, i, selectedDays.Count);
            var intensityLevel = isDeloadWeek ? IntensityLevel.Low : week.IntensityLevel;

            var workout = new Workout
            {
                WeekId = week.Id,
                DayOfWeek = selectedDays[i],
                ScheduledDate = week.StartDate.AddDays((int)selectedDays[i]),
                Discipline = discipline,
                SessionType = sessionType,
                WorkoutName = GenerateWorkoutName(discipline, sessionType, week.Phase),
                Description = GenerateWorkoutDescription(discipline, sessionType, week.Phase),
                EstimatedDuration = EstimateWorkoutDuration(sessionType, intensityLevel),
                IntensityLevel = intensityLevel,
                IsKeyWorkout = DetermineIfKeyWorkout(week.Phase, i, selectedDays.Count),
                CompletionStatus = CompletionStatus.NotStarted,
                WorkoutExercises = new List<WorkoutExercise>()
            };

            // Select and assign exercises
            await AssignExercisesToWorkoutAsync(
                workout, 
                userProfile, 
                injuryTypes, 
                weekNumber,
                cancellationToken);

            week.Workouts.Add(workout);
        }
    }

    private async Task AssignExercisesToWorkoutAsync(
        Workout workout,
        UserProfile userProfile,
        List<InjuryType> injuryTypes,
        int weekNumber,
        CancellationToken cancellationToken)
    {
        // Determine difficulty level based on user's fitness level for this discipline
        var difficultyLevel = GetDifficultyForDiscipline(userProfile, workout.Discipline);

        // Get safe exercises for this discipline and session type
        var availableExercises = await GetAvailableExercisesAsync(
            workout.Discipline,
            difficultyLevel,
            workout.SessionType,
            injuryTypes,
            cancellationToken);

        if (!availableExercises.Any())
        {
            _logger.LogWarning("No exercises found for {Discipline} at {Difficulty} level", 
                workout.Discipline, difficultyLevel);
            return;
        }

        // Select exercises based on session type and phase
        var selectedExercises = SelectExercisesForWorkout(
            availableExercises.ToList(), 
            workout.SessionType, 
            workout.IntensityLevel);

        // Add exercises to workout with progressive overload parameters
        int order = 1;
        foreach (var exercise in selectedExercises)
        {
            var workoutExercise = new WorkoutExercise
            {
                WorkoutId = workout.Id,
                ExerciseId = exercise.Id,
                OrderInWorkout = order++,
                Exercise = exercise
            };

            // Apply progressive overload based on week number and phase
            ApplyProgressiveOverload(workoutExercise, exercise, weekNumber, workout.IntensityLevel);

            workout.WorkoutExercises.Add(workoutExercise);
        }
    }

    #endregion

    #region Helper Methods - Exercise Selection

    private async Task<IEnumerable<Exercise>> GetAvailableExercisesAsync(
        Discipline discipline,
        DifficultyLevel difficultyLevel,
        SessionType? sessionType,
        List<InjuryType> injuryTypes,
        CancellationToken cancellationToken)
    {
        if (injuryTypes.Any())
        {
            return await _exerciseRepository.GetSafeExercisesAsync(
                injuryTypes,
                discipline,
                difficultyLevel,
                cancellationToken);
        }

        return await _exerciseRepository.GetExercisesByCriteriaAsync(
            discipline,
            difficultyLevel,
            sessionType,
            cancellationToken);
    }

    private List<Exercise> SelectExercisesForWorkout(
        List<Exercise> availableExercises,
        SessionType? sessionType,
        IntensityLevel intensityLevel)
    {
        var selected = new List<Exercise>();
        
        // Determine exercise count based on session type and intensity
        int exerciseCount = sessionType switch
        {
            SessionType.FullBody => 6,
            SessionType.UpperLower => 5,
            SessionType.PushPullLegs => 5,
            SessionType.LongRun => 1,
            SessionType.Intervals => 3,
            SessionType.RaceSimulation => 8,
            SessionType.StationPractice => 4,
            _ => 4
        };

        // Prioritize compound movements
        var compoundExercises = availableExercises
            .Where(e => e.ExerciseMovementPatterns.Count >= 2)
            .ToList();

        // Select exercises with variety
        var random = new Random(DateTime.UtcNow.Millisecond);
        var pool = compoundExercises.Any() ? compoundExercises : availableExercises;
        
        while (selected.Count < exerciseCount && pool.Any())
        {
            var index = random.Next(pool.Count);
            selected.Add(pool[index]);
            pool.RemoveAt(index);
        }

        return selected;
    }

    #endregion

    #region Helper Methods - Progressive Overload

    /// <summary>
    /// Applies progressive overload principles to exercise parameters
    /// Implements 10-15% weekly increases in volume or intensity
    /// </summary>
    private void ApplyProgressiveOverload(
        WorkoutExercise workoutExercise,
        Exercise exercise,
        int weekNumber,
        IntensityLevel intensityLevel)
    {
        // Base values for week 1
        var baseSets = 3;
        var baseReps = 10;
        var baseDuration = 300; // 5 minutes

        // Calculate progression multiplier (10% per week)
        var progressionMultiplier = 1.0m + ((weekNumber - 1) * ProgressiveOverloadIncreasePercent);

        // Apply based on exercise type
        if (exercise.ApproximateDuration.HasValue)
        {
            // Duration-based exercise (e.g., running, rowing)
            workoutExercise.Duration = (int)(baseDuration * progressionMultiplier);
            workoutExercise.IntensityGuidance = GetIntensityGuidance(intensityLevel, exercise.PrimaryDiscipline);
        }
        else
        {
            // Rep-based exercise (e.g., strength)
            workoutExercise.Sets = baseSets;
            workoutExercise.Reps = (int)(baseReps * progressionMultiplier);
            workoutExercise.RestPeriod = GetRestPeriod(intensityLevel);
            workoutExercise.IntensityGuidance = GetIntensityGuidance(intensityLevel, exercise.PrimaryDiscipline);
        }
    }

    private string GetIntensityGuidance(IntensityLevel level, Discipline discipline)
    {
        return level switch
        {
            IntensityLevel.Low when discipline == Discipline.Strength => "60-70% of max",
            IntensityLevel.Moderate when discipline == Discipline.Strength => "70-80% of max",
            IntensityLevel.High when discipline == Discipline.Strength => "80-90% of max",
            IntensityLevel.Low when discipline == Discipline.Running => "Easy pace, conversational",
            IntensityLevel.Moderate when discipline == Discipline.Running => "Tempo pace, slightly uncomfortable",
            IntensityLevel.High when discipline == Discipline.Running => "Hard pace, near maximum effort",
            _ => "RPE 5-7"
        };
    }

    private int GetRestPeriod(IntensityLevel level)
    {
        return level switch
        {
            IntensityLevel.Low => 60,
            IntensityLevel.Moderate => 90,
            IntensityLevel.High => 120,
            _ => 90
        };
    }

    #endregion

    #region Helper Methods - Discipline and Schedule

    private Dictionary<Discipline, int> GetDisciplinePriorities(UserProfile userProfile)
    {
        var priorities = new Dictionary<Discipline, int>();
        var activeGoals = userProfile.TrainingGoals?
            .Where(g => g.Status == GoalStatus.Active)
            .ToList() ?? new List<TrainingGoal>();

        foreach (var goal in activeGoals)
        {
            var discipline = goal.GoalType switch
            {
                GoalType.HyroxRace => Discipline.HYROX,
                GoalType.RunningDistance => Discipline.Running,
                GoalType.StrengthMilestone => Discipline.Strength,
                GoalType.GeneralFitness => Discipline.HYROX, // Default to hybrid training
                _ => Discipline.HYROX
            };

            if (priorities.ContainsKey(discipline))
                priorities[discipline] += (4 - goal.Priority); // Higher priority = higher value
            else
                priorities[discipline] = (4 - goal.Priority);
        }

        // Ensure at least one discipline
        if (!priorities.Any())
        {
            priorities[Discipline.HYROX] = 1;
        }

        return priorities;
    }

    private List<Discipline> AllocateDisciplines(Dictionary<Discipline, int> priorities, int workoutsPerWeek)
    {
        var allocation = new List<Discipline>();
        var totalPriority = priorities.Sum(p => p.Value);

        foreach (var priority in priorities.OrderByDescending(p => p.Value))
        {
            var count = Math.Max(1, (int)Math.Round((double)priority.Value / totalPriority * workoutsPerWeek));
            for (int i = 0; i < count && allocation.Count < workoutsPerWeek; i++)
            {
                allocation.Add(priority.Key);
            }
        }

        // Fill remaining slots with primary discipline
        while (allocation.Count < workoutsPerWeek)
        {
            allocation.Add(priorities.OrderByDescending(p => p.Value).First().Key);
        }

        return allocation;
    }

    private List<WorkoutDay> GetAvailableTrainingDays(ScheduleAvailability schedule)
    {
        var days = new List<WorkoutDay>();
        
        if (schedule.Monday) days.Add(WorkoutDay.Monday);
        if (schedule.Tuesday) days.Add(WorkoutDay.Tuesday);
        if (schedule.Wednesday) days.Add(WorkoutDay.Wednesday);
        if (schedule.Thursday) days.Add(WorkoutDay.Thursday);
        if (schedule.Friday) days.Add(WorkoutDay.Friday);
        if (schedule.Saturday) days.Add(WorkoutDay.Saturday);
        if (schedule.Sunday) days.Add(WorkoutDay.Sunday);

        return days;
    }

    private List<WorkoutDay> SelectTrainingDays(List<WorkoutDay> availableDays, int count)
    {
        // Distribute training days evenly throughout the week
        var selected = new List<WorkoutDay>();
        var step = (double)availableDays.Count / count;

        for (int i = 0; i < count; i++)
        {
            var index = (int)Math.Round(i * step);
            if (index >= availableDays.Count)
                index = availableDays.Count - 1;
            
            selected.Add(availableDays[index]);
        }

        return selected.Distinct().ToList();
    }

    #endregion

    #region Helper Methods - Phase and Intensity

    private PeriodizationPhase GetPhaseForWeek(List<PeriodizationPhase> phases, int weekNumber)
    {
        return phases.First(p => weekNumber >= p.StartWeek && weekNumber <= p.EndWeek);
    }

    private IntensityLevel DetermineIntensityLevel(TrainingPhase phase, bool isDeloadWeek)
    {
        if (isDeloadWeek)
            return IntensityLevel.Low;

        return phase switch
        {
            TrainingPhase.Foundation => IntensityLevel.Low,
            TrainingPhase.Build => IntensityLevel.Moderate,
            TrainingPhase.Intensity => IntensityLevel.High,
            TrainingPhase.Peak => IntensityLevel.High,
            TrainingPhase.Taper => IntensityLevel.Low,
            TrainingPhase.Recovery => IntensityLevel.Low,
            _ => IntensityLevel.Moderate
        };
    }

    private string DetermineFocusArea(TrainingPhase phase, UserProfile userProfile)
    {
        var primaryGoal = GetPrimaryGoal(userProfile);
        
        return phase switch
        {
            TrainingPhase.Foundation => "Building aerobic base and movement quality",
            TrainingPhase.Build => $"Increasing volume for {primaryGoal?.GoalType}",
            TrainingPhase.Intensity => "High-intensity work and specific adaptations",
            TrainingPhase.Peak => "Peak performance and race-specific training",
            TrainingPhase.Taper => "Recovery and sharpening for event",
            _ => "General fitness improvement"
        };
    }

    private SessionType DetermineSessionType(TrainingPhase phase, Discipline discipline, int dayIndex, int totalDays)
    {
        // Vary session types based on discipline and phase
        if (discipline == Discipline.Running)
        {
            return phase switch
            {
                TrainingPhase.Foundation => dayIndex == 0 ? SessionType.EasyRun : SessionType.LongRun,
                TrainingPhase.Build => dayIndex % 2 == 0 ? SessionType.Tempo : SessionType.LongRun,
                TrainingPhase.Intensity => SessionType.Intervals,
                _ => SessionType.EasyRun
            };
        }
        else if (discipline == Discipline.Strength)
        {
            return totalDays >= 3 ? SessionType.PushPullLegs : SessionType.FullBody;
        }
        else // Hyrox
        {
            return phase switch
            {
                TrainingPhase.Peak => SessionType.RaceSimulation,
                TrainingPhase.Intensity => SessionType.StationPractice,
                _ => SessionType.HybridConditioning
            };
        }
    }

    private DifficultyLevel GetDifficultyForDiscipline(UserProfile userProfile, Discipline discipline)
    {
        var fitnessLevel = discipline switch
        {
            Discipline.HYROX => userProfile.HyroxLevel,
            Discipline.Running => userProfile.RunningLevel,
            Discipline.Strength => userProfile.StrengthLevel,
            _ => FitnessLevel.Beginner
        };

        return fitnessLevel switch
        {
            FitnessLevel.Beginner => DifficultyLevel.Beginner,
            FitnessLevel.Intermediate => DifficultyLevel.Intermediate,
            FitnessLevel.Advanced => DifficultyLevel.Advanced,
            _ => DifficultyLevel.Beginner
        };
    }

    #endregion

    #region Helper Methods - Workout Details

    private string GenerateWorkoutName(Discipline discipline, SessionType? sessionType, TrainingPhase phase)
    {
        var phaseName = phase.ToString();
        return $"{discipline} - {sessionType?.ToString() ?? "Training"} ({phaseName})";
    }

    private string GenerateWorkoutDescription(Discipline discipline, SessionType? sessionType, TrainingPhase phase)
    {
        return $"Focus on {sessionType?.ToString().ToLower() ?? "general training"} during {phase.ToString().ToLower()} phase. " +
               $"Maintain proper form and listen to your body.";
    }

    private int EstimateWorkoutDuration(SessionType? sessionType, IntensityLevel intensityLevel)
    {
        var baseDuration = sessionType switch
        {
            SessionType.EasyRun => 30,
            SessionType.LongRun => 60,
            SessionType.Intervals => 45,
            SessionType.Tempo => 40,
            SessionType.FullBody => 60,
            SessionType.UpperLower => 45,
            SessionType.PushPullLegs => 45,
            SessionType.RaceSimulation => 90,
            SessionType.StationPractice => 45,
            SessionType.HybridConditioning => 50,
            _ => 45
        };

        // Adjust for intensity
        return intensityLevel == IntensityLevel.High ? (int)(baseDuration * 1.2) : baseDuration;
    }

    private bool DetermineIfKeyWorkout(TrainingPhase phase, int dayIndex, int totalDays)
    {
        // Mark key workouts (typically the hardest session of the week)
        return phase == TrainingPhase.Peak && dayIndex == totalDays - 1;
    }

    private string GeneratePlanName(UserProfile userProfile, TrainingGoal? primaryGoal)
    {
        var goalType = primaryGoal?.GoalType.ToString() ?? "General Fitness";
        var startMonth = DateTime.UtcNow.ToString("MMM yyyy");
        return $"{goalType} Plan - {startMonth}";
    }

    #endregion

    #region Phase 7: Validation

    /// <summary>
    /// Validates the coherence and safety of the generated plan
    /// </summary>
    private void ValidatePlanCoherence(TrainingPlan plan)
    {
        // Ensure all weeks have workouts
        if (plan.TrainingWeeks.Any(w => !w.Workouts.Any()))
        {
            throw new InvalidOperationException("Plan contains weeks with no workouts");
        }

        // Ensure progressive overload is maintained
        for (int i = 1; i < plan.TrainingWeeks.Count; i++)
        {
            var prevWeek = plan.TrainingWeeks.ElementAt(i - 1);
            var currentWeek = plan.TrainingWeeks.ElementAt(i);

            // Skip deload weeks
            if (currentWeek.IntensityLevel == IntensityLevel.Low)
                continue;

            // Volume should generally increase (allowing for variation)
            if (currentWeek.WeeklyVolume < prevWeek.WeeklyVolume * 0.8)
            {
                _logger.LogWarning("Week {Week} has significantly lower volume than previous week", 
                    currentWeek.WeekNumber);
            }
        }

        _logger.LogInformation("Plan validation completed successfully");
    }

    #endregion

    #region Supporting Classes

    private class PeriodizationPhase
    {
        public TrainingPhase Phase { get; }
        public int StartWeek { get; }
        public int EndWeek { get; }

        public PeriodizationPhase(TrainingPhase phase, int startWeek, int endWeek)
        {
            Phase = phase;
            StartWeek = startWeek;
            EndWeek = endWeek;
        }
    }

    #endregion
}
