using Xunit;
using FluentAssertions;
using FitnessApp.Domain.Entities;

namespace FitnessApp.UnitTests.Domain.Entities;

public class ContraindicationTests
{
    [Fact]
    public void Contraindication_WithValidData_ShouldCreateSuccessfully()
    {
        // Arrange & Act
        var contraindication = new Contraindication
        {
            InjuryType = "Shoulder",
            MovementRestriction = "Overhead",
            Description = "Shoulder injuries limiting overhead movement"
        };

        // Assert
        contraindication.InjuryType.Should().Be("Shoulder");
        contraindication.MovementRestriction.Should().Be("Overhead");
        contraindication.Description.Should().Be("Shoulder injuries limiting overhead movement");
    }

    [Fact]
    public void Contraindication_ShouldInitializeCollections()
    {
        // Arrange & Act
        var contraindication = new Contraindication
        {
            InjuryType = "Knee",
            MovementRestriction = "Impact"
        };

        // Assert
        contraindication.ExerciseContraindications.Should().NotBeNull().And.BeEmpty();
    }

    [Theory]
    [InlineData("Shoulder", "Overhead")]
    [InlineData("Knee", "Impact")]
    [InlineData("Lower Back", "Heavy Load")]
    [InlineData("Wrist", "Weight Bearing")]
    public void Contraindication_WithDifferentTypes_ShouldAssignCorrectly(string injuryType, string restriction)
    {
        // Arrange & Act
        var contraindication = new Contraindication
        {
            InjuryType = injuryType,
            MovementRestriction = restriction
        };

        // Assert
        contraindication.InjuryType.Should().Be(injuryType);
        contraindication.MovementRestriction.Should().Be(restriction);
    }
}
