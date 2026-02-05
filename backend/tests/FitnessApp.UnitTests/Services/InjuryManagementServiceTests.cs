using Xunit;
using FluentAssertions;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using FitnessApp.Application.Services;
using FitnessApp.Application.Common.Interfaces;
using FitnessApp.Application.DTOs;
using FitnessApp.Domain.Entities;
using FitnessApp.Domain.Enums;

namespace FitnessApp.UnitTests.Services;

/// <summary>
/// Unit tests for InjuryManagementService
/// Tests injury reporting, contraindication matching, substitute selection, and status updates
/// </summary>
public class InjuryManagementServiceTests
{
    private readonly Mock<IUserProfileRepository> _userProfileRepositoryMock;
    private readonly Mock<IExerciseRepository> _exerciseRepositoryMock;
    private readonly Mock<IApplicationDbContext> _contextMock;
    private readonly Mock<IPlanAdaptationService> _planAdaptationServiceMock;
    private readonly Mock<ILogger<InjuryManagementService>> _loggerMock;
    private readonly InjuryManagementService _service;

    public InjuryManagementServiceTests()
    {
        _userProfileRepositoryMock = new Mock<IUserProfileRepository>();
        _exerciseRepositoryMock = new Mock<IExerciseRepository>();
        _contextMock = new Mock<IApplicationDbContext>();
        _planAdaptationServiceMock = new Mock<IPlanAdaptationService>();
        _loggerMock = new Mock<ILogger<InjuryManagementService>>();

        _service = new InjuryManagementService(
            _userProfileRepositoryMock.Object,
            _exerciseRepositoryMock.Object,
            _contextMock.Object,
            _planAdaptationServiceMock.Object,
            _loggerMock.Object);
    }

    #region ReportInjuryAsync Tests

    [Fact]
    public async Task ReportInjuryAsync_WithValidData_ShouldCreateInjuryAndTriggerAdaptation()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var profileId = Guid.NewGuid();
        var profile = new UserProfile { Id = profileId, UserId = userId, Name = "Test User", Email = "test@example.com" };

        var injuryDto = new ReportInjuryDto
        {
            BodyPart = "Shoulder",
            InjuryType = InjuryType.Acute,
            Severity = "Moderate",
            MovementRestrictions = "Overhead"
        };

