using FitnessApp.Application.DTOs;
using FitnessApp.Domain.Entities;
using FitnessApp.Domain.ValueObjects;

namespace FitnessApp.Application.Mapping;

/// <summary>
/// Mapping extensions for converting between domain entities and DTOs
/// </summary>
public static class MappingExtensions
{
    /// <summary>
    /// Maps a UserProfile entity to UserProfileDto
    /// </summary>
    public static UserProfileDto ToDto(this UserProfile profile)
    {
        return new UserProfileDto
        {
            Id = profile.Id,
            Name = profile.Name,
            Email = profile.Email,
            HyroxLevel = profile.HyroxLevel,
            RunningLevel = profile.RunningLevel,
            StrengthLevel = profile.StrengthLevel,
            ScheduleAvailability = profile.ScheduleAvailability?.ToDto(),
            TrainingBackground = profile.TrainingBackground?.ToDto(),
            TrainingGoals = profile.TrainingGoals?.Select(g => g.ToDto()).ToList() ?? new List<TrainingGoalDto>(),
            InjuryLimitations = profile.InjuryLimitations?.Select(i => i.ToDto()).ToList() ?? new List<InjuryLimitationDto>()
        };
    }

    /// <summary>
    /// Maps a ScheduleAvailability value object to ScheduleAvailabilityDto
    /// </summary>
    public static ScheduleAvailabilityDto ToDto(this ScheduleAvailability schedule)
    {
        return new ScheduleAvailabilityDto
        {
            Monday = schedule.Monday,
            Tuesday = schedule.Tuesday,
            Wednesday = schedule.Wednesday,
            Thursday = schedule.Thursday,
            Friday = schedule.Friday,
            Saturday = schedule.Saturday,
            Sunday = schedule.Sunday,
            MinimumSessionsPerWeek = schedule.MinimumSessionsPerWeek,
            MaximumSessionsPerWeek = schedule.MaximumSessionsPerWeek
        };
    }

    /// <summary>
    /// Maps a TrainingBackground entity to TrainingBackgroundDto
    /// </summary>
    public static TrainingBackgroundDto ToDto(this TrainingBackground background)
    {
        return new TrainingBackgroundDto
        {
            HasStructuredTrainingExperience = background.HasStructuredTrainingExperience,
            PreviousTrainingDetails = background.PreviousTrainingDetails,
            EquipmentFamiliarity = background.EquipmentFamiliarity,
            TrainingHistoryDetails = background.TrainingHistoryDetails
        };
    }

    /// <summary>
    /// Maps a TrainingGoal entity to TrainingGoalDto
    /// </summary>
    public static TrainingGoalDto ToDto(this TrainingGoal goal)
    {
        return new TrainingGoalDto
        {
            Id = goal.Id,
            GoalType = goal.GoalType,
            Description = goal.Description,
            TargetDate = goal.TargetDate,
            Priority = goal.Priority,
            Status = goal.Status
        };
    }

    /// <summary>
    /// Maps an InjuryLimitation entity to InjuryLimitationDto
    /// </summary>
    public static InjuryLimitationDto ToDto(this InjuryLimitation injury)
    {
        return new InjuryLimitationDto
        {
            Id = injury.Id,
            BodyPart = injury.BodyPart,
            InjuryType = injury.InjuryType,
            ReportedDate = injury.ReportedDate,
            Status = injury.Status,
            MovementRestrictions = injury.MovementRestrictions
        };
    }

    /// <summary>
    /// Maps a ScheduleAvailabilityDto to ScheduleAvailability value object
    /// </summary>
    public static ScheduleAvailability ToEntity(this ScheduleAvailabilityDto dto)
    {
        return new ScheduleAvailability
        {
            Monday = dto.Monday,
            Tuesday = dto.Tuesday,
            Wednesday = dto.Wednesday,
            Thursday = dto.Thursday,
            Friday = dto.Friday,
            Saturday = dto.Saturday,
            Sunday = dto.Sunday,
            MinimumSessionsPerWeek = dto.MinimumSessionsPerWeek,
            MaximumSessionsPerWeek = dto.MaximumSessionsPerWeek
        };
    }

