using Xunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using FitnessApp.Application.Common.Interfaces;
using FitnessApp.Domain.Entities;
using FitnessApp.Domain.Enums;
using FitnessApp.Infrastructure.Persistence;

namespace FitnessApp.IntegrationTests.Repositories;

public class WorkoutRepositoryTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public WorkoutRepositoryTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    private async Task<(TrainingPlan plan, TrainingWeek week)> CreateTestPlanWithWeekAsync(IApplicationDbContext context, Guid userId)
    {
        var plan = new TrainingPlan
        {
            UserId = userId,
            PlanName = "Test Plan",
            StartDate = DateTime.UtcNow.Date,
            EndDate = DateTime.UtcNow.Date.AddDays(4 * 7),
            TotalWeeks = 4,
            TrainingDaysPerWeek = 5,
            Status = PlanStatus.Active,
            CurrentWeek = 1
        };

        var week = new TrainingWeek
        {
            PlanId = plan.Id,
            WeekNumber = 1,
            Phase = TrainingPhase.Foundation,
            StartDate = plan.StartDate,
            EndDate = plan.StartDate.AddDays(7),
            IntensityLevel = IntensityLevel.Low
        };

        plan.TrainingWeeks.Add(week);
        context.TrainingPlans.Add(plan);
        await context.SaveChangesAsync();

        return (plan, week);
    }

    [Fact]
    public async Task GetWorkoutByIdAsync_WithExistingWorkout_ShouldReturnWorkout()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IWorkoutRepository>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Clean database
        dbContext.TrainingPlans.RemoveRange(dbContext.TrainingPlans);
        await dbContext.SaveChangesAsync();

        var userId = Guid.NewGuid();
        var (plan, week) = await CreateTestPlanWithWeekAsync(dbContext, userId);

        var workout = new Workout
        {
            WeekId = week.Id,
            DayOfWeek = WorkoutDay.Monday,
            ScheduledDate = DateTime.UtcNow.Date,
            Discipline = Discipline.Running,
            WorkoutName = "Easy Run",
            CompletionStatus = CompletionStatus.NotStarted,
            IntensityLevel = IntensityLevel.Low
        };

        await repository.CreateWorkoutAsync(workout);

        // Act
        var result = await repository.GetWorkoutByIdAsync(workout.Id);

        // Assert
        result.Should().NotBeNull();
        result!.WorkoutName.Should().Be("Easy Run");
        result.Discipline.Should().Be(Discipline.Running);
    }

    [Fact]
    public async Task UpdateWorkoutStatusAsync_ShouldUpdateStatusAndCompletedAt()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IWorkoutRepository>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Clean database
        dbContext.TrainingPlans.RemoveRange(dbContext.TrainingPlans);
        await dbContext.SaveChangesAsync();

        var userId = Guid.NewGuid();
        var (plan, week) = await CreateTestPlanWithWeekAsync(dbContext, userId);

        var workout = new Workout
        {
            WeekId = week.Id,
            DayOfWeek = WorkoutDay.Monday,
            ScheduledDate = DateTime.UtcNow.Date,
            Discipline = Discipline.Strength,
            WorkoutName = "Full Body",
            CompletionStatus = CompletionStatus.NotStarted,
            IntensityLevel = IntensityLevel.Moderate
        };

        await repository.CreateWorkoutAsync(workout);

        // Act
        var result = await repository.UpdateWorkoutStatusAsync(workout.Id, CompletionStatus.Completed);

        // Assert
        result.Should().NotBeNull();
        result.CompletionStatus.Should().Be(CompletionStatus.Completed);
        result.CompletedAt.Should().NotBeNull();
        result.CompletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        // Verify in database
        var dbWorkout = await repository.GetWorkoutByIdAsync(workout.Id);
        dbWorkout!.CompletionStatus.Should().Be(CompletionStatus.Completed);
        dbWorkout.CompletedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task GetTodaysWorkoutAsync_ShouldReturnWorkoutScheduledForToday()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IWorkoutRepository>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Clean database
        dbContext.TrainingPlans.RemoveRange(dbContext.TrainingPlans);
        await dbContext.SaveChangesAsync();

        var userId = Guid.NewGuid();
        var (plan, week) = await CreateTestPlanWithWeekAsync(dbContext, userId);

        var today = DateTime.UtcNow.Date;
        var todayWorkout = new Workout
        {
            WeekId = week.Id,
            DayOfWeek = WorkoutDay.Monday,
            ScheduledDate = today,
            Discipline = Discipline.HYROX,
            WorkoutName = "Today's Workout",
            CompletionStatus = CompletionStatus.NotStarted,
            IntensityLevel = IntensityLevel.High
        };

        var tomorrowWorkout = new Workout
        {
            WeekId = week.Id,
            DayOfWeek = WorkoutDay.Tuesday,
            ScheduledDate = today.AddDays(1),
            Discipline = Discipline.Running,
            WorkoutName = "Tomorrow's Workout",
            CompletionStatus = CompletionStatus.NotStarted,
            IntensityLevel = IntensityLevel.Low
        };

        await repository.CreateWorkoutAsync(todayWorkout);
        await repository.CreateWorkoutAsync(tomorrowWorkout);

        // Act
        var result = await repository.GetTodaysWorkoutAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result!.WorkoutName.Should().Be("Today's Workout");
        result.ScheduledDate.Date.Should().Be(today);
    }

    [Fact]
    public async Task GetUpcomingWorkoutsAsync_ShouldReturnWorkoutsInRange()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IWorkoutRepository>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Clean database
        dbContext.TrainingPlans.RemoveRange(dbContext.TrainingPlans);
        await dbContext.SaveChangesAsync();

        var userId = Guid.NewGuid();
        var (plan, week) = await CreateTestPlanWithWeekAsync(dbContext, userId);

        var today = DateTime.UtcNow.Date;

        // Create workouts for next 7 days
        for (int i = 0; i < 7; i++)
        {
            var workout = new Workout
            {
                WeekId = week.Id,
                DayOfWeek = (WorkoutDay)(i % 7),
                ScheduledDate = today.AddDays(i),
                Discipline = Discipline.Running,
                WorkoutName = $"Workout Day {i + 1}",
                CompletionStatus = CompletionStatus.NotStarted,
                IntensityLevel = IntensityLevel.Low
            };
            await repository.CreateWorkoutAsync(workout);
        }

        // Act
        var result = await repository.GetUpcomingWorkoutsAsync(userId, 5);

        // Assert
        result.Should().HaveCount(6); // Days 0-5 inclusive
        result.First().WorkoutName.Should().Be("Workout Day 1");
    }

    [Fact]
    public async Task GetWorkoutsByDateRangeAsync_ShouldReturnWorkoutsInRange()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IWorkoutRepository>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Clean database
        dbContext.TrainingPlans.RemoveRange(dbContext.TrainingPlans);
        await dbContext.SaveChangesAsync();

        var userId = Guid.NewGuid();
        var (plan, week) = await CreateTestPlanWithWeekAsync(dbContext, userId);

        var startDate = DateTime.UtcNow.Date;
        var midDate = startDate.AddDays(3);
        var endDate = startDate.AddDays(7);

        await repository.CreateWorkoutAsync(new Workout
        {
            WeekId = week.Id,
            DayOfWeek = WorkoutDay.Monday,
            ScheduledDate = startDate,
            Discipline = Discipline.Running,
            WorkoutName = "Start Workout",
            CompletionStatus = CompletionStatus.NotStarted,
            IntensityLevel = IntensityLevel.Low
        });

        await repository.CreateWorkoutAsync(new Workout
        {
            WeekId = week.Id,
            DayOfWeek = WorkoutDay.Wednesday,
            ScheduledDate = midDate,
            Discipline = Discipline.Strength,
            WorkoutName = "Mid Workout",
            CompletionStatus = CompletionStatus.NotStarted,
            IntensityLevel = IntensityLevel.Moderate
        });

        await repository.CreateWorkoutAsync(new Workout
        {
            WeekId = week.Id,
            DayOfWeek = WorkoutDay.Sunday,
            ScheduledDate = endDate,
            Discipline = Discipline.HYROX,
            WorkoutName = "End Workout",
            CompletionStatus = CompletionStatus.NotStarted,
            IntensityLevel = IntensityLevel.High
        });

        // Act
        var result = await repository.GetWorkoutsByDateRangeAsync(userId, startDate, endDate);

        // Assert
        result.Should().HaveCount(3);
        result.Should().Contain(w => w.WorkoutName == "Start Workout");
        result.Should().Contain(w => w.WorkoutName == "Mid Workout");
        result.Should().Contain(w => w.WorkoutName == "End Workout");
        result.Should().BeInAscendingOrder(w => w.ScheduledDate);
    }

    [Fact]
    public async Task GetWorkoutWithExercisesAsync_ShouldIncludeAllExercises()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IWorkoutRepository>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Clean database
        dbContext.TrainingPlans.RemoveRange(dbContext.TrainingPlans);
        await dbContext.SaveChangesAsync();

        var userId = Guid.NewGuid();
        var (plan, week) = await CreateTestPlanWithWeekAsync(dbContext, userId);

        var workout = new Workout
        {
            WeekId = week.Id,
            DayOfWeek = WorkoutDay.Monday,
            ScheduledDate = DateTime.UtcNow.Date,
            Discipline = Discipline.Strength,
            WorkoutName = "Full Body",
            CompletionStatus = CompletionStatus.NotStarted,
            IntensityLevel = IntensityLevel.Moderate
        };

        // Create an exercise to link to
        var exercise = new Exercise
        {
            Name = "Squat",
            Description = "Barbell Back Squat",
            PrimaryDiscipline = Discipline.Strength,
            DifficultyLevel = DifficultyLevel.Intermediate,
            IntensityLevel = IntensityLevel.High
        };
        dbContext.Exercises.Add(exercise);
        await dbContext.SaveChangesAsync();

        var workoutExercise = new WorkoutExercise
        {
            WorkoutId = workout.Id,
            ExerciseId = exercise.Id,
            OrderInWorkout = 1,
            Sets = 3,
            Reps = 10,
            RestPeriod = 120,
            IntensityGuidance = "70% max"
        };

        workout.WorkoutExercises.Add(workoutExercise);
        await repository.CreateWorkoutAsync(workout);

        // Act
        var result = await repository.GetWorkoutWithExercisesAsync(workout.Id);

        // Assert
        result.Should().NotBeNull();
        result!.WorkoutExercises.Should().HaveCount(1);
        result.WorkoutExercises.First().Exercise.Should().NotBeNull();
        result.WorkoutExercises.First().Exercise!.Name.Should().Be("Squat");
        result.WorkoutExercises.First().Sets.Should().Be(3);
        result.WorkoutExercises.First().Reps.Should().Be(10);
    }

    [Fact]
    public async Task UpdateWorkoutAsync_ShouldModifyExistingWorkout()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IWorkoutRepository>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Clean database
        dbContext.TrainingPlans.RemoveRange(dbContext.TrainingPlans);
        await dbContext.SaveChangesAsync();

        var userId = Guid.NewGuid();
        var (plan, week) = await CreateTestPlanWithWeekAsync(dbContext, userId);

        var workout = new Workout
        {
            WeekId = week.Id,
            DayOfWeek = WorkoutDay.Monday,
            ScheduledDate = DateTime.UtcNow.Date,
            Discipline = Discipline.Running,
            WorkoutName = "Original Name",
            EstimatedDuration = 30,
            CompletionStatus = CompletionStatus.NotStarted,
            IntensityLevel = IntensityLevel.Low
        };

        await repository.CreateWorkoutAsync(workout);

        // Act
        workout.WorkoutName = "Updated Name";
        workout.EstimatedDuration = 45;
        var result = await repository.UpdateWorkoutAsync(workout);

        // Assert
        result.Should().NotBeNull();
        result.WorkoutName.Should().Be("Updated Name");
        result.EstimatedDuration.Should().Be(45);

        // Verify in database
        var dbWorkout = await repository.GetWorkoutByIdAsync(workout.Id);
        dbWorkout!.WorkoutName.Should().Be("Updated Name");
        dbWorkout.EstimatedDuration.Should().Be(45);
    }
}
