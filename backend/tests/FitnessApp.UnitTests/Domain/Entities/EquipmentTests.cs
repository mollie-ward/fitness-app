using Xunit;
using FluentAssertions;
using FitnessApp.Domain.Entities;

namespace FitnessApp.UnitTests.Domain.Entities;

public class EquipmentTests
{
    [Fact]
    public void Equipment_WithValidData_ShouldCreateSuccessfully()
    {
        // Arrange & Act
        var equipment = new Equipment
        {
            Name = "Barbell",
            Description = "Standard barbell"
        };

        // Assert
        equipment.Name.Should().Be("Barbell");
        equipment.Description.Should().Be("Standard barbell");
    }

    [Fact]
    public void Equipment_ShouldInitializeCollections()
    {
        // Arrange & Act
        var equipment = new Equipment
        {
            Name = "Dumbbells",
            Description = "Pair of dumbbells"
        };

        // Assert
        equipment.ExerciseEquipments.Should().NotBeNull().And.BeEmpty();
    }

    [Theory]
    [InlineData("Bodyweight", "No equipment required")]
    [InlineData("Ski Erg", "Ski ergometer machine")]
    [InlineData("Sled", "Weighted sled for pushing/pulling")]
    public void Equipment_WithDifferentTypes_ShouldAssignCorrectly(string name, string description)
    {
        // Arrange & Act
        var equipment = new Equipment
        {
            Name = name,
            Description = description
        };

        // Assert
        equipment.Name.Should().Be(name);
        equipment.Description.Should().Be(description);
    }
}
