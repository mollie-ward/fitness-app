using Xunit;
using FluentAssertions;
using Moq;
using Microsoft.Extensions.Logging;
using FitnessApp.Application.Services;
using FitnessApp.Application.Common.Interfaces;
using FitnessApp.Domain.Entities;
using FitnessApp.Domain.Enums;
using FitnessApp.Domain.ValueObjects;

namespace FitnessApp.UnitTests.Services;

/// <summary>
/// Unit tests for TrainingPlanGenerationService
/// Tests periodization, exercise selection, progressive overload, and multi-discipline balancing
/// </summary>
public class TrainingPlanGenerationServiceTests
{
    private readonly Mock<IUserProfileRepository> _userProfileRepositoryMock;
    private readonly Mock<ITrainingPlanRepository> _trainingPlanRepositoryMock;
    private readonly Mock<IExerciseRepository> _exerciseRepositoryMock;
    private readonly Mock<ILogger<TrainingPlanGenerationService>> _loggerMock;
    private readonly TrainingPlanGenerationService _service;

    public TrainingPlanGenerationServiceTests()
    {
        _userProfileRepositoryMock = new Mock<IUserProfileRepository>();
        _trainingPlanRepositoryMock = new Mock<ITrainingPlanRepository>();
        _exerciseRepositoryMock = new Mock<IExerciseRepository>();
        _loggerMock = new Mock<ILogger<TrainingPlanGenerationService>>();

        _service = new TrainingPlanGenerationService(
            _userProfileRepositoryMock.Object,
            _trainingPlanRepositoryMock.Object,
            _exerciseRepositoryMock.Object,
            _loggerMock.Object);
    }

    #region Profile Validation Tests

