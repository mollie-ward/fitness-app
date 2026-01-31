using Xunit;
using FluentAssertions;
using FitnessApp.Domain.Entities;
using FitnessApp.Domain.Enums;

namespace FitnessApp.UnitTests.Domain.Entities;

public class ExerciseTests
{
    [Fact]
    public void Exercise_WithValidData_ShouldCreateSuccessfully()
    {
        // Arrange & Act
        var exercise = new Exercise
        {
            Name = "Back Squat",
            Description = "Barbell back squat",
            Instructions = "Bar on upper back, squat to depth",
            PrimaryDiscipline = Discipline.Strength,
            DifficultyLevel = DifficultyLevel.Intermediate,
            IntensityLevel = IntensityLevel.High,
            ApproximateDuration = 30,
            SessionType = SessionType.FullBody
        };

        // Assert
        exercise.Name.Should().Be("Back Squat");
        exercise.Description.Should().Be("Barbell back squat");
        exercise.PrimaryDiscipline.Should().Be(Discipline.Strength);
        exercise.DifficultyLevel.Should().Be(DifficultyLevel.Intermediate);
        exercise.IntensityLevel.Should().Be(IntensityLevel.High);
        exercise.ApproximateDuration.Should().Be(30);
        exercise.SessionType.Should().Be(SessionType.FullBody);
    }

    [Fact]
    public void Exercise_ShouldInitializeCollections()
    {
        // Arrange & Act
        var exercise = new Exercise
        {
            Name = "Ski Erg Intervals",
            Description = "High intensity ski erg",
            PrimaryDiscipline = Discipline.HYROX,
            DifficultyLevel = DifficultyLevel.Advanced,
            IntensityLevel = IntensityLevel.Maximum
        };

        // Assert
        exercise.ExerciseMovementPatterns.Should().NotBeNull().And.BeEmpty();
        exercise.ExerciseMuscleGroups.Should().NotBeNull().And.BeEmpty();
        exercise.ExerciseEquipments.Should().NotBeNull().And.BeEmpty();
        exercise.ExerciseContraindications.Should().NotBeNull().And.BeEmpty();
        exercise.ProgressionsAsBase.Should().NotBeNull().And.BeEmpty();
        exercise.ProgressionsAsRegression.Should().NotBeNull().And.BeEmpty();
        exercise.ProgressionsAsProgression.Should().NotBeNull().And.BeEmpty();
        exercise.ProgressionsAsAlternative.Should().NotBeNull().And.BeEmpty();
    }

    [Theory]
    [InlineData(Discipline.HYROX, SessionType.StationPractice)]
    [InlineData(Discipline.Running, SessionType.Intervals)]
    [InlineData(Discipline.Strength, SessionType.FullBody)]
    [InlineData(Discipline.Hybrid, SessionType.HybridConditioning)]
    public void Exercise_WithDifferentDisciplines_ShouldAssignCorrectly(Discipline discipline, SessionType sessionType)
    {
        // Arrange & Act
        var exercise = new Exercise
        {
            Name = "Test Exercise",
            Description = "Test",
            PrimaryDiscipline = discipline,
            DifficultyLevel = DifficultyLevel.Beginner,
            IntensityLevel = IntensityLevel.Low,
            SessionType = sessionType
        };

        // Assert
        exercise.PrimaryDiscipline.Should().Be(discipline);
        exercise.SessionType.Should().Be(sessionType);
    }

    [Theory]
    [InlineData(DifficultyLevel.Beginner, IntensityLevel.Low)]
    [InlineData(DifficultyLevel.Intermediate, IntensityLevel.Moderate)]
    [InlineData(DifficultyLevel.Advanced, IntensityLevel.High)]
    public void Exercise_WithDifferentLevels_ShouldAssignCorrectly(DifficultyLevel difficulty, IntensityLevel intensity)
    {
        // Arrange & Act
        var exercise = new Exercise
        {
            Name = "Test Exercise",
            Description = "Test",
            PrimaryDiscipline = Discipline.Strength,
            DifficultyLevel = difficulty,
            IntensityLevel = intensity
        };

        // Assert
        exercise.DifficultyLevel.Should().Be(difficulty);
        exercise.IntensityLevel.Should().Be(intensity);
    }

