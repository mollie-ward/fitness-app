using Xunit;
using FluentAssertions;
using FitnessApp.Domain.Entities;
using FitnessApp.Domain.Enums;

namespace FitnessApp.UnitTests.Domain.Entities;

public class UserProfileTests
{
    [Fact]
    public void UserProfile_WithValidData_ShouldCreateSuccessfully()
    {
        // Arrange & Act
        var userId = Guid.NewGuid();
        var profile = new UserProfile
        {
            UserId = userId,
            Name = "John Doe",
            Email = "john.doe@example.com",
            HyroxLevel = FitnessLevel.Intermediate,
            RunningLevel = FitnessLevel.Advanced,
            StrengthLevel = FitnessLevel.Beginner
        };

        // Assert
        profile.UserId.Should().Be(userId);
        profile.Name.Should().Be("John Doe");
        profile.Email.Should().Be("john.doe@example.com");
        profile.HyroxLevel.Should().Be(FitnessLevel.Intermediate);
        profile.RunningLevel.Should().Be(FitnessLevel.Advanced);
        profile.StrengthLevel.Should().Be(FitnessLevel.Beginner);
        profile.TrainingGoals.Should().NotBeNull();
        profile.InjuryLimitations.Should().NotBeNull();
    }

    [Fact]
    public void UserProfile_ShouldInitializeCollections()
    {
        // Arrange & Act
        var profile = new UserProfile
        {
            UserId = Guid.NewGuid(),
            Name = "Jane Doe",
            Email = "jane.doe@example.com"
        };

        // Assert
        profile.TrainingGoals.Should().NotBeNull().And.BeEmpty();
        profile.InjuryLimitations.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void UserProfile_CanAddTrainingGoals()
    {
        // Arrange
        var profile = new UserProfile
        {
            UserId = Guid.NewGuid(),
            Name = "John Doe",
            Email = "john.doe@example.com"
        };

        var goal = new TrainingGoal
        {
            UserProfileId = profile.Id,
            Description = "Complete HYROX race",
            GoalType = GoalType.HyroxRace,
            TargetDate = DateTime.UtcNow.AddMonths(3)
        };

        // Act
        profile.TrainingGoals.Add(goal);

        // Assert
        profile.TrainingGoals.Should().HaveCount(1);
        profile.TrainingGoals.First().Should().Be(goal);
    }

    [Fact]
    public void UserProfile_CanAddInjuryLimitations()
    {
        // Arrange
        var profile = new UserProfile
        {
            UserId = Guid.NewGuid(),
            Name = "John Doe",
            Email = "john.doe@example.com"
        };

        var injury = new InjuryLimitation
        {
            UserProfileId = profile.Id,
            BodyPart = "Knee",
            InjuryType = InjuryType.Chronic,
            ReportedDate = DateTime.UtcNow
        };

        // Act
        profile.InjuryLimitations.Add(injury);

        // Assert
        profile.InjuryLimitations.Should().HaveCount(1);
        profile.InjuryLimitations.First().Should().Be(injury);
    }
}
