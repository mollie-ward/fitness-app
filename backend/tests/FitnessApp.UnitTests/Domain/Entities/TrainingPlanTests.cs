using Xunit;
using FluentAssertions;
using FitnessApp.Domain.Entities;
using FitnessApp.Domain.Enums;

namespace FitnessApp.UnitTests.Domain.Entities;

public class TrainingPlanTests
{
    [Fact]
    public void TrainingPlan_WithValidData_ShouldCreateSuccessfully()
    {
        // Arrange & Act
        var startDate = DateTime.UtcNow.Date;
        var endDate = startDate.AddDays(12 * 7);
        var plan = new TrainingPlan
        {
            UserId = Guid.NewGuid(),
            PlanName = "12-Week HYROX Preparation",
            StartDate = startDate,
            EndDate = endDate,
            TotalWeeks = 12,
            TrainingDaysPerWeek = 5,
            Status = PlanStatus.Active,
            CurrentWeek = 1
        };

        // Assert
        plan.PlanName.Should().Be("12-Week HYROX Preparation");
        plan.TotalWeeks.Should().Be(12);
        plan.TrainingDaysPerWeek.Should().Be(5);
        plan.Status.Should().Be(PlanStatus.Active);
        plan.CurrentWeek.Should().Be(1);
        plan.IsDeleted.Should().BeFalse();
    }

    [Fact]
    public void TrainingPlan_ShouldHaveEmptyCollections_ByDefault()
    {
        // Arrange & Act
        var plan = new TrainingPlan
        {
            UserId = Guid.NewGuid(),
            PlanName = "Test Plan",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(8 * 7),
            TotalWeeks = 8,
            TrainingDaysPerWeek = 4
        };

        // Assert
        plan.TrainingWeeks.Should().NotBeNull();
        plan.TrainingWeeks.Should().BeEmpty();
    }

    [Fact]
    public void TrainingPlan_CanAddTrainingWeeks()
    {
        // Arrange
        var plan = new TrainingPlan
        {
            UserId = Guid.NewGuid(),
            PlanName = "Test Plan",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(4 * 7),
            TotalWeeks = 4,
            TrainingDaysPerWeek = 5
        };

        var week1 = new TrainingWeek
        {
            PlanId = plan.Id,
            WeekNumber = 1,
            Phase = TrainingPhase.Foundation,
            StartDate = plan.StartDate,
            EndDate = plan.StartDate.AddDays(7)
        };

        // Act
        plan.TrainingWeeks.Add(week1);

        // Assert
        plan.TrainingWeeks.Should().HaveCount(1);
        plan.TrainingWeeks.First().WeekNumber.Should().Be(1);
        plan.TrainingWeeks.First().Phase.Should().Be(TrainingPhase.Foundation);
    }

    [Fact]
    public void TrainingPlan_StatusTransitions_ShouldWork()
    {
        // Arrange
        var plan = new TrainingPlan
        {
            UserId = Guid.NewGuid(),
            PlanName = "Test Plan",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(8 * 7),
            TotalWeeks = 8,
            TrainingDaysPerWeek = 4,
            Status = PlanStatus.Active,
            CurrentWeek = 1
        };

        // Act & Assert - Active to Paused
        plan.Status = PlanStatus.Paused;
        plan.Status.Should().Be(PlanStatus.Paused);

        // Act & Assert - Paused to Active
        plan.Status = PlanStatus.Active;
        plan.Status.Should().Be(PlanStatus.Active);

        // Act & Assert - Active to Completed
        plan.Status = PlanStatus.Completed;
        plan.Status.Should().Be(PlanStatus.Completed);
    }

    [Fact]
    public void TrainingPlan_SoftDelete_ShouldSetIsDeletedFlag()
    {
        // Arrange
        var plan = new TrainingPlan
        {
            UserId = Guid.NewGuid(),
            PlanName = "Test Plan",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(8 * 7),
            TotalWeeks = 8,
            TrainingDaysPerWeek = 4,
            IsDeleted = false
        };

        // Act
        plan.IsDeleted = true;

        // Assert
        plan.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public void TrainingPlan_CanAssociatePlanMetadata()
    {
        // Arrange
        var plan = new TrainingPlan
        {
            UserId = Guid.NewGuid(),
            PlanName = "Test Plan",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(8 * 7),
            TotalWeeks = 8,
            TrainingDaysPerWeek = 4
        };

        var metadata = new PlanMetadata
        {
            PlanId = plan.Id,
            AlgorithmVersion = "1.0.0",
            GenerationParameters = "{\"goal\": \"HYROX\"}"
        };

        // Act
        plan.PlanMetadata = metadata;

        // Assert
        plan.PlanMetadata.Should().NotBeNull();
        plan.PlanMetadata!.AlgorithmVersion.Should().Be("1.0.0");
    }
}
