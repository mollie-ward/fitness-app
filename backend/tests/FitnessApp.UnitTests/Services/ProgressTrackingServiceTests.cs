using Xunit;
using FluentAssertions;
using Moq;
using Microsoft.Extensions.Logging;
using FitnessApp.Application.Services;
using FitnessApp.Application.Common.Interfaces;
using FitnessApp.Application.DTOs;
using FitnessApp.Domain.Entities;
using FitnessApp.Domain.Enums;

namespace FitnessApp.UnitTests.Services;

/// <summary>
/// Unit tests for ProgressTrackingService
/// Tests completion tracking, streak calculation, and statistics
/// </summary>
public class ProgressTrackingServiceTests
{
    private readonly Mock<IWorkoutRepository> _workoutRepositoryMock;
    private readonly Mock<ITrainingPlanRepository> _planRepositoryMock;
    private readonly Mock<ICompletionHistoryRepository> _completionHistoryRepositoryMock;
    private readonly Mock<IUserStreakRepository> _userStreakRepositoryMock;
    private readonly Mock<ILogger<ProgressTrackingService>> _loggerMock;
    private readonly ProgressTrackingService _service;

    public ProgressTrackingServiceTests()
    {
        _workoutRepositoryMock = new Mock<IWorkoutRepository>();
        _planRepositoryMock = new Mock<ITrainingPlanRepository>();
        _completionHistoryRepositoryMock = new Mock<ICompletionHistoryRepository>();
        _userStreakRepositoryMock = new Mock<IUserStreakRepository>();
        _loggerMock = new Mock<ILogger<ProgressTrackingService>>();

        _service = new ProgressTrackingService(
            _workoutRepositoryMock.Object,
            _planRepositoryMock.Object,
            _completionHistoryRepositoryMock.Object,
            _userStreakRepositoryMock.Object,
            _loggerMock.Object);
    }

    #region MarkWorkoutCompleteAsync Tests

    [Fact]
    public async Task MarkWorkoutCompleteAsync_WithValidWorkout_ShouldMarkComplete()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var workoutId = Guid.NewGuid();
        var planId = Guid.NewGuid();
        var completionDto = new WorkoutCompletionDto
        {
            CompletedAt = DateTime.UtcNow,
            Duration = 45,
            Notes = "Great workout!"
        };

        var workout = CreateValidWorkout(workoutId, planId);
        var plan = CreateValidPlan(planId, userId);
        var userStreak = new UserStreak
        {
            UserId = userId,
            CurrentStreak = 0,
            LongestStreak = 0,
            CurrentWeeklyStreak = 0,
            LongestWeeklyStreak = 0
        };

        SetupWorkoutMocks(workoutId, workout, plan, userStreak);

        // Act
        var result = await _service.MarkWorkoutCompleteAsync(workoutId, userId, completionDto);

