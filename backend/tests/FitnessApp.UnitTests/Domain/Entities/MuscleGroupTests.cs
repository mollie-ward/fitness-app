using Xunit;
using FluentAssertions;
using FitnessApp.Domain.Entities;

namespace FitnessApp.UnitTests.Domain.Entities;

public class MuscleGroupTests
{
    [Fact]
    public void MuscleGroup_WithValidData_ShouldCreateSuccessfully()
    {
        // Arrange & Act
        var muscleGroup = new MuscleGroup
        {
            Name = "Quadriceps",
            Category = "Lower Body",
            Description = "Front thigh muscles"
        };

        // Assert
        muscleGroup.Name.Should().Be("Quadriceps");
        muscleGroup.Category.Should().Be("Lower Body");
        muscleGroup.Description.Should().Be("Front thigh muscles");
    }

    [Fact]
    public void MuscleGroup_ShouldInitializeCollections()
    {
        // Arrange & Act
        var muscleGroup = new MuscleGroup
        {
            Name = "Chest",
            Category = "Upper Body"
        };

        // Assert
        muscleGroup.ExerciseMuscleGroups.Should().NotBeNull().And.BeEmpty();
    }

    [Theory]
    [InlineData("Upper Body", "Chest")]
    [InlineData("Lower Body", "Quadriceps")]
    [InlineData("Core", "Core")]
    [InlineData("Full Body", "Full Body")]
    public void MuscleGroup_WithDifferentCategories_ShouldAssignCorrectly(string category, string name)
    {
        // Arrange & Act
        var muscleGroup = new MuscleGroup
        {
            Name = name,
            Category = category
        };

        // Assert
        muscleGroup.Category.Should().Be(category);
        muscleGroup.Name.Should().Be(name);
    }
}
