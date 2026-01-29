using Xunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using FitnessApp.Application.Common.Interfaces;
using FitnessApp.Domain.Entities;
using FitnessApp.Domain.Enums;
using FitnessApp.Domain.ValueObjects;
using FitnessApp.Infrastructure.Persistence;

namespace FitnessApp.IntegrationTests.Repositories;

public class UserProfileRepositoryTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public UserProfileRepositoryTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreateAsync_WithValidProfile_ShouldPersistToDatabase()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserProfileRepository>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Clean database
        dbContext.UserProfiles.RemoveRange(dbContext.UserProfiles);
        await dbContext.SaveChangesAsync();

        var profile = new UserProfile
        {
            UserId = Guid.NewGuid(),
            Name = "John Doe",
            Email = "john.doe@example.com",
            HyroxLevel = FitnessLevel.Intermediate,
            RunningLevel = FitnessLevel.Advanced,
            StrengthLevel = FitnessLevel.Beginner
        };

        // Act
        var result = await repository.CreateAsync(profile);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        // Verify in database
        var dbProfile = await repository.GetByUserIdAsync(profile.UserId);
        dbProfile.Should().NotBeNull();
        dbProfile!.Name.Should().Be("John Doe");
        dbProfile.Email.Should().Be("john.doe@example.com");
        dbProfile.HyroxLevel.Should().Be(FitnessLevel.Intermediate);
    }

    [Fact]
    public async Task GetByUserIdAsync_WithExistingProfile_ShouldReturnProfile()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserProfileRepository>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Clean database
        dbContext.UserProfiles.RemoveRange(dbContext.UserProfiles);
        await dbContext.SaveChangesAsync();

        var userId = Guid.NewGuid();
        var profile = new UserProfile
        {
            UserId = userId,
            Name = "Test User",
            Email = "test@example.com"
        };

        await repository.CreateAsync(profile);

        // Act
        var result = await repository.GetByUserIdAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result!.UserId.Should().Be(userId);
        result.Name.Should().Be("Test User");
    }

    [Fact]
    public async Task GetProfileWithGoalsAsync_ShouldIncludeTrainingGoals()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserProfileRepository>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Clean database
        dbContext.UserProfiles.RemoveRange(dbContext.UserProfiles);
        await dbContext.SaveChangesAsync();

        var userId = Guid.NewGuid();
        var profile = new UserProfile
        {
            UserId = userId,
            Name = "Test User",
            Email = "test@example.com"
        };

        await repository.CreateAsync(profile);

        // Add training goals
        var goal1 = new TrainingGoal
        {
            UserProfileId = profile.Id,
            Description = "Complete HYROX race",
            GoalType = GoalType.HyroxRace,
            TargetDate = DateTime.UtcNow.AddMonths(3)
        };

        var goal2 = new TrainingGoal
        {
            UserProfileId = profile.Id,
            Description = "Run 10K under 45 minutes",
            GoalType = GoalType.RunningDistance,
            TargetDate = DateTime.UtcNow.AddMonths(2)
        };

        dbContext.TrainingGoals.Add(goal1);
        dbContext.TrainingGoals.Add(goal2);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await repository.GetProfileWithGoalsAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result!.TrainingGoals.Should().HaveCount(2);
        result.TrainingGoals.Should().Contain(g => g.Description == "Complete HYROX race");
        result.TrainingGoals.Should().Contain(g => g.Description == "Run 10K under 45 minutes");
    }

    [Fact]
    public async Task GetProfileWithInjuriesAsync_ShouldIncludeInjuryLimitations()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserProfileRepository>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Clean database
        dbContext.UserProfiles.RemoveRange(dbContext.UserProfiles);
        await dbContext.SaveChangesAsync();

        var userId = Guid.NewGuid();
        var profile = new UserProfile
        {
            UserId = userId,
            Name = "Test User",
            Email = "test@example.com"
        };

        await repository.CreateAsync(profile);

        // Add injury limitations
        var injury = new InjuryLimitation
        {
            UserProfileId = profile.Id,
            BodyPart = "Knee",
            InjuryType = InjuryType.Chronic,
            ReportedDate = DateTime.UtcNow,
            Status = InjuryStatus.Active,
            MovementRestrictions = "Avoid deep squats"
        };

        dbContext.InjuryLimitations.Add(injury);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await repository.GetProfileWithInjuriesAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result!.InjuryLimitations.Should().HaveCount(1);
        result.InjuryLimitations.First().BodyPart.Should().Be("Knee");
        result.InjuryLimitations.First().InjuryType.Should().Be(InjuryType.Chronic);
    }

    [Fact]
    public async Task UpdateAsync_WithValidProfile_ShouldUpdateInDatabase()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserProfileRepository>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Clean database
        dbContext.UserProfiles.RemoveRange(dbContext.UserProfiles);
        await dbContext.SaveChangesAsync();

        var userId = Guid.NewGuid();
        var profile = new UserProfile
        {
            UserId = userId,
            Name = "Old Name",
            Email = "old@example.com",
            HyroxLevel = FitnessLevel.Beginner
        };

        await repository.CreateAsync(profile);

        // Act
        profile.Name = "New Name";
        profile.Email = "new@example.com";
        profile.HyroxLevel = FitnessLevel.Advanced;

        await repository.UpdateAsync(profile);

        // Assert
        var updated = await repository.GetByUserIdAsync(userId);
        updated.Should().NotBeNull();
        updated!.Name.Should().Be("New Name");
        updated.Email.Should().Be("new@example.com");
        updated.HyroxLevel.Should().Be(FitnessLevel.Advanced);
        updated.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task DeleteAsync_WithExistingProfile_ShouldRemoveFromDatabase()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserProfileRepository>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Clean database
        dbContext.UserProfiles.RemoveRange(dbContext.UserProfiles);
        await dbContext.SaveChangesAsync();

        var userId = Guid.NewGuid();
        var profile = new UserProfile
        {
            UserId = userId,
            Name = "Delete Me",
            Email = "delete@example.com"
        };

        await repository.CreateAsync(profile);

        // Act
        var result = await repository.DeleteAsync(userId);

        // Assert
        result.Should().BeTrue();

        var deleted = await repository.GetByUserIdAsync(userId);
        deleted.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WithCascade_ShouldDeleteRelatedEntities()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserProfileRepository>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Clean database
        dbContext.UserProfiles.RemoveRange(dbContext.UserProfiles);
        await dbContext.SaveChangesAsync();

        var userId = Guid.NewGuid();
        var profile = new UserProfile
        {
            UserId = userId,
            Name = "Test User",
            Email = "test@example.com"
        };

        await repository.CreateAsync(profile);

        // Add related entities
        var goal = new TrainingGoal
        {
            UserProfileId = profile.Id,
            Description = "Test Goal",
            GoalType = GoalType.GeneralFitness
        };

        var injury = new InjuryLimitation
        {
            UserProfileId = profile.Id,
            BodyPart = "Ankle",
            InjuryType = InjuryType.Acute,
            ReportedDate = DateTime.UtcNow
        };

        var background = new TrainingBackground
        {
            UserProfileId = profile.Id,
            HasStructuredTrainingExperience = true,
            PreviousTrainingDetails = "CrossFit for 2 years"
        };

        dbContext.TrainingGoals.Add(goal);
        dbContext.InjuryLimitations.Add(injury);
        dbContext.TrainingBackgrounds.Add(background);
        await dbContext.SaveChangesAsync();

        // Act
        await repository.DeleteAsync(userId);

        // Assert
        var deletedProfile = await repository.GetByUserIdAsync(userId);
        deletedProfile.Should().BeNull();

        // Verify related entities are also deleted (cascade)
        var remainingGoals = dbContext.TrainingGoals.Where(g => g.UserProfileId == profile.Id).ToList();
        var remainingInjuries = dbContext.InjuryLimitations.Where(i => i.UserProfileId == profile.Id).ToList();
        var remainingBackground = dbContext.TrainingBackgrounds.Where(b => b.UserProfileId == profile.Id).ToList();

        remainingGoals.Should().BeEmpty();
        remainingInjuries.Should().BeEmpty();
        remainingBackground.Should().BeEmpty();
    }

    [Fact]
    public async Task CreateAsync_WithScheduleAvailability_ShouldPersistOwnedEntity()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserProfileRepository>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Clean database
        dbContext.UserProfiles.RemoveRange(dbContext.UserProfiles);
        await dbContext.SaveChangesAsync();

        var profile = new UserProfile
        {
            UserId = Guid.NewGuid(),
            Name = "Test User",
            Email = "test@example.com",
            ScheduleAvailability = new ScheduleAvailability
            {
                Monday = true,
                Wednesday = true,
                Friday = true,
                MinimumSessionsPerWeek = 2,
                MaximumSessionsPerWeek = 3
            }
        };

        // Act
        await repository.CreateAsync(profile);

        // Assert
        var retrieved = await repository.GetByUserIdAsync(profile.UserId);
        retrieved.Should().NotBeNull();
        retrieved!.ScheduleAvailability.Should().NotBeNull();
        retrieved.ScheduleAvailability!.Monday.Should().BeTrue();
        retrieved.ScheduleAvailability.Wednesday.Should().BeTrue();
        retrieved.ScheduleAvailability.Friday.Should().BeTrue();
        retrieved.ScheduleAvailability.MinimumSessionsPerWeek.Should().Be(2);
        retrieved.ScheduleAvailability.MaximumSessionsPerWeek.Should().Be(3);
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateUserId_ShouldThrowException()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserProfileRepository>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Clean database
        dbContext.UserProfiles.RemoveRange(dbContext.UserProfiles);
        await dbContext.SaveChangesAsync();

        var userId = Guid.NewGuid();
        var profile1 = new UserProfile
        {
            UserId = userId,
            Name = "First Profile",
            Email = "first@example.com"
        };

        await repository.CreateAsync(profile1);

        var profile2 = new UserProfile
        {
            UserId = userId,
            Name = "Second Profile",
            Email = "second@example.com"
        };

        // Act
        Func<Task> act = async () => await repository.CreateAsync(profile2);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}
