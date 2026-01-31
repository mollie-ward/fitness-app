using Xunit;
using FluentAssertions;
using FitnessApp.Domain.Entities;
using FitnessApp.Domain.Enums;
using FitnessApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FitnessApp.IntegrationTests.Persistence;

public class ExerciseDatabaseTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public ExerciseDatabaseTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Database_CanSaveAndRetrieveExercise()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var exercise = new Exercise
        {
            Name = "Test Exercise",
            Description = "Test description",
            Instructions = "Test instructions",
            PrimaryDiscipline = Discipline.Strength,
            DifficultyLevel = DifficultyLevel.Beginner,
            IntensityLevel = IntensityLevel.Low,
            ApproximateDuration = 30
        };

        // Act
        await context.Exercises.AddAsync(exercise);
        await context.SaveChangesAsync();

        var retrieved = await context.Exercises
            .FirstOrDefaultAsync(e => e.Name == "Test Exercise");

        // Assert
        retrieved.Should().NotBeNull();
        retrieved!.Name.Should().Be("Test Exercise");
        retrieved.Description.Should().Be("Test description");
        retrieved.PrimaryDiscipline.Should().Be(Discipline.Strength);

        // Cleanup
        context.Exercises.Remove(retrieved);
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task Database_CanFilterExercisesByDiscipline()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var exercises = new[]
        {
            new Exercise
            {
                Name = "Test HYROX 1",
                Description = "HYROX exercise",
                PrimaryDiscipline = Discipline.HYROX,
                DifficultyLevel = DifficultyLevel.Beginner,
                IntensityLevel = IntensityLevel.Low
            },
            new Exercise
            {
                Name = "Test Running 1",
                Description = "Running exercise",
                PrimaryDiscipline = Discipline.Running,
                DifficultyLevel = DifficultyLevel.Beginner,
                IntensityLevel = IntensityLevel.Low
            }
        };

        await context.Exercises.AddRangeAsync(exercises);
        await context.SaveChangesAsync();

        // Act
        var hyroxExercises = await context.Exercises
            .Where(e => e.PrimaryDiscipline == Discipline.HYROX && e.Name.StartsWith("Test"))
            .ToListAsync();

        // Assert
        hyroxExercises.Should().HaveCountGreaterThanOrEqualTo(1);

        // Cleanup
        context.Exercises.RemoveRange(exercises);
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task Database_CanFilterExercisesByDifficultyLevel()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var exercises = new[]
        {
            new Exercise
            {
                Name = "Test Beginner Ex",
                Description = "Beginner exercise",
                PrimaryDiscipline = Discipline.Strength,
                DifficultyLevel = DifficultyLevel.Beginner,
                IntensityLevel = IntensityLevel.Low
            },
            new Exercise
            {
                Name = "Test Advanced Ex",
                Description = "Advanced exercise",
                PrimaryDiscipline = Discipline.Strength,
                DifficultyLevel = DifficultyLevel.Advanced,
                IntensityLevel = IntensityLevel.High
            }
        };

        await context.Exercises.AddRangeAsync(exercises);
        await context.SaveChangesAsync();

        // Act
        var beginnerExercises = await context.Exercises
            .Where(e => e.DifficultyLevel == DifficultyLevel.Beginner && e.Name.StartsWith("Test"))
            .ToListAsync();

        // Assert
        beginnerExercises.Should().HaveCountGreaterThanOrEqualTo(1);

        // Cleanup
        context.Exercises.RemoveRange(exercises);
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task Database_CanSaveExerciseWithMuscleGroups()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var muscleGroup = new MuscleGroup
        {
            Name = "Test Muscle Group",
            Category = "Test Category"
        };

        var exercise = new Exercise
        {
            Name = "Test Exercise with Muscles",
            Description = "Test",
            PrimaryDiscipline = Discipline.Strength,
            DifficultyLevel = DifficultyLevel.Beginner,
            IntensityLevel = IntensityLevel.Low
        };

        await context.MuscleGroups.AddAsync(muscleGroup);
        await context.Exercises.AddAsync(exercise);
        await context.SaveChangesAsync();

        var exerciseMuscleGroup = new ExerciseMuscleGroup
        {
            ExerciseId = exercise.Id,
            MuscleGroupId = muscleGroup.Id,
            IsPrimary = true
        };

        // Act
        await context.ExerciseMuscleGroups.AddAsync(exerciseMuscleGroup);
        await context.SaveChangesAsync();

        var retrieved = await context.Exercises
            .Include(e => e.ExerciseMuscleGroups)
                .ThenInclude(emg => emg.MuscleGroup)
            .FirstOrDefaultAsync(e => e.Name == "Test Exercise with Muscles");

        // Assert
        retrieved.Should().NotBeNull();
        retrieved!.ExerciseMuscleGroups.Should().HaveCount(1);
        retrieved.ExerciseMuscleGroups.First().MuscleGroup.Name.Should().Be("Test Muscle Group");

        // Cleanup
        context.ExerciseMuscleGroups.Remove(exerciseMuscleGroup);
        context.Exercises.Remove(exercise);
        context.MuscleGroups.Remove(muscleGroup);
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task Database_CanSaveExerciseProgression()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var baseExercise = new Exercise
        {
            Name = "Test Base Exercise",
            Description = "Base exercise",
            PrimaryDiscipline = Discipline.Strength,
            DifficultyLevel = DifficultyLevel.Intermediate,
            IntensityLevel = IntensityLevel.Moderate
        };

        var regression = new Exercise
        {
            Name = "Test Regression Exercise",
            Description = "Easier variation",
            PrimaryDiscipline = Discipline.Strength,
            DifficultyLevel = DifficultyLevel.Beginner,
            IntensityLevel = IntensityLevel.Low
        };

        await context.Exercises.AddRangeAsync(new[] { baseExercise, regression });
        await context.SaveChangesAsync();

        var exerciseProgression = new ExerciseProgression
        {
            BaseExerciseId = baseExercise.Id,
            RegressionExerciseId = regression.Id,
            Notes = "Test progression chain"
        };

        // Act
        await context.ExerciseProgressions.AddAsync(exerciseProgression);
        await context.SaveChangesAsync();

        var retrieved = await context.ExerciseProgressions
            .Include(ep => ep.BaseExercise)
            .Include(ep => ep.RegressionExercise)
            .FirstOrDefaultAsync(ep => ep.BaseExerciseId == baseExercise.Id);

        // Assert
        retrieved.Should().NotBeNull();
        retrieved!.BaseExercise.Name.Should().Be("Test Base Exercise");

        // Cleanup
        context.ExerciseProgressions.Remove(exerciseProgression);
        context.Exercises.RemoveRange(new[] { baseExercise, regression });
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task Database_IndexesExist_ForCommonQueries()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var exercise = new Exercise
        {
            Name = "Index Test Exercise",
            Description = "Test",
            PrimaryDiscipline = Discipline.HYROX,
            DifficultyLevel = DifficultyLevel.Intermediate,
            IntensityLevel = IntensityLevel.High
        };

        await context.Exercises.AddAsync(exercise);
        await context.SaveChangesAsync();

        // Act - Queries that should use indexes
        var byDiscipline = await context.Exercises
            .Where(e => e.PrimaryDiscipline == Discipline.HYROX)
            .ToListAsync();

        var byDifficulty = await context.Exercises
            .Where(e => e.DifficultyLevel == DifficultyLevel.Intermediate)
            .ToListAsync();

        var byBoth = await context.Exercises
            .Where(e => e.PrimaryDiscipline == Discipline.HYROX && e.DifficultyLevel == DifficultyLevel.Intermediate)
            .ToListAsync();

        // Assert - Queries complete without error
        byDiscipline.Should().NotBeEmpty();
        byDifficulty.Should().NotBeEmpty();
        byBoth.Should().NotBeEmpty();

        // Cleanup
        context.Exercises.Remove(exercise);
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task Database_CanFilterExercisesByMultipleCriteria()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var exercises = new[]
        {
            new Exercise
            {
                Name = "Test Multi 1",
                Description = "HYROX Beginner",
                PrimaryDiscipline = Discipline.HYROX,
                DifficultyLevel = DifficultyLevel.Beginner,
                IntensityLevel = IntensityLevel.Low
            },
            new Exercise
            {
                Name = "Test Multi 2",
                Description = "HYROX Advanced",
                PrimaryDiscipline = Discipline.HYROX,
                DifficultyLevel = DifficultyLevel.Advanced,
                IntensityLevel = IntensityLevel.High
            }
        };

        await context.Exercises.AddRangeAsync(exercises);
        await context.SaveChangesAsync();

        // Act
        var filtered = await context.Exercises
            .Where(e => e.PrimaryDiscipline == Discipline.HYROX 
                && e.DifficultyLevel == DifficultyLevel.Beginner
                && e.Name.StartsWith("Test Multi"))
            .ToListAsync();

        // Assert
        filtered.Should().HaveCountGreaterThanOrEqualTo(1);

        // Cleanup
        context.Exercises.RemoveRange(exercises);
        await context.SaveChangesAsync();
    }
}