        _userProfileRepositoryMock
            .Setup(x => x.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(profile);

        _userProfileRepositoryMock
            .Setup(x => x.AddInjuryAsync(It.IsAny<InjuryLimitation>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((InjuryLimitation injury, CancellationToken _) =>
            {
                injury.Id = Guid.NewGuid();
                return injury;
            });

        _planAdaptationServiceMock
            .Setup(x => x.AdaptForInjuryAsync(userId, It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PlanAdaptationResultDto
            {
                Success = true,
                WorkoutsAffected = 5,
                Description = "Plan adapted for injury"
            });

        // Act
        var result = await _service.ReportInjuryAsync(userId, injuryDto);

        // Assert
        result.Should().NotBeNull();
        result.BodyPart.Should().Be("Shoulder");
        result.InjuryType.Should().Be(InjuryType.Acute);
        result.Status.Should().Be(InjuryStatus.Active);

        _userProfileRepositoryMock.Verify(x => x.AddInjuryAsync(
            It.Is<InjuryLimitation>(i => i.BodyPart == "Shoulder" && i.UserProfileId == profileId),
            It.IsAny<CancellationToken>()), Times.Once);

        _planAdaptationServiceMock.Verify(x => x.AdaptForInjuryAsync(
            userId, It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ReportInjuryAsync_WithoutUserProfile_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var injuryDto = new ReportInjuryDto { BodyPart = "Knee", InjuryType = InjuryType.Acute };

        _userProfileRepositoryMock
            .Setup(x => x.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserProfile?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _service.ReportInjuryAsync(userId, injuryDto));
    }

    [Fact]
    public async Task ReportInjuryAsync_WhenPlanAdaptationFails_ShouldStillCreateInjury()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var profileId = Guid.NewGuid();
        var profile = new UserProfile { Id = profileId, UserId = userId, Name = "Test User", Email = "test@example.com" };

        var injuryDto = new ReportInjuryDto
        {
            BodyPart = "Back",
            InjuryType = InjuryType.Chronic
        };

        _userProfileRepositoryMock
            .Setup(x => x.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(profile);

        _userProfileRepositoryMock
            .Setup(x => x.AddInjuryAsync(It.IsAny<InjuryLimitation>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((InjuryLimitation injury, CancellationToken _) =>
            {
                injury.Id = Guid.NewGuid();
                return injury;
            });

        _planAdaptationServiceMock
            .Setup(x => x.AdaptForInjuryAsync(userId, It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Plan adaptation failed"));

        // Act
        var result = await _service.ReportInjuryAsync(userId, injuryDto);

        // Assert
        result.Should().NotBeNull();
        result.BodyPart.Should().Be("Back");
        
        // Injury should still be created even if adaptation fails
        _userProfileRepositoryMock.Verify(x => x.AddInjuryAsync(
            It.IsAny<InjuryLimitation>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region UpdateInjuryStatusAsync Tests

    [Fact]
    public async Task UpdateInjuryStatusAsync_WithValidData_ShouldUpdateStatus()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var profileId = Guid.NewGuid();
        var injuryId = Guid.NewGuid();

        var profile = new UserProfile { Id = profileId, UserId = userId, Name = "Test User", Email = "test@example.com" };
        var injury = new InjuryLimitation
        {
            Id = injuryId,
            UserProfileId = profileId,
            BodyPart = "Knee",
            InjuryType = InjuryType.Acute,
            Status = InjuryStatus.Active,
            ReportedDate = DateTime.UtcNow
        };

        var statusUpdate = new UpdateInjuryStatusDto { Status = InjuryStatus.Improving };

        _userProfileRepositoryMock
            .Setup(x => x.GetInjuryByIdAsync(injuryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(injury);

        _userProfileRepositoryMock
            .Setup(x => x.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(profile);

        _userProfileRepositoryMock
            .Setup(x => x.UpdateInjuryAsync(It.IsAny<InjuryLimitation>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((InjuryLimitation i, CancellationToken _) => i);

        _planAdaptationServiceMock
            .Setup(x => x.AdaptForInjuryAsync(userId, injuryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PlanAdaptationResultDto { Success = true, Description = "Plan adapted for status change" });

        // Act
        var result = await _service.UpdateInjuryStatusAsync(userId, injuryId, statusUpdate);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(InjuryStatus.Improving);

        _userProfileRepositoryMock.Verify(x => x.UpdateInjuryAsync(
            It.Is<InjuryLimitation>(i => i.Status == InjuryStatus.Improving),
            It.IsAny<CancellationToken>()), Times.Once);

        _planAdaptationServiceMock.Verify(x => x.AdaptForInjuryAsync(
            userId, injuryId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateInjuryStatusAsync_WithUnauthorizedUser_ShouldThrowUnauthorizedAccessException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var profileId = Guid.NewGuid();
        var injuryId = Guid.NewGuid();

        var otherProfile = new UserProfile { Id = profileId, UserId = otherUserId, Name = "Other User", Email = "other@example.com" };
        var injury = new InjuryLimitation
        {
            Id = injuryId,
            UserProfileId = profileId,
            BodyPart = "Knee",
            InjuryType = InjuryType.Acute,
            Status = InjuryStatus.Active,
            ReportedDate = DateTime.UtcNow
        };

        var statusUpdate = new UpdateInjuryStatusDto { Status = InjuryStatus.Resolved };

        _userProfileRepositoryMock
            .Setup(x => x.GetInjuryByIdAsync(injuryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(injury);

        _userProfileRepositoryMock
            .Setup(x => x.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserProfile?)null);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _service.UpdateInjuryStatusAsync(userId, injuryId, statusUpdate));
    }

    [Fact]
    public async Task UpdateInjuryStatusAsync_WithNoStatusChange_ShouldNotTriggerAdaptation()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var profileId = Guid.NewGuid();
        var injuryId = Guid.NewGuid();

        var profile = new UserProfile { Id = profileId, UserId = userId, Name = "Test User", Email = "test@example.com" };
        var injury = new InjuryLimitation
        {
            Id = injuryId,
            UserProfileId = profileId,
            BodyPart = "Shoulder",
            InjuryType = InjuryType.Acute,
            Status = InjuryStatus.Active,
            ReportedDate = DateTime.UtcNow
        };

        var statusUpdate = new UpdateInjuryStatusDto { Status = InjuryStatus.Active }; // No change

        _userProfileRepositoryMock
            .Setup(x => x.GetInjuryByIdAsync(injuryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(injury);

        _userProfileRepositoryMock
            .Setup(x => x.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(profile);

        _userProfileRepositoryMock
            .Setup(x => x.UpdateInjuryAsync(It.IsAny<InjuryLimitation>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((InjuryLimitation i, CancellationToken _) => i);

        // Act
        var result = await _service.UpdateInjuryStatusAsync(userId, injuryId, statusUpdate);

        // Assert
        result.Should().NotBeNull();
        
        // Plan adaptation should not be triggered if status didn't change
        _planAdaptationServiceMock.Verify(x => x.AdaptForInjuryAsync(
            It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion

    #region MarkInjuryResolvedAsync Tests

    [Fact]
    public async Task MarkInjuryResolvedAsync_ShouldUpdateStatusToResolved()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var profileId = Guid.NewGuid();
        var injuryId = Guid.NewGuid();

        var profile = new UserProfile { Id = profileId, UserId = userId, Name = "Test User", Email = "test@example.com" };
        var injury = new InjuryLimitation
        {
            Id = injuryId,
            UserProfileId = profileId,
            BodyPart = "Ankle",
            InjuryType = InjuryType.Acute,
            Status = InjuryStatus.Improving,
            ReportedDate = DateTime.UtcNow
        };

        _userProfileRepositoryMock
            .Setup(x => x.GetInjuryByIdAsync(injuryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(injury);

        _userProfileRepositoryMock
            .Setup(x => x.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(profile);

        _userProfileRepositoryMock
            .Setup(x => x.UpdateInjuryAsync(It.IsAny<InjuryLimitation>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((InjuryLimitation i, CancellationToken _) => i);

        _planAdaptationServiceMock
            .Setup(x => x.AdaptForInjuryAsync(userId, injuryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PlanAdaptationResultDto { Success = true, Description = "Plan adapted for resolved injury" });

        // Act
        var result = await _service.MarkInjuryResolvedAsync(userId, injuryId);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(InjuryStatus.Resolved);

        _userProfileRepositoryMock.Verify(x => x.UpdateInjuryAsync(
            It.Is<InjuryLimitation>(i => i.Status == InjuryStatus.Resolved),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region GetActiveInjuriesAsync Tests

    [Fact]
    public async Task GetActiveInjuriesAsync_WithActiveAndImprovingInjuries_ShouldReturnBoth()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var profileId = Guid.NewGuid();

        var profile = new UserProfile
        {
            Id = profileId,
            UserId = userId,
            Name = "Test User",
            Email = "test@example.com",
            InjuryLimitations = new List<InjuryLimitation>
            {
                new InjuryLimitation
                {
                    Id = Guid.NewGuid(),
                    UserProfileId = profileId,
                    BodyPart = "Shoulder",
                    InjuryType = InjuryType.Acute,
                    Status = InjuryStatus.Active,
                    ReportedDate = DateTime.UtcNow
                },
                new InjuryLimitation
                {
                    Id = Guid.NewGuid(),
                    UserProfileId = profileId,
                    BodyPart = "Knee",
                    InjuryType = InjuryType.Chronic,
                    Status = InjuryStatus.Improving,
                    ReportedDate = DateTime.UtcNow.AddDays(-7)
                },
                new InjuryLimitation
                {
                    Id = Guid.NewGuid(),
                    UserProfileId = profileId,
                    BodyPart = "Ankle",
                    InjuryType = InjuryType.Acute,
                    Status = InjuryStatus.Resolved,
                    ReportedDate = DateTime.UtcNow.AddDays(-30)
                }
            }
        };

        _userProfileRepositoryMock
            .Setup(x => x.GetProfileWithInjuriesAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(profile);

        // Act
        var result = await _service.GetActiveInjuriesAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2); // Active and Improving only
        result.Should().Contain(i => i.BodyPart == "Shoulder" && i.Status == InjuryStatus.Active);
        result.Should().Contain(i => i.BodyPart == "Knee" && i.Status == InjuryStatus.Improving);
        result.Should().NotContain(i => i.Status == InjuryStatus.Resolved);
    }

    [Fact]
    public async Task GetActiveInjuriesAsync_WithNoProfile_ShouldReturnEmptyList()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _userProfileRepositoryMock
            .Setup(x => x.GetProfileWithInjuriesAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserProfile?)null);

        // Act
        var result = await _service.GetActiveInjuriesAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    #endregion

    #region GetContraindicatedExercisesAsync Tests

    [Fact]
    public async Task GetContraindicatedExercisesAsync_WithShoulderInjury_ShouldReturnOverheadExercises()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var profileId = Guid.NewGuid();

        var profile = new UserProfile
        {
            Id = profileId,
            UserId = userId,
            Name = "Test User",
            Email = "test@example.com",
            InjuryLimitations = new List<InjuryLimitation>
            {
                new InjuryLimitation
                {
                    Id = Guid.NewGuid(),
                    UserProfileId = profileId,
                    BodyPart = "Shoulder",
                    InjuryType = InjuryType.Acute,
                    Status = InjuryStatus.Active,
                    MovementRestrictions = "Overhead; Push",
                    ReportedDate = DateTime.UtcNow
                }
            }
        };

        var exercises = new List<Exercise>
        {
            new Exercise
            {
                Id = Guid.NewGuid(),
                Name = "Overhead Press",
                Description = "Press weight overhead",
                PrimaryDiscipline = Discipline.Strength,
                DifficultyLevel = DifficultyLevel.Intermediate,
                IntensityLevel = IntensityLevel.High,
                ExerciseContraindications = new List<ExerciseContraindication>
                {
                    new ExerciseContraindication
                    {
                        Id = Guid.NewGuid(),
                        Contraindication = new Contraindication
                        {
                            Id = Guid.NewGuid(),
                            InjuryType = "Shoulder",
                            MovementRestriction = "Overhead"
                        },
                        Severity = "Absolute"
                    }
                },
                ExerciseMovementPatterns = new List<ExerciseMovementPattern>()
            }
        };

        _userProfileRepositoryMock
            .Setup(x => x.GetProfileWithInjuriesAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(profile);

        var exercisesDbSetMock = CreateDbSetMock(exercises);
        _contextMock.Setup(x => x.Exercises).Returns(exercisesDbSetMock.Object);

        // Act
        var result = await _service.GetContraindicatedExercisesAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCountGreaterThan(0);
        result.Should().Contain(c => c.ExerciseName == "Overhead Press");
    }

    [Fact]
    public async Task GetContraindicatedExercisesAsync_WithNoActiveInjuries_ShouldReturnEmpty()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var profileId = Guid.NewGuid();

        var profile = new UserProfile
        {
            Id = profileId,
            UserId = userId,
            Name = "Test User",
            Email = "test@example.com",
            InjuryLimitations = new List<InjuryLimitation>()
        };

        _userProfileRepositoryMock
            .Setup(x => x.GetProfileWithInjuriesAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(profile);

        // Act
        var result = await _service.GetContraindicatedExercisesAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    #endregion

    #region Helper Methods

    private Mock<DbSet<T>> CreateDbSetMock<T>(List<T> data) where T : class
    {
        var queryable = data.AsQueryable();
        var dbSetMock = new Mock<DbSet<T>>();

        dbSetMock.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
        dbSetMock.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
        dbSetMock.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
        dbSetMock.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());

        return dbSetMock;
    }

    #endregion
}