    [Fact]
    public void Exercise_CanAddMovementPatterns()
    {
        // Arrange
        var exercise = new Exercise
        {
            Name = "Bench Press",
            Description = "Barbell bench press",
            PrimaryDiscipline = Discipline.Strength,
            DifficultyLevel = DifficultyLevel.Intermediate,
            IntensityLevel = IntensityLevel.High
        };

        var movementPattern = new ExerciseMovementPattern
        {
            ExerciseId = exercise.Id,
            Exercise = exercise,
            MovementPattern = MovementPattern.Push,
            IsPrimary = true
        };

        // Act
        exercise.ExerciseMovementPatterns.Add(movementPattern);

        // Assert
        exercise.ExerciseMovementPatterns.Should().HaveCount(1);
        exercise.ExerciseMovementPatterns.First().MovementPattern.Should().Be(MovementPattern.Push);
        exercise.ExerciseMovementPatterns.First().IsPrimary.Should().BeTrue();
    }

    [Fact]
    public void Exercise_CanAddMuscleGroups()
    {
        // Arrange
        var exercise = new Exercise
        {
            Name = "Deadlift",
            Description = "Conventional deadlift",
            PrimaryDiscipline = Discipline.Strength,
            DifficultyLevel = DifficultyLevel.Intermediate,
            IntensityLevel = IntensityLevel.High
        };

        var muscleGroup = new MuscleGroup
        {
            Name = "Hamstrings",
            Category = "Lower Body"
        };

        var exerciseMuscleGroup = new ExerciseMuscleGroup
        {
            ExerciseId = exercise.Id,
            Exercise = exercise,
            MuscleGroupId = muscleGroup.Id,
            MuscleGroup = muscleGroup,
            IsPrimary = true
        };

        // Act
        exercise.ExerciseMuscleGroups.Add(exerciseMuscleGroup);

        // Assert
        exercise.ExerciseMuscleGroups.Should().HaveCount(1);
        exercise.ExerciseMuscleGroups.First().MuscleGroup.Name.Should().Be("Hamstrings");
        exercise.ExerciseMuscleGroups.First().IsPrimary.Should().BeTrue();
    }

    [Fact]
    public void Exercise_CanAddEquipment()
    {
        // Arrange
        var exercise = new Exercise
        {
            Name = "Ski Erg Intervals",
            Description = "High intensity intervals",
            PrimaryDiscipline = Discipline.HYROX,
            DifficultyLevel = DifficultyLevel.Intermediate,
            IntensityLevel = IntensityLevel.High
        };

        var equipment = new Equipment
        {
            Name = "Ski Erg",
            Description = "Ski ergometer"
        };

        var exerciseEquipment = new ExerciseEquipment
        {
            ExerciseId = exercise.Id,
            Exercise = exercise,
            EquipmentId = equipment.Id,
            Equipment = equipment,
            IsRequired = true
        };

        // Act
        exercise.ExerciseEquipments.Add(exerciseEquipment);

        // Assert
        exercise.ExerciseEquipments.Should().HaveCount(1);
        exercise.ExerciseEquipments.First().Equipment.Name.Should().Be("Ski Erg");
        exercise.ExerciseEquipments.First().IsRequired.Should().BeTrue();
    }

    [Fact]
    public void Exercise_CanAddContraindications()
    {
        // Arrange
        var exercise = new Exercise
        {
            Name = "Overhead Press",
            Description = "Standing barbell press",
            PrimaryDiscipline = Discipline.Strength,
            DifficultyLevel = DifficultyLevel.Intermediate,
            IntensityLevel = IntensityLevel.High
        };

        var contraindication = new Contraindication
        {
            InjuryType = "Shoulder",
            MovementRestriction = "Overhead"
        };

        var exerciseContraindication = new ExerciseContraindication
        {
            ExerciseId = exercise.Id,
            Exercise = exercise,
            ContraindicationId = contraindication.Id,
            Contraindication = contraindication,
            Severity = "Absolute"
        };

        // Act
        exercise.ExerciseContraindications.Add(exerciseContraindication);

        // Assert
        exercise.ExerciseContraindications.Should().HaveCount(1);
        exercise.ExerciseContraindications.First().Contraindication.InjuryType.Should().Be("Shoulder");
        exercise.ExerciseContraindications.First().Severity.Should().Be("Absolute");
    }
}
