using Xunit;
using FluentAssertions;
using FitnessApp.Domain.Entities;
using FitnessApp.Domain.Enums;

namespace FitnessApp.UnitTests.Domain.Entities;

public class WorkoutTests
{
    [Fact]
    public void Workout_WithValidData_ShouldCreateSuccessfully()
    {
        // Arrange & Act
        var scheduledDate = DateTime.UtcNow.Date;
        var workout = new Workout
        {
            WeekId = Guid.NewGuid(),
            DayOfWeek = WorkoutDay.Monday,
            ScheduledDate = scheduledDate,
            Discipline = Discipline.HYROX,
            WorkoutName = "HYROX Station Practice",
            Description = "Focus on sled push and pull technique",
            EstimatedDuration = 60,
            IntensityLevel = IntensityLevel.Moderate,
            IsKeyWorkout = false,
            CompletionStatus = CompletionStatus.NotStarted
        };

        // Assert
        workout.WorkoutName.Should().Be("HYROX Station Practice");
        workout.Discipline.Should().Be(Discipline.HYROX);
        workout.DayOfWeek.Should().Be(WorkoutDay.Monday);
        workout.ScheduledDate.Should().Be(scheduledDate);
        workout.EstimatedDuration.Should().Be(60);
        workout.IntensityLevel.Should().Be(IntensityLevel.Moderate);
        workout.CompletionStatus.Should().Be(CompletionStatus.NotStarted);
        workout.CompletedAt.Should().BeNull();
    }

    [Fact]
    public void Workout_CompletionStatus_NotStartedToCompleted()
    {
        // Arrange
        var workout = new Workout
        {
            WeekId = Guid.NewGuid(),
            DayOfWeek = WorkoutDay.Tuesday,
            ScheduledDate = DateTime.UtcNow,
            Discipline = Discipline.Running,
            WorkoutName = "Easy Run",
            CompletionStatus = CompletionStatus.NotStarted
        };

        // Act
        workout.CompletionStatus = CompletionStatus.Completed;
        workout.CompletedAt = DateTime.UtcNow;

        // Assert
        workout.CompletionStatus.Should().Be(CompletionStatus.Completed);
        workout.CompletedAt.Should().NotBeNull();
        workout.CompletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Workout_CompletionStatus_TransitionToSkipped()
    {
        // Arrange
        var workout = new Workout
        {
            WeekId = Guid.NewGuid(),
            DayOfWeek = WorkoutDay.Wednesday,
            ScheduledDate = DateTime.UtcNow,
            Discipline = Discipline.Strength,
            WorkoutName = "Full Body Strength",
            CompletionStatus = CompletionStatus.NotStarted
        };

        // Act
        workout.CompletionStatus = CompletionStatus.Skipped;

        // Assert
        workout.CompletionStatus.Should().Be(CompletionStatus.Skipped);
        workout.CompletedAt.Should().BeNull();
    }

    [Fact]
    public void Workout_CompletionStatus_InProgressToCompleted()
    {
        // Arrange
        var workout = new Workout
        {
            WeekId = Guid.NewGuid(),
            DayOfWeek = WorkoutDay.Thursday,
            ScheduledDate = DateTime.UtcNow,
            Discipline = Discipline.Hybrid,
            WorkoutName = "Hybrid Conditioning",
            CompletionStatus = CompletionStatus.InProgress
        };

        // Act
        workout.CompletionStatus = CompletionStatus.Completed;
        workout.CompletedAt = DateTime.UtcNow;

        // Assert
        workout.CompletionStatus.Should().Be(CompletionStatus.Completed);
        workout.CompletedAt.Should().NotBeNull();
    }

    [Fact]
    public void Workout_IsKeyWorkout_CanBeSet()
    {
        // Arrange & Act
        var workout = new Workout
        {
            WeekId = Guid.NewGuid(),
            DayOfWeek = WorkoutDay.Saturday,
            ScheduledDate = DateTime.UtcNow,
            Discipline = Discipline.HYROX,
            WorkoutName = "HYROX Race Simulation",
            IsKeyWorkout = true,
            CompletionStatus = CompletionStatus.NotStarted
        };

        // Assert
        workout.IsKeyWorkout.Should().BeTrue();
    }

    [Fact]
    public void Workout_CanAddWorkoutExercises()
    {
        // Arrange
        var workout = new Workout
        {
            WeekId = Guid.NewGuid(),
            DayOfWeek = WorkoutDay.Friday,
            ScheduledDate = DateTime.UtcNow,
            Discipline = Discipline.Strength,
            WorkoutName = "Upper Body",
            CompletionStatus = CompletionStatus.NotStarted
        };

        var exercise1 = new WorkoutExercise
        {
            WorkoutId = workout.Id,
            ExerciseId = Guid.NewGuid(),
            OrderInWorkout = 1,
            Sets = 3,
            Reps = 10,
            RestPeriod = 90,
            IntensityGuidance = "70% max"
        };

        var exercise2 = new WorkoutExercise
        {
            WorkoutId = workout.Id,
            ExerciseId = Guid.NewGuid(),
            OrderInWorkout = 2,
            Sets = 4,
            Reps = 8,
            RestPeriod = 120,
            IntensityGuidance = "75% max"
        };

        // Act
        workout.WorkoutExercises.Add(exercise1);
        workout.WorkoutExercises.Add(exercise2);

        // Assert
        workout.WorkoutExercises.Should().HaveCount(2);
        workout.WorkoutExercises.First().OrderInWorkout.Should().Be(1);
        workout.WorkoutExercises.Last().OrderInWorkout.Should().Be(2);
    }

    [Fact]
    public void Workout_WithSessionType_ShouldStoreCorrectly()
    {
        // Arrange & Act
        var workout = new Workout
        {
            WeekId = Guid.NewGuid(),
            DayOfWeek = WorkoutDay.Sunday,
            ScheduledDate = DateTime.UtcNow,
            Discipline = Discipline.Running,
            SessionType = SessionType.Intervals,
            WorkoutName = "Interval Training - 8x400m",
            CompletionStatus = CompletionStatus.NotStarted
        };

        // Assert
        workout.SessionType.Should().Be(SessionType.Intervals);
    }
}