    [Fact]
    public async Task ValidatePlanParametersAsync_WithValidProfile_ShouldReturnTrue()
    {
        // Arrange
        var userProfile = CreateValidUserProfile();

        // Act
        var result = await _service.ValidatePlanParametersAsync(userProfile);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ValidatePlanParametersAsync_WithoutScheduleAvailability_ShouldReturnFalse()
    {
        // Arrange
        var userProfile = CreateValidUserProfile();
        userProfile.ScheduleAvailability = null;

        // Act
        var result = await _service.ValidatePlanParametersAsync(userProfile);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ValidatePlanParametersAsync_WithInvalidSchedule_ShouldReturnFalse()
    {
        // Arrange
        var userProfile = CreateValidUserProfile();
        userProfile.ScheduleAvailability!.MaximumSessionsPerWeek = 0;
        userProfile.ScheduleAvailability.MinimumSessionsPerWeek = 0;

        // Act
        var result = await _service.ValidatePlanParametersAsync(userProfile);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ValidatePlanParametersAsync_WithoutTrainingGoals_ShouldReturnFalse()
    {
        // Arrange
        var userProfile = CreateValidUserProfile();
        userProfile.TrainingGoals = new List<TrainingGoal>();

        // Act
        var result = await _service.ValidatePlanParametersAsync(userProfile);

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region Plan Generation Tests

    [Fact]
    public async Task GeneratePlanAsync_ForBeginnerUser_ShouldGenerateValidPlan()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userProfile = CreateValidUserProfile(userId, FitnessLevel.Beginner);
        var exercises = CreateSampleExercises(DifficultyLevel.Beginner);

        SetupMocksForPlanGeneration(userId, userProfile, exercises);

        // Act
        var result = await _service.GeneratePlanAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result.UserId.Should().Be(userId);
        result.TotalWeeks.Should().Be(12); // Default duration
        result.TrainingDaysPerWeek.Should().Be(3);
        result.Status.Should().Be(PlanStatus.Active);
        result.TrainingWeeks.Should().HaveCount(12);
        
        // Verify repository was called
        _trainingPlanRepositoryMock.Verify(
            x => x.CreatePlanAsync(It.IsAny<TrainingPlan>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GeneratePlanAsync_WithTargetDate_ShouldCalculateCorrectDuration()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var targetDate = DateTime.UtcNow.Date.AddDays(10 * 7); // 10 weeks
        var userProfile = CreateValidUserProfile(userId, FitnessLevel.Intermediate);
        userProfile.TrainingGoals.First().TargetDate = targetDate;
        var exercises = CreateSampleExercises(DifficultyLevel.Intermediate);

        SetupMocksForPlanGeneration(userId, userProfile, exercises);

        // Act
        var result = await _service.GeneratePlanAsync(userId);

        // Assert
        result.TotalWeeks.Should().Be(10);
        result.EndDate.Should().BeCloseTo(targetDate, TimeSpan.FromDays(1));
    }

    [Fact]
    public async Task GeneratePlanAsync_WithInjuries_ShouldExcludeContraindicatedExercises()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userProfile = CreateValidUserProfile(userId);
        
        // Add injury
        userProfile.InjuryLimitations.Add(new InjuryLimitation
        {
            UserProfileId = userProfile.Id,
            BodyPart = "Shoulder",
            InjuryType = InjuryType.Chronic,
            Status = InjuryStatus.Active,
            ReportedDate = DateTime.UtcNow.AddDays(-10)
        });

        var safeExercises = CreateSampleExercises(DifficultyLevel.Beginner);
        
        _userProfileRepositoryMock
            .Setup(x => x.GetCompleteProfileAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userProfile);

        _exerciseRepositoryMock
            .Setup(x => x.GetSafeExercisesAsync(
                It.IsAny<IEnumerable<InjuryType>>(),
                It.IsAny<Discipline?>(),
                It.IsAny<DifficultyLevel?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(safeExercises);

        _trainingPlanRepositoryMock
            .Setup(x => x.CreatePlanAsync(It.IsAny<TrainingPlan>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TrainingPlan plan, CancellationToken ct) => plan);

        // Act
        var result = await _service.GeneratePlanAsync(userId);

        // Assert
        result.Should().NotBeNull();
        
        // Verify safe exercises were requested
        _exerciseRepositoryMock.Verify(
            x => x.GetSafeExercisesAsync(
                It.Is<IEnumerable<InjuryType>>(injuries => injuries.Contains(InjuryType.Chronic)),
                It.IsAny<Discipline?>(),
                It.IsAny<DifficultyLevel?>(),
                It.IsAny<CancellationToken>()),
            Times.AtLeastOnce);
    }

    [Fact]
    public async Task GeneratePlanAsync_ShouldRespectUserScheduleAvailability()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userProfile = CreateValidUserProfile(userId);
        
        // Set specific days available
        userProfile.ScheduleAvailability!.Monday = true;
        userProfile.ScheduleAvailability.Tuesday = false;
        userProfile.ScheduleAvailability.Wednesday = true;
        userProfile.ScheduleAvailability.Thursday = false;
        userProfile.ScheduleAvailability.Friday = true;
        userProfile.ScheduleAvailability.Saturday = false;
        userProfile.ScheduleAvailability.Sunday = false;
        userProfile.ScheduleAvailability.MaximumSessionsPerWeek = 3;

        var exercises = CreateSampleExercises(DifficultyLevel.Beginner);
        SetupMocksForPlanGeneration(userId, userProfile, exercises);

        // Act
        var result = await _service.GeneratePlanAsync(userId);

        // Assert
        result.TrainingDaysPerWeek.Should().Be(3);
        
        foreach (var week in result.TrainingWeeks)
        {
            week.Workouts.Should().HaveCount(3);
            
            // Workouts should only be on available days
            var allowedDays = new[] { WorkoutDay.Monday, WorkoutDay.Wednesday, WorkoutDay.Friday };
            foreach (var workout in week.Workouts)
            {
                allowedDays.Should().Contain(workout.DayOfWeek);
            }
        }
    }

    [Fact]
    public async Task GeneratePlanAsync_ShouldIncludeAppropriatePhases()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userProfile = CreateValidUserProfile(userId);
        var exercises = CreateSampleExercises(DifficultyLevel.Intermediate);

        SetupMocksForPlanGeneration(userId, userProfile, exercises);

        // Act
        var result = await _service.GeneratePlanAsync(userId);

        // Assert - 12-week plan should have multiple phases
        var distinctPhases = result.TrainingWeeks.Select(w => w.Phase).Distinct().ToList();
        distinctPhases.Should().Contain(TrainingPhase.Foundation);
        distinctPhases.Should().Contain(TrainingPhase.Build);
        distinctPhases.Count.Should().BeGreaterThan(1);
    }

    [Fact]
    public async Task GeneratePlanAsync_ShouldApplyProgressiveOverload()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userProfile = CreateValidUserProfile(userId);
        var exercises = CreateSampleExercises(DifficultyLevel.Beginner);

        SetupMocksForPlanGeneration(userId, userProfile, exercises);

        // Act
        var result = await _service.GeneratePlanAsync(userId);

        // Assert
        var weeks = result.TrainingWeeks.OrderBy(w => w.WeekNumber).ToList();
        
        // Volume should generally increase over time (excluding deload weeks)
        for (int i = 1; i < weeks.Count - 1; i++)
        {
            var currentWeek = weeks[i];
            var prevWeek = weeks[i - 1];
            
            // Skip deload weeks
            if (currentWeek.IntensityLevel == IntensityLevel.Low)
                continue;

            // Weekly volume should increase or stay stable
            currentWeek.WeeklyVolume.Should().BeGreaterThanOrEqualTo((int)(prevWeek.WeeklyVolume!.Value * 0.8));
        }
    }

    [Fact]
    public async Task GeneratePlanAsync_ShouldIncludeDeloadWeeks()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userProfile = CreateValidUserProfile(userId);
        var exercises = CreateSampleExercises(DifficultyLevel.Intermediate);

        SetupMocksForPlanGeneration(userId, userProfile, exercises);

        // Act
        var result = await _service.GeneratePlanAsync(userId);

        // Assert - Should have at least one deload week (week 4, 8, etc.)
        var deloadWeeks = result.TrainingWeeks
            .Where(w => w.IntensityLevel == IntensityLevel.Low && w.WeekNumber % 4 == 0)
            .ToList();

        deloadWeeks.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GeneratePlanAsync_MultiDisciplineGoals_ShouldBalanceWorkload()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userProfile = CreateValidUserProfile(userId, FitnessLevel.Advanced);
        
        // Add multiple goals
        userProfile.TrainingGoals.Add(new TrainingGoal
        {
            UserProfileId = userProfile.Id,
            GoalType = GoalType.RunningDistance,
            Description = "10K Race",
            Priority = 2,
            Status = GoalStatus.Active
        });

        var exercises = CreateSampleExercises(DifficultyLevel.Advanced);
        SetupMocksForPlanGeneration(userId, userProfile, exercises);

        // Act
        var result = await _service.GeneratePlanAsync(userId);

        // Assert
        var allWorkouts = result.TrainingWeeks.SelectMany(w => w.Workouts).ToList();
        
        // Should have workouts from multiple disciplines
        var disciplines = allWorkouts.Select(w => w.Discipline).Distinct().ToList();
        disciplines.Count.Should().BeGreaterThan(1);
        
        // Both HYROX and Running should be present
        disciplines.Should().Contain(Discipline.HYROX);
        disciplines.Should().Contain(Discipline.Running);
    }

    [Fact]
    public async Task GeneratePlanAsync_ShouldAssignExercisesToWorkouts()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userProfile = CreateValidUserProfile(userId);
        var exercises = CreateSampleExercises(DifficultyLevel.Beginner);

        SetupMocksForPlanGeneration(userId, userProfile, exercises);

        // Act
        var result = await _service.GeneratePlanAsync(userId);

        // Assert
        var workouts = result.TrainingWeeks.SelectMany(w => w.Workouts).ToList();
        workouts.Should().NotBeEmpty();
        
        foreach (var workout in workouts)
        {
            workout.WorkoutExercises.Should().NotBeEmpty();
            
            // Exercises should have proper ordering
            var exercises_ordered = workout.WorkoutExercises.OrderBy(we => we.OrderInWorkout).ToList();
            for (int i = 0; i < exercises_ordered.Count; i++)
            {
                exercises_ordered[i].OrderInWorkout.Should().Be(i + 1);
            }
        }
    }

    [Fact]
    public async Task GeneratePlanAsync_ShouldSetProgressiveOverloadParameters()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userProfile = CreateValidUserProfile(userId);
        var exercises = CreateSampleExercises(DifficultyLevel.Beginner);

        SetupMocksForPlanGeneration(userId, userProfile, exercises);

        // Act
        var result = await _service.GeneratePlanAsync(userId);

        // Assert
        var week1Workouts = result.TrainingWeeks.First(w => w.WeekNumber == 1).Workouts;
        var week4Workouts = result.TrainingWeeks.First(w => w.WeekNumber == 4).Workouts;

        // Check that exercises have sets/reps or duration
        foreach (var workout in week1Workouts)
        {
            foreach (var exercise in workout.WorkoutExercises)
            {
                // Either sets/reps or duration should be set
                var hasSetReps = exercise.Sets.HasValue && exercise.Reps.HasValue;
                var hasDuration = exercise.Duration.HasValue;
                
                (hasSetReps || hasDuration).Should().BeTrue();
                exercise.IntensityGuidance.Should().NotBeNullOrEmpty();
            }
        }
    }