    /// <summary>
    /// Maps a TrainingBackgroundDto to TrainingBackground entity
    /// </summary>
    public static TrainingBackground ToEntity(this TrainingBackgroundDto dto, Guid userProfileId)
    {
        return new TrainingBackground
        {
            UserProfileId = userProfileId,
            HasStructuredTrainingExperience = dto.HasStructuredTrainingExperience,
            PreviousTrainingDetails = dto.PreviousTrainingDetails,
            EquipmentFamiliarity = dto.EquipmentFamiliarity,
            TrainingHistoryDetails = dto.TrainingHistoryDetails
        };
    }

    /// <summary>
    /// Maps a TrainingGoalDto to TrainingGoal entity
    /// </summary>
    public static TrainingGoal ToEntity(this TrainingGoalDto dto, Guid userProfileId)
    {
        return new TrainingGoal
        {
            UserProfileId = userProfileId,
            GoalType = dto.GoalType,
            Description = dto.Description,
            TargetDate = dto.TargetDate,
            Priority = dto.Priority,
            Status = dto.Status
        };
    }

    /// <summary>
    /// Maps an InjuryLimitationDto to InjuryLimitation entity
    /// </summary>
    public static InjuryLimitation ToEntity(this InjuryLimitationDto dto, Guid userProfileId)
    {
        return new InjuryLimitation
        {
            UserProfileId = userProfileId,
            BodyPart = dto.BodyPart,
            InjuryType = dto.InjuryType,
            ReportedDate = dto.ReportedDate,
            Status = dto.Status,
            MovementRestrictions = dto.MovementRestrictions
        };
    }

    /// <summary>
    /// Updates a TrainingGoal entity from a TrainingGoalDto
    /// </summary>
    public static void UpdateFromDto(this TrainingGoal goal, TrainingGoalDto dto)
    {
        goal.GoalType = dto.GoalType;
        goal.Description = dto.Description;
        goal.TargetDate = dto.TargetDate;
        goal.Priority = dto.Priority;
        goal.Status = dto.Status;
    }

    /// <summary>
    /// Updates an InjuryLimitation entity from an InjuryLimitationDto
    /// </summary>
    public static void UpdateFromDto(this InjuryLimitation injury, InjuryLimitationDto dto)
    {
        injury.BodyPart = dto.BodyPart;
        injury.InjuryType = dto.InjuryType;
        injury.ReportedDate = dto.ReportedDate;
        injury.Status = dto.Status;
        injury.MovementRestrictions = dto.MovementRestrictions;
    }

    /// <summary>
    /// Updates a UserProfile entity from a UserProfileDto
    /// </summary>
    public static void UpdateFromDto(this UserProfile profile, UserProfileDto dto)
    {
        profile.Name = dto.Name;
        profile.Email = dto.Email;
        profile.HyroxLevel = dto.HyroxLevel;
        profile.RunningLevel = dto.RunningLevel;
        profile.StrengthLevel = dto.StrengthLevel;
        profile.ScheduleAvailability = dto.ScheduleAvailability?.ToEntity();
    }

    /// <summary>
    /// Maps a TrainingPlan entity to TrainingPlanSummaryDto
    /// </summary>
    public static TrainingPlanSummaryDto ToSummaryDto(this TrainingPlan plan)
    {
        var totalWorkouts = plan.TrainingWeeks?.SelectMany(w => w.Workouts).Count() ?? 0;
        var completedWorkouts = plan.TrainingWeeks?.SelectMany(w => w.Workouts)
            .Count(w => w.CompletionStatus == Domain.Enums.CompletionStatus.Completed) ?? 0;
        var progress = totalWorkouts > 0 ? (decimal)completedWorkouts / totalWorkouts * 100 : 0;

        return new TrainingPlanSummaryDto
        {
            Id = plan.Id,
            PlanName = plan.PlanName,
            StartDate = plan.StartDate,
            EndDate = plan.EndDate,
            TotalWeeks = plan.TotalWeeks,
            TrainingDaysPerWeek = plan.TrainingDaysPerWeek,
            CurrentWeek = plan.CurrentWeek,
            Status = plan.Status,
            PrimaryGoalId = plan.PrimaryGoalId,
            CreatedAt = plan.CreatedAt,
            ProgressPercentage = Math.Round(progress, 2)
        };
    }

