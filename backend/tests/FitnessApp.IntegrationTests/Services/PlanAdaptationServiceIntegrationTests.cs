using Xunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using FitnessApp.Application.Common.Interfaces;
using FitnessApp.Application.DTOs;
using FitnessApp.Domain.Entities;
using FitnessApp.Domain.Enums;
using FitnessApp.Infrastructure.Persistence;

namespace FitnessApp.IntegrationTests.Services;

public class PlanAdaptationServiceIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public PlanAdaptationServiceIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task AdaptForMissedWorkoutsAsync_ShouldPersistAdaptationToDatabase()
    {
        using var scope = _factory.Services.CreateScope();
        var adaptationService = scope.ServiceProvider.GetRequiredService<IPlanAdaptationService>();
        var planRepository = scope.ServiceProvider.GetRequiredService<ITrainingPlanRepository>();
        var adaptationRepository = scope.ServiceProvider.GetRequiredService<IPlanAdaptationRepository>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await CleanDatabaseAsync(dbContext);

        var userId = Guid.NewGuid();
        var plan = await CreateTestPlanWithWorkouts(planRepository, userId);
        var missedWorkoutIds = plan.TrainingWeeks
            .SelectMany(w => w.Workouts)
            .Take(2)
            .Select(w => w.Id)
            .ToList();

        var result = await adaptationService.AdaptForMissedWorkoutsAsync(userId, missedWorkoutIds);

        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.AdaptationId.Should().NotBeEmpty();

        var savedAdaptation = await adaptationRepository.GetByIdAsync(result.AdaptationId);
        savedAdaptation.Should().NotBeNull();
        savedAdaptation!.PlanId.Should().Be(plan.Id);
        savedAdaptation.Trigger.Should().Be(AdaptationTrigger.MissedWorkouts);
    }

    private async Task CleanDatabaseAsync(ApplicationDbContext dbContext)
    {
        dbContext.PlanAdaptations.RemoveRange(dbContext.PlanAdaptations);
        dbContext.WorkoutExercises.RemoveRange(dbContext.WorkoutExercises);
        dbContext.Workouts.RemoveRange(dbContext.Workouts);
        dbContext.TrainingWeeks.RemoveRange(dbContext.TrainingWeeks);
        dbContext.PlanMetadatas.RemoveRange(dbContext.PlanMetadatas);
        dbContext.TrainingPlans.RemoveRange(dbContext.TrainingPlans);
        await dbContext.SaveChangesAsync();
    }

    private async Task<TrainingPlan> CreateTestPlanWithWorkouts(
        ITrainingPlanRepository planRepository, 
        Guid userId,
        IntensityLevel intensity = IntensityLevel.Moderate)
    {
        var plan = new TrainingPlan
        {
            UserId = userId,
            PlanName = "Integration Test Plan",
            StartDate = DateTime.UtcNow.Date,
            EndDate = DateTime.UtcNow.Date.AddDays(56),
            TotalWeeks = 8,
            TrainingDaysPerWeek = 4,
            Status = PlanStatus.Active,
            CurrentWeek = 1
        };

        var createdPlan = await planRepository.CreatePlanAsync(plan);

        var week = new TrainingWeek
        {
            PlanId = createdPlan.Id,
            WeekNumber = 1,
            Phase = TrainingPhase.Foundation,
            StartDate = DateTime.UtcNow.Date,
            EndDate = DateTime.UtcNow.Date.AddDays(7)
        };

        createdPlan.TrainingWeeks = new List<TrainingWeek> { week };

        var workouts = new List<Workout>();
        for (int i = 0; i < 4; i++)
        {
            workouts.Add(new Workout
            {
                WeekId = week.Id,
                DayOfWeek = (WorkoutDay)(i + 1),
                ScheduledDate = DateTime.UtcNow.Date.AddDays(i + 1),
                Discipline = Discipline.Running,
                WorkoutName = $"Test Workout {i + 1}",
                Description = $"Integration test workout {i + 1}",
                IntensityLevel = intensity,
                IsKeyWorkout = i % 2 == 0,
                CompletionStatus = CompletionStatus.NotStarted,
                TrainingWeek = week
            });
        }
        week.Workouts = workouts;

        await planRepository.UpdatePlanAsync(createdPlan);
        return (await planRepository.GetPlanWithDetailsAsync(createdPlan.Id))!;
    }
}