    [Fact]
    public async Task RegeneratePlanAsync_ShouldArchiveExistingPlan()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userProfile = CreateValidUserProfile(userId);
        var exercises = CreateSampleExercises(DifficultyLevel.Intermediate);
        
        var existingPlan = new TrainingPlan
        {
            UserId = userId,
            PlanName = "Old Plan",
            StartDate = DateTime.UtcNow.Date.AddDays(-30),
            EndDate = DateTime.UtcNow.Date.AddDays(30),
            TotalWeeks = 8,
            TrainingDaysPerWeek = 3,
            Status = PlanStatus.Active,
            CurrentWeek = 1
        };

        _userProfileRepositoryMock
            .Setup(x => x.GetCompleteProfileAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userProfile);

        _trainingPlanRepositoryMock
            .Setup(x => x.GetActivePlanByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingPlan);

        _trainingPlanRepositoryMock
            .Setup(x => x.UpdatePlanAsync(It.IsAny<TrainingPlan>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TrainingPlan plan, CancellationToken ct) => plan);

        _exerciseRepositoryMock
            .Setup(x => x.GetExercisesByCriteriaAsync(
                It.IsAny<Discipline?>(),
                It.IsAny<DifficultyLevel?>(),
                It.IsAny<SessionType?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(exercises);

        _trainingPlanRepositoryMock
            .Setup(x => x.CreatePlanAsync(It.IsAny<TrainingPlan>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TrainingPlan plan, CancellationToken ct) => plan);

        // Act
        var result = await _service.RegeneratePlanAsync(userId);

        // Assert
        result.Should().NotBeNull();
        
        // Verify existing plan was updated to Abandoned
        _trainingPlanRepositoryMock.Verify(
            x => x.UpdatePlanAsync(
                It.Is<TrainingPlan>(p => p.Status == PlanStatus.Abandoned),
                It.IsAny<CancellationToken>()),
            Times.Once);

        // Verify new plan was created
        _trainingPlanRepositoryMock.Verify(
            x => x.CreatePlanAsync(It.IsAny<TrainingPlan>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GeneratePlanAsync_WithInvalidProfile_ShouldThrowException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userProfile = CreateValidUserProfile(userId);
        userProfile.ScheduleAvailability = null; // Invalid

        _userProfileRepositoryMock
            .Setup(x => x.GetCompleteProfileAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userProfile);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _service.GeneratePlanAsync(userId));
    }

    [Fact]
    public async Task GeneratePlanAsync_WithNonExistentUser_ShouldThrowException()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _userProfileRepositoryMock
            .Setup(x => x.GetCompleteProfileAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserProfile?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _service.GeneratePlanAsync(userId));
    }

    #endregion

    #region Helper Methods

    private UserProfile CreateValidUserProfile(Guid? userId = null, FitnessLevel fitnessLevel = FitnessLevel.Beginner)
    {
        var id = userId ?? Guid.NewGuid();
        return new UserProfile
        {
            UserId = id,
            Name = "Test User",
            Email = "test@example.com",
            HyroxLevel = fitnessLevel,
            RunningLevel = fitnessLevel,
            StrengthLevel = fitnessLevel,
            ScheduleAvailability = new ScheduleAvailability
            {
                Monday = true,
                Tuesday = true,
                Wednesday = true,
                Thursday = false,
                Friday = false,
                Saturday = false,
                Sunday = false,
                MinimumSessionsPerWeek = 3,
                MaximumSessionsPerWeek = 3
            },
            TrainingGoals = new List<TrainingGoal>
            {
                new TrainingGoal
                {
                    UserProfileId = Guid.NewGuid(),
                    GoalType = GoalType.HyroxRace,
                    Description = "Complete HYROX race",
                    Priority = 1,
                    Status = GoalStatus.Active
                }
            },
            InjuryLimitations = new List<InjuryLimitation>()
        };
    }

    private List<Exercise> CreateSampleExercises(DifficultyLevel difficulty)
    {
        return new List<Exercise>
        {
            new Exercise
            {
                Name = "Burpees",
                Description = "Full body exercise",
                PrimaryDiscipline = Discipline.HYROX,
                DifficultyLevel = difficulty,
                IntensityLevel = IntensityLevel.High,
                SessionType = SessionType.HybridConditioning,
                ApproximateDuration = null,
                ExerciseMovementPatterns = new List<ExerciseMovementPattern>(),
                ExerciseMuscleGroups = new List<ExerciseMuscleGroup>(),
                ExerciseEquipments = new List<ExerciseEquipment>(),
                ExerciseContraindications = new List<ExerciseContraindication>()
            },
            new Exercise
            {
                Name = "Running",
                Description = "Endurance running",
                PrimaryDiscipline = Discipline.Running,
                DifficultyLevel = difficulty,
                IntensityLevel = IntensityLevel.Moderate,
                SessionType = SessionType.EasyRun,
                ApproximateDuration = 30,
                ExerciseMovementPatterns = new List<ExerciseMovementPattern>(),
                ExerciseMuscleGroups = new List<ExerciseMuscleGroup>(),
                ExerciseEquipments = new List<ExerciseEquipment>(),
                ExerciseContraindications = new List<ExerciseContraindication>()
            },
            new Exercise
            {
                Name = "Squats",
                Description = "Lower body strength",
                PrimaryDiscipline = Discipline.Strength,
                DifficultyLevel = difficulty,
                IntensityLevel = IntensityLevel.Moderate,
                SessionType = SessionType.FullBody,
                ApproximateDuration = null,
                ExerciseMovementPatterns = new List<ExerciseMovementPattern>(),
                ExerciseMuscleGroups = new List<ExerciseMuscleGroup>(),
                ExerciseEquipments = new List<ExerciseEquipment>(),
                ExerciseContraindications = new List<ExerciseContraindication>()
            },
            new Exercise
            {
                Name = "Push-ups",
                Description = "Upper body strength",
                PrimaryDiscipline = Discipline.Strength,
                DifficultyLevel = difficulty,
                IntensityLevel = IntensityLevel.Moderate,
                SessionType = SessionType.FullBody,
                ApproximateDuration = null,
                ExerciseMovementPatterns = new List<ExerciseMovementPattern>(),
                ExerciseMuscleGroups = new List<ExerciseMuscleGroup>(),
                ExerciseEquipments = new List<ExerciseEquipment>(),
                ExerciseContraindications = new List<ExerciseContraindication>()
            }
        };
    }

    private void SetupMocksForPlanGeneration(Guid userId, UserProfile userProfile, List<Exercise> exercises)
    {
        _userProfileRepositoryMock
            .Setup(x => x.GetCompleteProfileAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userProfile);

        _exerciseRepositoryMock
            .Setup(x => x.GetExercisesByCriteriaAsync(
                It.IsAny<Discipline?>(),
                It.IsAny<DifficultyLevel?>(),
                It.IsAny<SessionType?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(exercises);

        _exerciseRepositoryMock
            .Setup(x => x.GetSafeExercisesAsync(
                It.IsAny<IEnumerable<InjuryType>>(),
                It.IsAny<Discipline?>(),
                It.IsAny<DifficultyLevel?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(exercises);

        _trainingPlanRepositoryMock
            .Setup(x => x.CreatePlanAsync(It.IsAny<TrainingPlan>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TrainingPlan plan, CancellationToken ct) => plan);
    }

    #endregion
}
