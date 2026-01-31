using Xunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using FitnessApp.Application.Common.Interfaces;
using FitnessApp.Domain.Entities;
using FitnessApp.Domain.Enums;
using FitnessApp.Infrastructure.Persistence;

namespace FitnessApp.IntegrationTests.Repositories;

public class TrainingPlanRepositoryTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public TrainingPlanRepositoryTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreatePlanAsync_WithValidPlan_ShouldPersistToDatabase()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<ITrainingPlanRepository>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Clean database
        dbContext.TrainingPlans.RemoveRange(dbContext.TrainingPlans);
        await dbContext.SaveChangesAsync();

        var userId = Guid.NewGuid();
        var plan = new TrainingPlan
        {
            UserId = userId,
            PlanName = "12-Week HYROX Preparation",
            StartDate = DateTime.UtcNow.Date,
            EndDate = DateTime.UtcNow.Date.AddDays(12 * 7),
            TotalWeeks = 12,
            TrainingDaysPerWeek = 5,
            Status = PlanStatus.Active,
            CurrentWeek = 1
        };

        // Act
        var result = await repository.CreatePlanAsync(plan);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        // Verify in database
        var dbPlan = await repository.GetByIdAsync(result.Id);
        dbPlan.Should().NotBeNull();
        dbPlan!.PlanName.Should().Be("12-Week HYROX Preparation");
        dbPlan.TotalWeeks.Should().Be(12);
    }

    [Fact]
    public async Task GetActivePlanByUserIdAsync_WithActivePlan_ShouldReturnPlan()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<ITrainingPlanRepository>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Clean database
        dbContext.TrainingPlans.RemoveRange(dbContext.TrainingPlans);
        await dbContext.SaveChangesAsync();

        var userId = Guid.NewGuid();
        var plan = new TrainingPlan
        {
            UserId = userId,
            PlanName = "Test Plan",
            StartDate = DateTime.UtcNow.Date,
            EndDate = DateTime.UtcNow.Date.AddDays(8 * 7),
            TotalWeeks = 8,
            TrainingDaysPerWeek = 4,
            Status = PlanStatus.Active,
            CurrentWeek = 1
        };

        await repository.CreatePlanAsync(plan);

        // Act
        var result = await repository.GetActivePlanByUserIdAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result!.UserId.Should().Be(userId);
        result.Status.Should().Be(PlanStatus.Active);
    }

    [Fact]
    public async Task GetPlanWithWeeksAsync_ShouldIncludeAllWeeksAndWorkouts()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<ITrainingPlanRepository>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Clean database
        dbContext.TrainingPlans.RemoveRange(dbContext.TrainingPlans);
        await dbContext.SaveChangesAsync();

        var userId = Guid.NewGuid();
        var plan = new TrainingPlan
        {
            UserId = userId,
            PlanName = "Complete Plan",
            StartDate = DateTime.UtcNow.Date,
            EndDate = DateTime.UtcNow.Date.AddDays(4 * 7),
            TotalWeeks = 4,
            TrainingDaysPerWeek = 3,
            Status = PlanStatus.Active,
            CurrentWeek = 1
        };

        var week1 = new TrainingWeek
        {
            PlanId = plan.Id,
            WeekNumber = 1,
            Phase = TrainingPhase.Foundation,
            StartDate = plan.StartDate,
            EndDate = plan.StartDate.AddDays(7),
            IntensityLevel = IntensityLevel.Low
        };

        var workout1 = new Workout
        {
            WeekId = week1.Id,
            DayOfWeek = WorkoutDay.Monday,
            ScheduledDate = week1.StartDate,
            Discipline = Discipline.Running,
            WorkoutName = "Easy Run",
            CompletionStatus = CompletionStatus.NotStarted,
            IntensityLevel = IntensityLevel.Low
        };

        week1.Workouts.Add(workout1);
        plan.TrainingWeeks.Add(week1);

        await repository.CreatePlanAsync(plan);

        // Act
        var result = await repository.GetPlanWithWeeksAsync(plan.Id);

        // Assert
        result.Should().NotBeNull();
        result!.TrainingWeeks.Should().HaveCount(1);
        result.TrainingWeeks.First().Workouts.Should().HaveCount(1);
        result.TrainingWeeks.First().Workouts.First().WorkoutName.Should().Be("Easy Run");
    }

    [Fact]
    public async Task DeletePlanAsync_ShouldSoftDeletePlan()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<ITrainingPlanRepository>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Clean database
        dbContext.TrainingPlans.RemoveRange(dbContext.TrainingPlans);
        await dbContext.SaveChangesAsync();

        var userId = Guid.NewGuid();
        var plan = new TrainingPlan
        {
            UserId = userId,
            PlanName = "Plan to Delete",
            StartDate = DateTime.UtcNow.Date,
            EndDate = DateTime.UtcNow.Date.AddDays(8 * 7),
            TotalWeeks = 8,
            TrainingDaysPerWeek = 4,
            Status = PlanStatus.Active,
            CurrentWeek = 1
        };

        await repository.CreatePlanAsync(plan);

        // Act
        var result = await repository.DeletePlanAsync(plan.Id);

        // Assert
        result.Should().BeTrue();

        // Verify it's not returned in normal queries (due to query filter)
        var deletedPlan = await repository.GetByIdAsync(plan.Id);
        deletedPlan.Should().BeNull();

        // Verify it still exists but is marked as deleted
        var plans = await repository.GetPlansByUserIdAsync(userId, includeDeleted: true);
        plans.Should().HaveCount(1);
        plans.First().IsDeleted.Should().BeTrue();
    }

    [Fact]
    public async Task UpdatePlanAsync_ShouldModifyExistingPlan()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<ITrainingPlanRepository>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Clean database
        dbContext.TrainingPlans.RemoveRange(dbContext.TrainingPlans);
        await dbContext.SaveChangesAsync();

        var userId = Guid.NewGuid();
        var plan = new TrainingPlan
        {
            UserId = userId,
            PlanName = "Original Plan",
            StartDate = DateTime.UtcNow.Date,
            EndDate = DateTime.UtcNow.Date.AddDays(8 * 7),
            TotalWeeks = 8,
            TrainingDaysPerWeek = 4,
            Status = PlanStatus.Active,
            CurrentWeek = 1
        };

        await repository.CreatePlanAsync(plan);

        // Act
        plan.CurrentWeek = 2;
        plan.PlanName = "Updated Plan";
        var result = await repository.UpdatePlanAsync(plan);

        // Assert
        result.Should().NotBeNull();
        result.CurrentWeek.Should().Be(2);
        result.PlanName.Should().Be("Updated Plan");

        // Verify in database
        var dbPlan = await repository.GetByIdAsync(plan.Id);
        dbPlan!.CurrentWeek.Should().Be(2);
        dbPlan.PlanName.Should().Be("Updated Plan");
    }

    [Fact]
    public async Task GetCurrentWeekWorkoutsAsync_ShouldReturnWorkoutsForCurrentWeek()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<ITrainingPlanRepository>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Clean database
        dbContext.TrainingPlans.RemoveRange(dbContext.TrainingPlans);
        await dbContext.SaveChangesAsync();

        var userId = Guid.NewGuid();
        var plan = new TrainingPlan
        {
            UserId = userId,
            PlanName = "Test Plan",
            StartDate = DateTime.UtcNow.Date,
            EndDate = DateTime.UtcNow.Date.AddDays(4 * 7),
            TotalWeeks = 4,
            TrainingDaysPerWeek = 3,
            Status = PlanStatus.Active,
            CurrentWeek = 2
        };

        var week1 = new TrainingWeek
        {
            PlanId = plan.Id,
            WeekNumber = 1,
            Phase = TrainingPhase.Foundation,
            StartDate = plan.StartDate,
            EndDate = plan.StartDate.AddDays(7),
            IntensityLevel = IntensityLevel.Low
        };

        var week2 = new TrainingWeek
        {
            PlanId = plan.Id,
            WeekNumber = 2,
            Phase = TrainingPhase.Build,
            StartDate = week1.EndDate,
            EndDate = week1.EndDate.AddDays(7),
            IntensityLevel = IntensityLevel.Moderate
        };

        week1.Workouts.Add(new Workout
        {
            WeekId = week1.Id,
            DayOfWeek = WorkoutDay.Monday,
            ScheduledDate = week1.StartDate,
            Discipline = Discipline.Running,
            WorkoutName = "Week 1 Run",
            CompletionStatus = CompletionStatus.NotStarted,
            IntensityLevel = IntensityLevel.Low
        });

        week2.Workouts.Add(new Workout
        {
            WeekId = week2.Id,
            DayOfWeek = WorkoutDay.Monday,
            ScheduledDate = week2.StartDate,
            Discipline = Discipline.Running,
            WorkoutName = "Week 2 Run",
            CompletionStatus = CompletionStatus.NotStarted,
            IntensityLevel = IntensityLevel.Moderate
        });

        plan.TrainingWeeks.Add(week1);
        plan.TrainingWeeks.Add(week2);

        await repository.CreatePlanAsync(plan);

        // Act
        var result = await repository.GetCurrentWeekWorkoutsAsync(userId);

        // Assert
        result.Should().HaveCount(1);
        result.First().WorkoutName.Should().Be("Week 2 Run");
    }
}