        // Assert
        result.Should().NotBeNull();
        _workoutRepositoryMock.Verify(x => x.UpdateAsync(It.Is<Workout>(w => 
            w.CompletionStatus == CompletionStatus.Completed &&
            w.CompletedAt == completionDto.CompletedAt), 
            It.IsAny<CancellationToken>()), Times.Once);
        _completionHistoryRepositoryMock.Verify(x => x.AddAsync(
            It.Is<CompletionHistory>(ch => 
                ch.WorkoutId == workoutId && 
                ch.UserId == userId &&
                ch.Duration == 45 &&
                ch.Notes == "Great workout!"), 
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task MarkWorkoutCompleteAsync_WithFutureDate_ShouldThrowException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var workoutId = Guid.NewGuid();
        var completionDto = new WorkoutCompletionDto
        {
            CompletedAt = DateTime.UtcNow.AddDays(1), // Future date
            Duration = 45
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () => 
            await _service.MarkWorkoutCompleteAsync(workoutId, userId, completionDto));
    }

    [Fact]
    public async Task MarkWorkoutCompleteAsync_WorkoutNotFound_ShouldThrowException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var workoutId = Guid.NewGuid();
        var completionDto = new WorkoutCompletionDto
        {
            CompletedAt = DateTime.UtcNow,
            Duration = 45
        };

        _workoutRepositoryMock
            .Setup(x => x.GetWorkoutWithExercisesAsync(workoutId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Workout?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(async () => 
            await _service.MarkWorkoutCompleteAsync(workoutId, userId, completionDto));
    }

    [Fact]
    public async Task MarkWorkoutCompleteAsync_UserDoesNotOwnWorkout_ShouldThrowException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var wrongUserId = Guid.NewGuid();
        var workoutId = Guid.NewGuid();
        var planId = Guid.NewGuid();
        var completionDto = new WorkoutCompletionDto
        {
            CompletedAt = DateTime.UtcNow,
            Duration = 45
        };

        var workout = CreateValidWorkout(workoutId, planId);
        var plan = CreateValidPlan(planId, wrongUserId); // Different user

        _workoutRepositoryMock
            .Setup(x => x.GetWorkoutWithExercisesAsync(workoutId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(workout);
        _planRepositoryMock
            .Setup(x => x.GetByIdAsync(planId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(plan);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => 
            await _service.MarkWorkoutCompleteAsync(workoutId, userId, completionDto));
    }

    [Fact]
    public async Task MarkWorkoutCompleteAsync_AlreadyCompleted_ShouldThrowException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var workoutId = Guid.NewGuid();
        var planId = Guid.NewGuid();
        var completionDto = new WorkoutCompletionDto
        {
            CompletedAt = DateTime.UtcNow,
            Duration = 45
        };

        var workout = CreateValidWorkout(workoutId, planId);
        workout.CompletionStatus = CompletionStatus.Completed; // Already completed
        var plan = CreateValidPlan(planId, userId);

        _workoutRepositoryMock
            .Setup(x => x.GetWorkoutWithExercisesAsync(workoutId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(workout);
        _planRepositoryMock
            .Setup(x => x.GetByIdAsync(planId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(plan);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () => 
            await _service.MarkWorkoutCompleteAsync(workoutId, userId, completionDto));
    }

    [Fact]
    public async Task MarkWorkoutCompleteAsync_FutureScheduledWorkout_ShouldThrowException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var workoutId = Guid.NewGuid();
        var planId = Guid.NewGuid();
        var completionDto = new WorkoutCompletionDto
        {
            CompletedAt = DateTime.UtcNow,
            Duration = 45
        };

        var workout = CreateValidWorkout(workoutId, planId);
        workout.ScheduledDate = DateTime.UtcNow.AddDays(5); // Future scheduled date
        var plan = CreateValidPlan(planId, userId);

        _workoutRepositoryMock
            .Setup(x => x.GetWorkoutWithExercisesAsync(workoutId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(workout);
        _planRepositoryMock
            .Setup(x => x.GetByIdAsync(planId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(plan);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () => 
            await _service.MarkWorkoutCompleteAsync(workoutId, userId, completionDto));
    }

    #endregion

    #region UndoWorkoutCompletionAsync Tests

    [Fact]
    public async Task UndoWorkoutCompletionAsync_WithCompletedWorkout_ShouldUndoCompletion()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var workoutId = Guid.NewGuid();
        var planId = Guid.NewGuid();

        var workout = CreateValidWorkout(workoutId, planId);
        workout.CompletionStatus = CompletionStatus.Completed;
        workout.CompletedAt = DateTime.UtcNow.AddDays(-1);

        var plan = CreateValidPlan(planId, userId);
        var completionHistory = new CompletionHistory
        {
            WorkoutId = workoutId,
            UserId = userId,
            CompletedAt = workout.CompletedAt.Value
        };

        _workoutRepositoryMock
            .Setup(x => x.GetWorkoutWithExercisesAsync(workoutId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(workout);
        _planRepositoryMock
            .Setup(x => x.GetByIdAsync(planId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(plan);
        _completionHistoryRepositoryMock
            .Setup(x => x.GetByWorkoutIdAsync(workoutId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(completionHistory);
        _completionHistoryRepositoryMock
            .Setup(x => x.GetDistinctCompletionDatesAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DateTime>());
        _userStreakRepositoryMock
            .Setup(x => x.GetOrCreateAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UserStreak { UserId = userId, CurrentStreak = 1, LongestStreak = 1 });
        _planRepositoryMock
            .Setup(x => x.GetPlanWithDetailsAsync(planId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(plan);

        // Act
        var result = await _service.UndoWorkoutCompletionAsync(workoutId, userId);

        // Assert
        result.Should().NotBeNull();
        _workoutRepositoryMock.Verify(x => x.UpdateAsync(It.Is<Workout>(w => 
            w.CompletionStatus == CompletionStatus.NotStarted &&
            w.CompletedAt == null), 
            It.IsAny<CancellationToken>()), Times.Once);
        _completionHistoryRepositoryMock.Verify(x => x.DeleteAsync(completionHistory, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UndoWorkoutCompletionAsync_NotCompleted_ShouldThrowException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var workoutId = Guid.NewGuid();
        var planId = Guid.NewGuid();

        var workout = CreateValidWorkout(workoutId, planId);
        workout.CompletionStatus = CompletionStatus.NotStarted; // Not completed
        var plan = CreateValidPlan(planId, userId);

        _workoutRepositoryMock
            .Setup(x => x.GetWorkoutWithExercisesAsync(workoutId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(workout);
        _planRepositoryMock
            .Setup(x => x.GetByIdAsync(planId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(plan);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () => 
            await _service.UndoWorkoutCompletionAsync(workoutId, userId));
    }

    #endregion

    #region Streak Calculation Tests

    [Fact]
    public async Task GetStreakInfoAsync_NoCompletions_ShouldReturnEmptyStreak()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userStreakRepositoryMock
            .Setup(x => x.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserStreak?)null);

        // Act
        var result = await _service.GetStreakInfoAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result.CurrentStreak.Should().Be(0);
        result.LongestStreak.Should().Be(0);
        result.NextMilestone.Should().Be(7);
    }

    [Fact]
    public async Task GetStreakInfoAsync_WithActiveStreak_ShouldReturnCorrectInfo()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userStreak = new UserStreak
        {
            UserId = userId,
            CurrentStreak = 5,
            LongestStreak = 10,
            CurrentWeeklyStreak = 2,
            LongestWeeklyStreak = 3,
            LastWorkoutDate = DateTime.UtcNow.Date.AddDays(-1)
        };

        _userStreakRepositoryMock
            .Setup(x => x.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userStreak);

        // Act
        var result = await _service.GetStreakInfoAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result.CurrentStreak.Should().Be(5);
        result.LongestStreak.Should().Be(10);
        result.NextMilestone.Should().Be(7);
        result.DaysUntilNextMilestone.Should().Be(2);
    }

    #endregion

    #region Statistics Tests

    [Fact]
    public async Task GetCompletionStatsAsync_WithWorkouts_ShouldCalculateCorrectly()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var planId = Guid.NewGuid();
        var startDate = DateTime.UtcNow.Date;
        var endDate = startDate.AddDays(7);

        var plan = CreateValidPlan(planId, userId);
        var workouts = new List<Workout>
        {
            CreateValidWorkout(Guid.NewGuid(), planId),
            CreateValidWorkout(Guid.NewGuid(), planId),
            CreateValidWorkout(Guid.NewGuid(), planId),
            CreateValidWorkout(Guid.NewGuid(), planId)
        };
        
        workouts[0].CompletionStatus = CompletionStatus.Completed;
        workouts[1].CompletionStatus = CompletionStatus.Completed;
        workouts[2].CompletionStatus = CompletionStatus.NotStarted;
        workouts[3].CompletionStatus = CompletionStatus.NotStarted;

        _planRepositoryMock
            .Setup(x => x.GetActivePlansByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TrainingPlan> { plan });
        _workoutRepositoryMock
            .Setup(x => x.GetWorkoutsByPlanAndDateRangeAsync(planId, startDate, endDate, It.IsAny<CancellationToken>()))
            .ReturnsAsync(workouts);

        // Act
        var result = await _service.GetCompletionStatsAsync(userId, startDate, endDate);

        // Assert
        result.Should().NotBeNull();
        result.CompletedCount.Should().Be(2);
        result.TotalScheduled.Should().Be(4);
        result.CompletionPercentage.Should().Be(50);
    }

    [Fact]
    public async Task GetOverallStatsAsync_WithCompletions_ShouldCalculateCorrectly()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var completions = new List<CompletionHistory>
        {
            new CompletionHistory { UserId = userId, WorkoutId = Guid.NewGuid(), CompletedAt = DateTime.UtcNow.AddDays(-10) },
            new CompletionHistory { UserId = userId, WorkoutId = Guid.NewGuid(), CompletedAt = DateTime.UtcNow.AddDays(-5) },
            new CompletionHistory { UserId = userId, WorkoutId = Guid.NewGuid(), CompletedAt = DateTime.UtcNow.AddDays(-1) }
        };

        var distinctDates = new List<DateTime>
        {
            DateTime.UtcNow.Date.AddDays(-10),
            DateTime.UtcNow.Date.AddDays(-5),
            DateTime.UtcNow.Date.AddDays(-1)
        };

        _completionHistoryRepositoryMock
            .Setup(x => x.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(completions);
        _completionHistoryRepositoryMock
            .Setup(x => x.GetDistinctCompletionDatesAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(distinctDates);
        _planRepositoryMock
            .Setup(x => x.GetActivePlansByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TrainingPlan>());

        // Act
        var result = await _service.GetOverallStatsAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result.TotalTrainingDays.Should().Be(3);
        result.TotalWorkoutsCompleted.Should().Be(3);
    }

    [Fact]
    public async Task GetCompletionHistoryAsync_WithDateRange_ShouldReturnFilteredResults()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var startDate = DateTime.UtcNow.AddDays(-30);
        var endDate = DateTime.UtcNow;

        var completions = new List<CompletionHistory>
        {
            new CompletionHistory 
            { 
                UserId = userId, 
                WorkoutId = Guid.NewGuid(), 
                CompletedAt = DateTime.UtcNow.AddDays(-10),
                Workout = new Workout 
                { 
                    WorkoutName = "Test Workout",
                    Discipline = Discipline.Running,
                    WeekId = Guid.NewGuid(),
                    DayOfWeek = WorkoutDay.Monday,
                    ScheduledDate = DateTime.UtcNow.AddDays(-10)
                }
            }
        };

        _completionHistoryRepositoryMock
            .Setup(x => x.GetByUserAndDateRangeAsync(userId, startDate, endDate, It.IsAny<CancellationToken>()))
            .ReturnsAsync(completions);

        // Act
        var result = await _service.GetCompletionHistoryAsync(userId, startDate, endDate);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().WorkoutName.Should().Be("Test Workout");
    }

    #endregion

    #region Helper Methods

    private Workout CreateValidWorkout(Guid workoutId, Guid planId)
    {
        return new Workout
        {
            Id = workoutId,
            WeekId = Guid.NewGuid(),
            DayOfWeek = WorkoutDay.Monday,
            ScheduledDate = DateTime.UtcNow.Date.AddDays(-1),
            Discipline = Discipline.Running,
            WorkoutName = "Test Workout",
            IntensityLevel = IntensityLevel.Moderate,
            CompletionStatus = CompletionStatus.NotStarted,
            TrainingWeek = new TrainingWeek
            {
                Id = Guid.NewGuid(),
                PlanId = planId,
                WeekNumber = 1,
                Phase = TrainingPhase.Foundation,
                StartDate = DateTime.UtcNow.Date.AddDays(-7),
                EndDate = DateTime.UtcNow.Date
            },
            WorkoutExercises = new List<WorkoutExercise>()
        };
    }

    private TrainingPlan CreateValidPlan(Guid planId, Guid userId)
    {
        return new TrainingPlan
        {
            Id = planId,
            UserId = userId,
            PlanName = "Test Plan",
            StartDate = DateTime.UtcNow.Date.AddDays(-30),
            EndDate = DateTime.UtcNow.Date.AddDays(60),
            TotalWeeks = 12,
            TrainingDaysPerWeek = 4,
            Status = PlanStatus.Active,
            TrainingWeeks = new List<TrainingWeek>()
        };
    }

    private void SetupWorkoutMocks(Guid workoutId, Workout workout, TrainingPlan plan, UserStreak userStreak)
    {
        _workoutRepositoryMock
            .Setup(x => x.GetWorkoutWithExercisesAsync(workoutId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(workout);
        _planRepositoryMock
            .Setup(x => x.GetByIdAsync(workout.TrainingWeek!.PlanId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(plan);
        _userStreakRepositoryMock
            .Setup(x => x.GetOrCreateAsync(plan.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userStreak);
        _completionHistoryRepositoryMock
            .Setup(x => x.GetDistinctCompletionDatesAsync(plan.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DateTime>());
        _planRepositoryMock
            .Setup(x => x.GetPlanWithDetailsAsync(plan.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(plan);
    }

    #endregion
}
