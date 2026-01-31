using Xunit;
using FluentAssertions;
using FitnessApp.Domain.Entities;
using FitnessApp.Domain.Enums;

namespace FitnessApp.UnitTests.Domain.Entities;

public class TrainingWeekTests
{
    [Fact]
    public void TrainingWeek_WithValidData_ShouldCreateSuccessfully()
    {
        // Arrange & Act
        var startDate = DateTime.UtcNow.Date;
        var endDate = startDate.AddDays(7);
        var week = new TrainingWeek
        {
            PlanId = Guid.NewGuid(),
            WeekNumber = 1,
            Phase = TrainingPhase.Foundation,
            WeeklyVolume = 300,
            IntensityLevel = IntensityLevel.Low,
            FocusArea = "Building aerobic base",
            StartDate = startDate,
            EndDate = endDate
        };

        // Assert
        week.WeekNumber.Should().Be(1);
        week.Phase.Should().Be(TrainingPhase.Foundation);
        week.WeeklyVolume.Should().Be(300);
        week.IntensityLevel.Should().Be(IntensityLevel.Low);
        week.FocusArea.Should().Be("Building aerobic base");
        week.StartDate.Should().Be(startDate);
        week.EndDate.Should().Be(endDate);
    }

    [Fact]
    public void TrainingWeek_CanHaveMultipleWorkouts()
    {
        // Arrange
        var week = new TrainingWeek
        {
            PlanId = Guid.NewGuid(),
            WeekNumber = 2,
            Phase = TrainingPhase.Build,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(7)
        };

        var workout1 = new Workout
        {
            WeekId = week.Id,
            DayOfWeek = WorkoutDay.Monday,
            ScheduledDate = week.StartDate,
            Discipline = Discipline.Running,
            WorkoutName = "Easy Run",
            CompletionStatus = CompletionStatus.NotStarted
        };

        var workout2 = new Workout
        {
            WeekId = week.Id,
            DayOfWeek = WorkoutDay.Wednesday,
            ScheduledDate = week.StartDate.AddDays(2),
            Discipline = Discipline.Strength,
            WorkoutName = "Full Body",
            CompletionStatus = CompletionStatus.NotStarted
        };

        // Act
        week.Workouts.Add(workout1);
        week.Workouts.Add(workout2);

        // Assert
        week.Workouts.Should().HaveCount(2);
        week.Workouts.Should().Contain(w => w.DayOfWeek == WorkoutDay.Monday);
        week.Workouts.Should().Contain(w => w.DayOfWeek == WorkoutDay.Wednesday);
    }

    [Fact]
    public void TrainingWeek_PhaseProgression_ShouldWork()
    {
        // Arrange
        var week1 = new TrainingWeek
        {
            PlanId = Guid.NewGuid(),
            WeekNumber = 1,
            Phase = TrainingPhase.Foundation,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(7)
        };

        var week2 = new TrainingWeek
        {
            PlanId = week1.PlanId,
            WeekNumber = 2,
            Phase = TrainingPhase.Build,
            StartDate = week1.EndDate,
            EndDate = week1.EndDate.AddDays(7)
        };

        // Assert
        week1.Phase.Should().Be(TrainingPhase.Foundation);
        week2.Phase.Should().Be(TrainingPhase.Build);
        week2.WeekNumber.Should().BeGreaterThan(week1.WeekNumber);
    }

    [Fact]
    public void TrainingWeek_IntensityLevels_ShouldBeAssignable()
    {
        // Arrange & Act
        var lowIntensityWeek = new TrainingWeek
        {
            PlanId = Guid.NewGuid(),
            WeekNumber = 1,
            Phase = TrainingPhase.Foundation,
            IntensityLevel = IntensityLevel.Low,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(7)
        };

        var highIntensityWeek = new TrainingWeek
        {
            PlanId = Guid.NewGuid(),
            WeekNumber = 1,
            Phase = TrainingPhase.Peak,
            IntensityLevel = IntensityLevel.High,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(7)
        };

        // Assert
        lowIntensityWeek.IntensityLevel.Should().Be(IntensityLevel.Low);
        highIntensityWeek.IntensityLevel.Should().Be(IntensityLevel.High);
    }

    [Fact]
    public void TrainingWeek_WeeklyVolume_CanBeNull()
    {
        // Arrange & Act
        var week = new TrainingWeek
        {
            PlanId = Guid.NewGuid(),
            WeekNumber = 1,
            Phase = TrainingPhase.Recovery,
            WeeklyVolume = null,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(7)
        };

        // Assert
        week.WeeklyVolume.Should().BeNull();
    }
}
