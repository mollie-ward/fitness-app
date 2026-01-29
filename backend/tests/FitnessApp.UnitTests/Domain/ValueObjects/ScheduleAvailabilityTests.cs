using Xunit;
using FluentAssertions;
using FitnessApp.Domain.ValueObjects;

namespace FitnessApp.UnitTests.Domain.ValueObjects;

public class ScheduleAvailabilityTests
{
    [Fact]
    public void ScheduleAvailability_WithValidData_ShouldCreateSuccessfully()
    {
        // Arrange & Act
        var schedule = new ScheduleAvailability
        {
            Monday = true,
            Wednesday = true,
            Friday = true,
            MinimumSessionsPerWeek = 2,
            MaximumSessionsPerWeek = 3
        };

        // Assert
        schedule.Monday.Should().BeTrue();
        schedule.Wednesday.Should().BeTrue();
        schedule.Friday.Should().BeTrue();
        schedule.MinimumSessionsPerWeek.Should().Be(2);
        schedule.MaximumSessionsPerWeek.Should().Be(3);
    }

    [Fact]
    public void ScheduleAvailability_ShouldCountAvailableDays()
    {
        // Arrange
        var schedule = new ScheduleAvailability
        {
            Monday = true,
            Tuesday = false,
            Wednesday = true,
            Thursday = false,
            Friday = true,
            Saturday = true,
            Sunday = false,
            MinimumSessionsPerWeek = 1,
            MaximumSessionsPerWeek = 4
        };

        // Act
        var count = schedule.AvailableDaysCount;

        // Assert
        count.Should().Be(4);
    }

    [Fact]
    public void ScheduleAvailability_WithNoDaysSelected_IsInvalid()
    {
        // Arrange
        var schedule = new ScheduleAvailability
        {
            MinimumSessionsPerWeek = 1,
            MaximumSessionsPerWeek = 3
        };

        // Act
        var isValid = schedule.IsValid();

        // Assert
        isValid.Should().BeFalse();
    }

    [Fact]
    public void ScheduleAvailability_WithValidDaysAndSessions_IsValid()
    {
        // Arrange
        var schedule = new ScheduleAvailability
        {
            Monday = true,
            Wednesday = true,
            Friday = true,
            MinimumSessionsPerWeek = 2,
            MaximumSessionsPerWeek = 3
        };

        // Act
        var isValid = schedule.IsValid();

        // Assert
        isValid.Should().BeTrue();
    }

    [Fact]
    public void ScheduleAvailability_WithMaxLessThanMin_IsInvalid()
    {
        // Arrange
        var schedule = new ScheduleAvailability
        {
            Monday = true,
            Wednesday = true,
            Friday = true,
            MinimumSessionsPerWeek = 3,
            MaximumSessionsPerWeek = 2
        };

        // Act
        var isValid = schedule.IsValid();

        // Assert
        isValid.Should().BeFalse();
    }

    [Fact]
    public void ScheduleAvailability_WithMaxExceedingAvailableDays_IsInvalid()
    {
        // Arrange
        var schedule = new ScheduleAvailability
        {
            Monday = true,
            Wednesday = true,
            Friday = true,
            MinimumSessionsPerWeek = 2,
            MaximumSessionsPerWeek = 5
        };

        // Act
        var isValid = schedule.IsValid();

        // Assert
        isValid.Should().BeFalse();
    }

    [Fact]
    public void ScheduleAvailability_WithMinimumZero_IsInvalid()
    {
        // Arrange
        var schedule = new ScheduleAvailability
        {
            Monday = true,
            Wednesday = true,
            MinimumSessionsPerWeek = 0,
            MaximumSessionsPerWeek = 2
        };

        // Act
        var isValid = schedule.IsValid();

        // Assert
        isValid.Should().BeFalse();
    }

    [Fact]
    public void ScheduleAvailability_WithAllDaysSelected_ShouldCountSeven()
    {
        // Arrange
        var schedule = new ScheduleAvailability
        {
            Monday = true,
            Tuesday = true,
            Wednesday = true,
            Thursday = true,
            Friday = true,
            Saturday = true,
            Sunday = true,
            MinimumSessionsPerWeek = 1,
            MaximumSessionsPerWeek = 7
        };

        // Act
        var count = schedule.AvailableDaysCount;

        // Assert
        count.Should().Be(7);
        schedule.IsValid().Should().BeTrue();
    }
}
