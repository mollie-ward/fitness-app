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
/// Unit tests for PlanAdaptationService
/// Tests adaptive plan adjustment functionality including missed workouts, intensity changes,
/// schedule modifications, injury accommodations, and timeline adjustments
/// </summary>
public class PlanAdaptationServiceTests
{
    private readonly Mock<ITrainingPlanRepository> _planRepositoryMock;
    private readonly Mock<IWorkoutRepository> _workoutRepositoryMock;
    private readonly Mock<IPlanAdaptationRepository> _adaptationRepositoryMock;
    private readonly Mock<IExerciseRepository> _exerciseRepositoryMock;
    private readonly Mock<ILogger<PlanAdaptationService>> _loggerMock;
    private readonly PlanAdaptationService _service;

    public PlanAdaptationServiceTests()
    {
        _planRepositoryMock = new Mock<ITrainingPlanRepository>();
        _workoutRepositoryMock = new Mock<IWorkoutRepository>();
        _adaptationRepositoryMock = new Mock<IPlanAdaptationRepository>();
        _exerciseRepositoryMock = new Mock<IExerciseRepository>();
        _loggerMock = new Mock<ILogger<PlanAdaptationService>>();

        _service = new PlanAdaptationService(
            _planRepositoryMock.Object,
            _workoutRepositoryMock.Object,
            _adaptationRepositoryMock.Object,
            _exerciseRepositoryMock.Object,
            _loggerMock.Object);
    }

    #region AdaptForMissedWorkoutsAsync Tests

    [Fact]
    public async Task AdaptForMissedWorkoutsAsync_WithTwoMissedWorkouts_ShouldReduceIntensity()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var planId = Guid.NewGuid();
        var missedWorkoutIds = new[] { Guid.NewGuid(), Guid.NewGuid() };

        var plan = CreatePlanWithUpcomingWorkouts(planId, userId, 4);
        SetupPlanMocks(userId, plan);

        _adaptationRepositoryMock
            .Setup(x => x.GetMostRecentByPlanIdAsync(planId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PlanAdaptation?)null);

