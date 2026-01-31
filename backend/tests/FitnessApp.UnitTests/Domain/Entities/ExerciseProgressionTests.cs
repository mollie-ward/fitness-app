using Xunit;
using FluentAssertions;
using FitnessApp.Domain.Entities;

namespace FitnessApp.UnitTests.Domain.Entities;

public class ExerciseProgressionTests
{
    [Fact]
    public void ExerciseProgression_WithValidData_ShouldCreateSuccessfully()
    {
        // Arrange
        var baseExercise = new Exercise
        {
            Name = "Push-up",
            Description = "Standard push-up",
            PrimaryDiscipline = FitnessApp.Domain.Enums.Discipline.Strength,
            DifficultyLevel = FitnessApp.Domain.Enums.DifficultyLevel.Beginner,
            IntensityLevel = FitnessApp.Domain.Enums.IntensityLevel.Moderate
        };

        var regressionExercise = new Exercise
        {
            Name = "Knee Push-up",
            Description = "Push-up from knees",
            PrimaryDiscipline = FitnessApp.Domain.Enums.Discipline.Strength,
            DifficultyLevel = FitnessApp.Domain.Enums.DifficultyLevel.Beginner,
            IntensityLevel = FitnessApp.Domain.Enums.IntensityLevel.Low
        };

        var progressionExercise = new Exercise
        {
            Name = "Weighted Push-up",
            Description = "Push-up with added weight",
            PrimaryDiscipline = FitnessApp.Domain.Enums.Discipline.Strength,
            DifficultyLevel = FitnessApp.Domain.Enums.DifficultyLevel.Advanced,
            IntensityLevel = FitnessApp.Domain.Enums.IntensityLevel.High
        };

        // Act
        var exerciseProgression = new ExerciseProgression
        {
            BaseExerciseId = baseExercise.Id,
            BaseExercise = baseExercise,
            RegressionExerciseId = regressionExercise.Id,
            RegressionExercise = regressionExercise,
            ProgressionExerciseId = progressionExercise.Id,
            ProgressionExercise = progressionExercise,
            Notes = "Standard push-up progression chain"
        };

        // Assert
        exerciseProgression.BaseExercise.Should().Be(baseExercise);
        exerciseProgression.RegressionExercise.Should().Be(regressionExercise);
        exerciseProgression.ProgressionExercise.Should().Be(progressionExercise);
        exerciseProgression.Notes.Should().Be("Standard push-up progression chain");
    }

    [Fact]
    public void ExerciseProgression_WithAlternative_ShouldAssignCorrectly()
    {
        // Arrange
        var baseExercise = new Exercise
        {
            Name = "Barbell Row",
            Description = "Bent over barbell row",
            PrimaryDiscipline = FitnessApp.Domain.Enums.Discipline.Strength,
            DifficultyLevel = FitnessApp.Domain.Enums.DifficultyLevel.Intermediate,
            IntensityLevel = FitnessApp.Domain.Enums.IntensityLevel.Moderate
        };

        var alternativeExercise = new Exercise
        {
            Name = "Dumbbell Row",
            Description = "Single arm dumbbell row",
            PrimaryDiscipline = FitnessApp.Domain.Enums.Discipline.Strength,
            DifficultyLevel = FitnessApp.Domain.Enums.DifficultyLevel.Beginner,
            IntensityLevel = FitnessApp.Domain.Enums.IntensityLevel.Moderate
        };

        // Act
        var exerciseProgression = new ExerciseProgression
        {
            BaseExerciseId = baseExercise.Id,
            BaseExercise = baseExercise,
            AlternativeExerciseId = alternativeExercise.Id,
            AlternativeExercise = alternativeExercise
        };

        // Assert
        exerciseProgression.BaseExercise.Should().Be(baseExercise);
        exerciseProgression.AlternativeExercise.Should().Be(alternativeExercise);
        exerciseProgression.RegressionExercise.Should().BeNull();
        exerciseProgression.ProgressionExercise.Should().BeNull();
    }

    [Fact]
    public void ExerciseProgression_CanHaveOnlyRegression()
    {
        // Arrange
        var baseExercise = new Exercise
        {
            Name = "Pistol Squat",
            Description = "Single leg squat",
            PrimaryDiscipline = FitnessApp.Domain.Enums.Discipline.Strength,
            DifficultyLevel = FitnessApp.Domain.Enums.DifficultyLevel.Advanced,
            IntensityLevel = FitnessApp.Domain.Enums.IntensityLevel.High
        };

        var regressionExercise = new Exercise
        {
            Name = "Assisted Pistol Squat",
            Description = "Pistol squat with support",
            PrimaryDiscipline = FitnessApp.Domain.Enums.Discipline.Strength,
            DifficultyLevel = FitnessApp.Domain.Enums.DifficultyLevel.Intermediate,
            IntensityLevel = FitnessApp.Domain.Enums.IntensityLevel.Moderate
        };

        // Act
        var exerciseProgression = new ExerciseProgression
        {
            BaseExerciseId = baseExercise.Id,
            BaseExercise = baseExercise,
            RegressionExerciseId = regressionExercise.Id,
            RegressionExercise = regressionExercise
        };

        // Assert
        exerciseProgression.RegressionExercise.Should().Be(regressionExercise);
        exerciseProgression.ProgressionExercise.Should().BeNull();
        exerciseProgression.AlternativeExercise.Should().BeNull();
    }
}
