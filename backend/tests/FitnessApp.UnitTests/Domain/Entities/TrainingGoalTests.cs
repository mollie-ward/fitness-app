using Xunit;
using FluentAssertions;
using FitnessApp.Domain.Entities;
using FitnessApp.Domain.Enums;

namespace FitnessApp.UnitTests.Domain.Entities;

public class TrainingGoalTests
{
    [Fact]
    public void TrainingGoal_WithValidData_ShouldCreateSuccessfully()
    {
        // Arrange & Act
        var targetDate = DateTime.UtcNow.AddMonths(3);
        var goal = new TrainingGoal
        {
            UserProfileId = Guid.NewGuid(),
            Description = "Complete HYROX race in under 90 minutes",
            GoalType = GoalType.HyroxRace,
            TargetDate = targetDate,
            Priority = 1,
            Status = GoalStatus.Active
        };

        // Assert
        goal.Description.Should().Be("Complete HYROX race in under 90 minutes");
        goal.GoalType.Should().Be(GoalType.HyroxRace);
        goal.TargetDate.Should().Be(targetDate);
        goal.Priority.Should().Be(1);
        goal.Status.Should().Be(GoalStatus.Active);
    }

    [Fact]
    public void TrainingGoal_WithFutureTargetDate_IsValid()
    {
        // Arrange
        var goal = new TrainingGoal
        {
            UserProfileId = Guid.NewGuid(),
            Description = "Run 10K",
            GoalType = GoalType.RunningDistance,
            TargetDate = DateTime.UtcNow.AddDays(30),
            Priority = 1
        };

        // Act
        var isValid = goal.IsValid();

        // Assert
        isValid.Should().BeTrue();
    }

    [Fact]
    public void TrainingGoal_WithPastTargetDate_IsInvalid()
    {
        // Arrange
        var goal = new TrainingGoal
        {
            UserProfileId = Guid.NewGuid(),
            Description = "Run 10K",
            GoalType = GoalType.RunningDistance,
            TargetDate = DateTime.UtcNow.AddDays(-1),
            Priority = 1
        };

        // Act
        var isValid = goal.IsValid();

        // Assert
        isValid.Should().BeFalse();
    }

    [Fact]
    public void TrainingGoal_WithTodayTargetDate_IsValid()
    {
        // Arrange
        var goal = new TrainingGoal
        {
            UserProfileId = Guid.NewGuid(),
            Description = "Complete workout today",
            GoalType = GoalType.GeneralFitness,
            TargetDate = DateTime.UtcNow.Date,
            Priority = 1
        };

        // Act
        var isValid = goal.IsValid();

        // Assert
        isValid.Should().BeTrue();
    }

    [Fact]
    public void TrainingGoal_WithNullTargetDate_IsValid()
    {
        // Arrange
        var goal = new TrainingGoal
        {
            UserProfileId = Guid.NewGuid(),
            Description = "General fitness improvement",
            GoalType = GoalType.GeneralFitness,
            TargetDate = null,
            Priority = 1
        };

        // Act
        var isValid = goal.IsValid();

        // Assert
        isValid.Should().BeTrue();
    }

    [Fact]
    public void TrainingGoal_WithZeroPriority_IsInvalid()
    {
        // Arrange
        var goal = new TrainingGoal
        {
            UserProfileId = Guid.NewGuid(),
            Description = "Run 10K",
            GoalType = GoalType.RunningDistance,
            TargetDate = DateTime.UtcNow.AddDays(30),
            Priority = 0
        };

        // Act
        var isValid = goal.IsValid();

        // Assert
        isValid.Should().BeFalse();
    }

    [Fact]
    public void TrainingGoal_ShouldHaveDefaultStatus()
    {
        // Arrange & Act
        var goal = new TrainingGoal
        {
            UserProfileId = Guid.NewGuid(),
            Description = "Test goal",
            GoalType = GoalType.GeneralFitness
        };

        // Assert
        goal.Status.Should().Be(GoalStatus.Active);
        goal.Priority.Should().Be(1);
    }
}