        _adaptationRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<PlanAdaptation>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PlanAdaptation adaptation, CancellationToken _) => adaptation);

        // Act
        var result = await _service.AdaptForMissedWorkoutsAsync(userId, missedWorkoutIds);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Trigger.Should().Be(AdaptationTrigger.MissedWorkouts);
        result.Type.Should().Be(AdaptationType.Recovery);
        result.WorkoutsAffected.Should().BeGreaterThan(0);

        _workoutRepositoryMock.Verify(x => x.UpdateAsync(
            It.Is<Workout>(w => w.IntensityLevel < IntensityLevel.Maximum),
            It.IsAny<CancellationToken>()), Times.AtLeastOnce);

        _adaptationRepositoryMock.Verify(x => x.AddAsync(
            It.IsAny<PlanAdaptation>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AdaptForMissedWorkoutsAsync_WithNoMissedWorkouts_ShouldThrowException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var missedWorkoutIds = Array.Empty<Guid>();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.AdaptForMissedWorkoutsAsync(userId, missedWorkoutIds));
    }

    [Fact]
    public async Task AdaptForMissedWorkoutsAsync_WithNoActivePlan_ShouldThrowException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var missedWorkoutIds = new[] { Guid.NewGuid() };

        _planRepositoryMock
            .Setup(x => x.GetActivePlansByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TrainingPlan>());

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _service.AdaptForMissedWorkoutsAsync(userId, missedWorkoutIds));
    }

    [Fact]
    public async Task AdaptForMissedWorkoutsAsync_WithFourMissedWorkouts_ShouldApplyTwoWeekReentry()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var planId = Guid.NewGuid();
        var missedWorkoutIds = new[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };

        var plan = CreatePlanWithUpcomingWorkouts(planId, userId, 10);
        SetupPlanMocks(userId, plan);

        _adaptationRepositoryMock
            .Setup(x => x.GetMostRecentByPlanIdAsync(planId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PlanAdaptation?)null);

        _adaptationRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<PlanAdaptation>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PlanAdaptation adaptation, CancellationToken _) => adaptation);

        // Act
        var result = await _service.AdaptForMissedWorkoutsAsync(userId, missedWorkoutIds);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.WorkoutsAffected.Should().BeGreaterThan(0);
        
        // Should apply reduction to 2 weeks worth of workouts (8 workouts for 4 days/week plan)
        _workoutRepositoryMock.Verify(x => x.UpdateAsync(
            It.IsAny<Workout>(),
            It.IsAny<CancellationToken>()), Times.AtLeast(8));
    }

    #endregion

    #region AdaptForIntensityChangeAsync Tests

    [Fact]
    public async Task AdaptForIntensityChangeAsync_MakeHarder_ShouldIncreaseIntensity()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var planId = Guid.NewGuid();
        var adjustment = new IntensityAdjustmentDto
        {
            Direction = IntensityDirection.Harder,
            Reason = "User feels workouts are too easy"
        };

        var plan = CreatePlanWithUpcomingWorkouts(planId, userId, 4, IntensityLevel.Moderate);
        SetupPlanMocks(userId, plan);

        _adaptationRepositoryMock
            .Setup(x => x.GetMostRecentByPlanIdAsync(planId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PlanAdaptation?)null);

        _adaptationRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<PlanAdaptation>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PlanAdaptation adaptation, CancellationToken _) => adaptation);

        // Act
        var result = await _service.AdaptForIntensityChangeAsync(userId, adjustment);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Trigger.Should().Be(AdaptationTrigger.UserRequest);
        result.Type.Should().Be(AdaptationType.Intensity);
        result.WorkoutsAffected.Should().BeGreaterThan(0);

        _workoutRepositoryMock.Verify(x => x.UpdateAsync(
            It.Is<Workout>(w => w.IntensityLevel == IntensityLevel.High),
            It.IsAny<CancellationToken>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task AdaptForIntensityChangeAsync_MakeEasier_ShouldDecreaseIntensity()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var planId = Guid.NewGuid();
        var adjustment = new IntensityAdjustmentDto
        {
            Direction = IntensityDirection.Easier,
            Reason = "User feels workouts are too hard"
        };

        var plan = CreatePlanWithUpcomingWorkouts(planId, userId, 4, IntensityLevel.High);
        SetupPlanMocks(userId, plan);

        _adaptationRepositoryMock
            .Setup(x => x.GetMostRecentByPlanIdAsync(planId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PlanAdaptation?)null);

        _adaptationRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<PlanAdaptation>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PlanAdaptation adaptation, CancellationToken _) => adaptation);

        // Act
        var result = await _service.AdaptForIntensityChangeAsync(userId, adjustment);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.WorkoutsAffected.Should().BeGreaterThan(0);

        _workoutRepositoryMock.Verify(x => x.UpdateAsync(
            It.Is<Workout>(w => w.IntensityLevel == IntensityLevel.Moderate),
            It.IsAny<CancellationToken>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task AdaptForIntensityChangeAsync_WithNoFutureWorkouts_ShouldReturnFailure()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var planId = Guid.NewGuid();
        var adjustment = new IntensityAdjustmentDto
        {
            Direction = IntensityDirection.Harder
        };

        var plan = CreatePlanWithCompletedWorkouts(planId, userId);
        SetupPlanMocks(userId, plan);

        _adaptationRepositoryMock
            .Setup(x => x.GetMostRecentByPlanIdAsync(planId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PlanAdaptation?)null);

        // Act
        var result = await _service.AdaptForIntensityChangeAsync(userId, adjustment);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.WorkoutsAffected.Should().Be(0);
    }

    #endregion

    #region AdaptForScheduleChangeAsync Tests

    [Fact]
    public async Task AdaptForScheduleChangeAsync_WithNewSchedule_ShouldRedistributeWorkouts()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var planId = Guid.NewGuid();
        var newSchedule = new ScheduleAvailabilityDto
        {
            Monday = true,
            Tuesday = false,
            Wednesday = true,
            Thursday = false,
            Friday = true,
            Saturday = false,
            Sunday = false,
            MinimumSessionsPerWeek = 3,
            MaximumSessionsPerWeek = 3
        };

        var plan = CreatePlanWithUpcomingWorkouts(planId, userId, 8);
        SetupPlanMocks(userId, plan);

        _adaptationRepositoryMock
            .Setup(x => x.GetMostRecentByPlanIdAsync(planId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PlanAdaptation?)null);

        _adaptationRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<PlanAdaptation>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PlanAdaptation adaptation, CancellationToken _) => adaptation);

        // Act
        var result = await _service.AdaptForScheduleChangeAsync(userId, newSchedule);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Trigger.Should().Be(AdaptationTrigger.ScheduleChange);
        result.Type.Should().Be(AdaptationType.Schedule);
        result.WorkoutsAffected.Should().BeGreaterThan(0);

        _workoutRepositoryMock.Verify(x => x.UpdateAsync(
            It.IsAny<Workout>(),
            It.IsAny<CancellationToken>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task AdaptForScheduleChangeAsync_WithLessThanTwoDays_ShouldThrowException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var planId = Guid.NewGuid();
        var newSchedule = new ScheduleAvailabilityDto
        {
            Monday = true,
            Tuesday = false,
            Wednesday = false,
            Thursday = false,
            Friday = false,
            Saturday = false,
            Sunday = false
        };

        var plan = CreatePlanWithUpcomingWorkouts(planId, userId, 4);
        SetupPlanMocks(userId, plan);

        _adaptationRepositoryMock
            .Setup(x => x.GetMostRecentByPlanIdAsync(planId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PlanAdaptation?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _service.AdaptForScheduleChangeAsync(userId, newSchedule));
    }

    #endregion

    #region AdaptForInjuryAsync Tests

    [Fact]
    public async Task AdaptForInjuryAsync_WithInjury_ShouldMarkWorkoutsForModification()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var planId = Guid.NewGuid();
        var injuryId = Guid.NewGuid();

        var plan = CreatePlanWithUpcomingWorkouts(planId, userId, 4);
        SetupPlanMocks(userId, plan);

        _adaptationRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<PlanAdaptation>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PlanAdaptation adaptation, CancellationToken _) => adaptation);

        // Act
        var result = await _service.AdaptForInjuryAsync(userId, injuryId);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Trigger.Should().Be(AdaptationTrigger.Injury);
        result.Type.Should().Be(AdaptationType.Injury);
        result.WorkoutsAffected.Should().BeGreaterThan(0);
        result.Warnings.Should().NotBeNullOrEmpty();

        _workoutRepositoryMock.Verify(x => x.UpdateAsync(
            It.Is<Workout>(w => w.Description!.Contains("injury")),
            It.IsAny<CancellationToken>()), Times.AtLeastOnce);
    }

    #endregion

    #region AdaptForGoalTimelineChangeAsync Tests

    [Fact]
    public async Task AdaptForGoalTimelineChangeAsync_ExtendTimeline_ShouldUpdatePlanDates()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var planId = Guid.NewGuid();
        var goalId = Guid.NewGuid();
        var newEndDate = DateTime.UtcNow.AddDays(90);

        var timelineChange = new GoalTimelineChangeDto
        {
            GoalId = goalId,
            NewTargetDate = newEndDate,
            Reason = "Need more time to prepare"
        };

        var plan = CreatePlanWithUpcomingWorkouts(planId, userId, 4);
        plan.EndDate = DateTime.UtcNow.AddDays(60);
        SetupPlanMocks(userId, plan);

        _adaptationRepositoryMock
            .Setup(x => x.GetMostRecentByPlanIdAsync(planId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PlanAdaptation?)null);

        _adaptationRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<PlanAdaptation>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PlanAdaptation adaptation, CancellationToken _) => adaptation);

        // Act
        var result = await _service.AdaptForGoalTimelineChangeAsync(userId, timelineChange);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Trigger.Should().Be(AdaptationTrigger.TimelineChange);
        result.Type.Should().Be(AdaptationType.Timeline);

        _planRepositoryMock.Verify(x => x.UpdatePlanAsync(
            It.Is<TrainingPlan>(p => p.EndDate == newEndDate),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AdaptForGoalTimelineChangeAsync_CompressTooMuch_ShouldThrowException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var planId = Guid.NewGuid();
        var goalId = Guid.NewGuid();
        var newEndDate = DateTime.UtcNow.AddDays(20); // Less than 4 weeks

        var timelineChange = new GoalTimelineChangeDto
        {
            GoalId = goalId,
            NewTargetDate = newEndDate
        };

        var plan = CreatePlanWithUpcomingWorkouts(planId, userId, 4);
        plan.EndDate = DateTime.UtcNow.AddDays(60);
        SetupPlanMocks(userId, plan);

        _adaptationRepositoryMock
            .Setup(x => x.GetMostRecentByPlanIdAsync(planId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PlanAdaptation?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _service.AdaptForGoalTimelineChangeAsync(userId, timelineChange));
    }

    #endregion

    #region AdaptForPerceivedDifficultyAsync Tests

    [Fact]
    public async Task AdaptForPerceivedDifficultyAsync_TooEasy_ShouldIncreaseIntensity()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var planId = Guid.NewGuid();
        var feedbackPattern = "too easy";

        var plan = CreatePlanWithUpcomingWorkouts(planId, userId, 4, IntensityLevel.Moderate);
        SetupPlanMocks(userId, plan);

        _adaptationRepositoryMock
            .Setup(x => x.GetMostRecentByPlanIdAsync(planId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PlanAdaptation?)null);

        _adaptationRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<PlanAdaptation>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PlanAdaptation adaptation, CancellationToken _) => adaptation);

        // Act
        var result = await _service.AdaptForPerceivedDifficultyAsync(userId, feedbackPattern);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Trigger.Should().Be(AdaptationTrigger.PerceivedDifficulty);
        result.Type.Should().Be(AdaptationType.Intensity);
    }

    [Fact]
    public async Task AdaptForPerceivedDifficultyAsync_TooHard_ShouldDecreaseIntensity()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var planId = Guid.NewGuid();
        var feedbackPattern = "too hard";

        var plan = CreatePlanWithUpcomingWorkouts(planId, userId, 4, IntensityLevel.High);
        SetupPlanMocks(userId, plan);

        _adaptationRepositoryMock
            .Setup(x => x.GetMostRecentByPlanIdAsync(planId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PlanAdaptation?)null);

        _adaptationRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<PlanAdaptation>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PlanAdaptation adaptation, CancellationToken _) => adaptation);

        // Act
        var result = await _service.AdaptForPerceivedDifficultyAsync(userId, feedbackPattern);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Trigger.Should().Be(AdaptationTrigger.PerceivedDifficulty);
    }

    #endregion

    #region Safety Tests

    [Fact]
    public async Task AdaptForIntensityChangeAsync_WithRecentAdaptation_ShouldThrowException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var planId = Guid.NewGuid();
        var adjustment = new IntensityAdjustmentDto
        {
            Direction = IntensityDirection.Harder
        };

        var plan = CreatePlanWithUpcomingWorkouts(planId, userId, 4);
        SetupPlanMocks(userId, plan);

        // Setup recent adaptation (3 days ago)
        var recentAdaptation = new PlanAdaptation
        {
            PlanId = planId,
            Trigger = AdaptationTrigger.UserRequest,
            Type = AdaptationType.Intensity,
            Description = "Previous adaptation",
            AppliedAt = DateTime.UtcNow.AddDays(-3)
        };

        _adaptationRepositoryMock
            .Setup(x => x.GetMostRecentByPlanIdAsync(planId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(recentAdaptation);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _service.AdaptForIntensityChangeAsync(userId, adjustment));
    }

    #endregion

    #region Helper Methods

    private TrainingPlan CreatePlanWithUpcomingWorkouts(
        Guid planId, 
        Guid userId, 
        int workoutCount,
        IntensityLevel intensity = IntensityLevel.Moderate)
    {
        var plan = new TrainingPlan
        {
            Id = planId,
            UserId = userId,
            PlanName = "Test Plan",
            StartDate = DateTime.UtcNow.Date,
            EndDate = DateTime.UtcNow.AddDays(60),
            TotalWeeks = 8,
            TrainingDaysPerWeek = 4,
            Status = PlanStatus.Active,
            CurrentWeek = 1
        };

        var week = new TrainingWeek
        {
            Id = Guid.NewGuid(),
            PlanId = planId,
            WeekNumber = 1,
            Phase = TrainingPhase.Foundation,
            StartDate = DateTime.UtcNow.Date,
            EndDate = DateTime.UtcNow.Date.AddDays(7),
            TrainingPlan = plan,
            Workouts = new List<Workout>()
        };

        for (int i = 0; i < workoutCount; i++)
        {
            week.Workouts.Add(new Workout
            {
                Id = Guid.NewGuid(),
                WeekId = week.Id,
                DayOfWeek = (WorkoutDay)(i % 7),
                ScheduledDate = DateTime.UtcNow.Date.AddDays(i + 1),
                Discipline = Discipline.Running,
                WorkoutName = $"Workout {i + 1}",
                Description = $"Test workout {i + 1}",
                IntensityLevel = intensity,
                IsKeyWorkout = i % 2 == 0,
                CompletionStatus = CompletionStatus.NotStarted,
                TrainingWeek = week
            });
        }

        plan.TrainingWeeks = new List<TrainingWeek> { week };
        return plan;
    }

    private TrainingPlan CreatePlanWithCompletedWorkouts(Guid planId, Guid userId)
    {
        var plan = CreatePlanWithUpcomingWorkouts(planId, userId, 4);
        
        // Make all workouts in the past and completed
        foreach (var week in plan.TrainingWeeks)
        {
            foreach (var workout in week.Workouts)
            {
                workout.ScheduledDate = DateTime.UtcNow.Date.AddDays(-7);
                workout.CompletionStatus = CompletionStatus.Completed;
            }
        }

        return plan;
    }

    private void SetupPlanMocks(Guid userId, TrainingPlan plan)
    {
        _planRepositoryMock
            .Setup(x => x.GetActivePlansByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { plan });

        _planRepositoryMock
            .Setup(x => x.GetPlanWithDetailsAsync(plan.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(plan);

        _planRepositoryMock
            .Setup(x => x.UpdatePlanAsync(It.IsAny<TrainingPlan>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TrainingPlan p, CancellationToken _) => p);

        _workoutRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Workout>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
    }

    #endregion
}
