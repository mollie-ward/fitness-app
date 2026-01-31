using Xunit;
using FluentAssertions;
using FitnessApp.Domain.Entities;

namespace FitnessApp.UnitTests.Domain.Entities;

public class WorkoutExerciseTests
{
    [Fact]
    public void WorkoutExercise_WithValidData_ShouldCreateSuccessfully()
    {
        // Arrange & Act
        var workoutExercise = new WorkoutExercise
        {
            WorkoutId = Guid.NewGuid(),
            ExerciseId = Guid.NewGuid(),
            OrderInWorkout = 1,
            Sets = 3,
            Reps = 10,
            Duration = null,
            RestPeriod = 90,
            IntensityGuidance = "70% max",
            Notes = "Focus on form"
        };

        // Assert
        workoutExercise.OrderInWorkout.Should().Be(1);
        workoutExercise.Sets.Should().Be(3);
        workoutExercise.Reps.Should().Be(10);
        workoutExercise.RestPeriod.Should().Be(90);
        workoutExercise.IntensityGuidance.Should().Be("70% max");
        workoutExercise.Notes.Should().Be("Focus on form");
    }

    [Fact]
    public void WorkoutExercise_WithDurationBased_ShouldNotRequireReps()
    {
        // Arrange & Act
        var workoutExercise = new WorkoutExercise
        {
            WorkoutId = Guid.NewGuid(),
            ExerciseId = Guid.NewGuid(),
            OrderInWorkout = 1,
            Sets = 3,
            Reps = null,
            Duration = 60,
            RestPeriod = 30,
            IntensityGuidance = "Easy pace"
        };

        // Assert
        workoutExercise.Duration.Should().Be(60);
        workoutExercise.Reps.Should().BeNull();
        workoutExercise.IntensityGuidance.Should().Be("Easy pace");
    }

    [Fact]
    public void WorkoutExercise_OrderInWorkout_ShouldDefineSequence()
    {
        // Arrange & Act
        var exercise1 = new WorkoutExercise
        {
            WorkoutId = Guid.NewGuid(),
            ExerciseId = Guid.NewGuid(),
            OrderInWorkout = 1,
            Sets = 3,
            Reps = 10
        };

        var exercise2 = new WorkoutExercise
        {
            WorkoutId = exercise1.WorkoutId,
            ExerciseId = Guid.NewGuid(),
            OrderInWorkout = 2,
            Sets = 3,
            Reps = 12
        };

        var exercise3 = new WorkoutExercise
        {
            WorkoutId = exercise1.WorkoutId,
            ExerciseId = Guid.NewGuid(),
            OrderInWorkout = 3,
            Sets = 3,
            Reps = 8
        };

        // Assert
        exercise1.OrderInWorkout.Should().BeLessThan(exercise2.OrderInWorkout);
        exercise2.OrderInWorkout.Should().BeLessThan(exercise3.OrderInWorkout);
    }

    [Fact]
    public void WorkoutExercise_IntensityGuidance_CanBeRPE()
    {
        // Arrange & Act
        var workoutExercise = new WorkoutExercise
        {
            WorkoutId = Guid.NewGuid(),
            ExerciseId = Guid.NewGuid(),
            OrderInWorkout = 1,
            Sets = 4,
            Reps = 6,
            IntensityGuidance = "RPE 8"
        };

        // Assert
        workoutExercise.IntensityGuidance.Should().Be("RPE 8");
    }

    [Fact]
    public void WorkoutExercise_RestPeriod_CanVary()
    {
        // Arrange & Act
        var shortRest = new WorkoutExercise
        {
            WorkoutId = Guid.NewGuid(),
            ExerciseId = Guid.NewGuid(),
            OrderInWorkout = 1,
            Sets = 3,
            Reps = 15,
            RestPeriod = 30
        };

        var longRest = new WorkoutExercise
        {
            WorkoutId = Guid.NewGuid(),
            ExerciseId = Guid.NewGuid(),
            OrderInWorkout = 1,
            Sets = 5,
            Reps = 3,
            RestPeriod = 180
        };

        // Assert
        shortRest.RestPeriod.Should().Be(30);
        longRest.RestPeriod.Should().Be(180);
    }

    [Fact]
    public void WorkoutExercise_Notes_CanProvideAdditionalInstructions()
    {
        // Arrange & Act
        var workoutExercise = new WorkoutExercise
        {
            WorkoutId = Guid.NewGuid(),
            ExerciseId = Guid.NewGuid(),
            OrderInWorkout = 1,
            Sets = 3,
            Reps = 10,
            Notes = "Use tempo 3-1-1-0, focus on eccentric phase"
        };

        // Assert
        workoutExercise.Notes.Should().NotBeNullOrEmpty();
        workoutExercise.Notes.Should().Contain("tempo");
    }
}