    /// <summary>
    /// Maps a TrainingPlan entity to TrainingPlanDetailDto
    /// </summary>
    public static TrainingPlanDetailDto ToDetailDto(this TrainingPlan plan)
    {
        var totalWorkouts = plan.TrainingWeeks?.SelectMany(w => w.Workouts).Count() ?? 0;
        var completedWorkouts = plan.TrainingWeeks?.SelectMany(w => w.Workouts)
            .Count(w => w.CompletionStatus == Domain.Enums.CompletionStatus.Completed) ?? 0;
        var progress = totalWorkouts > 0 ? (decimal)completedWorkouts / totalWorkouts * 100 : 0;

        return new TrainingPlanDetailDto
        {
            Id = plan.Id,
            PlanName = plan.PlanName,
            StartDate = plan.StartDate,
            EndDate = plan.EndDate,
            TotalWeeks = plan.TotalWeeks,
            TrainingDaysPerWeek = plan.TrainingDaysPerWeek,
            CurrentWeek = plan.CurrentWeek,
            Status = plan.Status,
            PrimaryGoalId = plan.PrimaryGoalId,
            CreatedAt = plan.CreatedAt,
            ProgressPercentage = Math.Round(progress, 2),
            TrainingWeeks = plan.TrainingWeeks?.OrderBy(w => w.WeekNumber).Select(w => w.ToDto()).ToList() ?? new List<TrainingWeekDto>()
        };
    }

    /// <summary>
    /// Maps a TrainingWeek entity to TrainingWeekDto
    /// </summary>
    public static TrainingWeekDto ToDto(this TrainingWeek week)
    {
        return new TrainingWeekDto
        {
            Id = week.Id,
            WeekNumber = week.WeekNumber,
            Phase = week.Phase,
            WeeklyVolume = week.WeeklyVolume,
            IntensityLevel = week.IntensityLevel,
            FocusArea = week.FocusArea,
            StartDate = week.StartDate,
            EndDate = week.EndDate,
            Workouts = week.Workouts?.OrderBy(w => w.DayOfWeek).Select(w => w.ToSummaryDto()).ToList() ?? new List<WorkoutSummaryDto>()
        };
    }

    /// <summary>
    /// Maps a Workout entity to WorkoutSummaryDto
    /// </summary>
    public static WorkoutSummaryDto ToSummaryDto(this Workout workout)
    {
        return new WorkoutSummaryDto
        {
            Id = workout.Id,
            WorkoutName = workout.WorkoutName,
            ScheduledDate = workout.ScheduledDate,
            DayOfWeek = workout.DayOfWeek,
            Discipline = workout.Discipline,
            SessionType = workout.SessionType,
            EstimatedDuration = workout.EstimatedDuration,
            IntensityLevel = workout.IntensityLevel,
            IsKeyWorkout = workout.IsKeyWorkout,
            CompletionStatus = workout.CompletionStatus,
            CompletedAt = workout.CompletedAt
        };
    }

    /// <summary>
    /// Maps a Workout entity to WorkoutDetailDto
    /// </summary>
    public static WorkoutDetailDto ToDetailDto(this Workout workout)
    {
        return new WorkoutDetailDto
        {
            Id = workout.Id,
            WorkoutName = workout.WorkoutName,
            Description = workout.Description,
            ScheduledDate = workout.ScheduledDate,
            DayOfWeek = workout.DayOfWeek,
            Discipline = workout.Discipline,
            SessionType = workout.SessionType,
            EstimatedDuration = workout.EstimatedDuration,
            IntensityLevel = workout.IntensityLevel,
            IsKeyWorkout = workout.IsKeyWorkout,
            CompletionStatus = workout.CompletionStatus,
            CompletedAt = workout.CompletedAt,
            WorkoutExercises = workout.WorkoutExercises?.OrderBy(we => we.OrderInWorkout).Select(we => we.ToDto()).ToList() ?? new List<WorkoutExerciseDto>()
        };
    }

    /// <summary>
    /// Maps a WorkoutExercise entity to WorkoutExerciseDto
    /// </summary>
    public static WorkoutExerciseDto ToDto(this WorkoutExercise workoutExercise)
    {
        return new WorkoutExerciseDto
        {
            Id = workoutExercise.Id,
            ExerciseId = workoutExercise.ExerciseId,
            ExerciseName = workoutExercise.Exercise?.Name ?? "Unknown Exercise",
            OrderInWorkout = workoutExercise.OrderInWorkout,
            Sets = workoutExercise.Sets,
            Reps = workoutExercise.Reps,
            Duration = workoutExercise.Duration,
            RestPeriod = workoutExercise.RestPeriod,
            IntensityGuidance = workoutExercise.IntensityGuidance,
            Notes = workoutExercise.Notes,
            Description = workoutExercise.Exercise?.Description
        };
    }
}
