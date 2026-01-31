using Microsoft.EntityFrameworkCore;
using FitnessApp.Domain.Entities;

namespace FitnessApp.Application.Common.Interfaces;

/// <summary>
/// Interface for the application database context
/// </summary>
public interface IApplicationDbContext
{
    /// <summary>
    /// Users in the application
    /// </summary>
    DbSet<User> Users { get; }

    /// <summary>
    /// User profiles in the application
    /// </summary>
    DbSet<UserProfile> UserProfiles { get; }

    /// <summary>
    /// Training goals in the application
    /// </summary>
    DbSet<TrainingGoal> TrainingGoals { get; }

    /// <summary>
    /// Injury limitations in the application
    /// </summary>
    DbSet<InjuryLimitation> InjuryLimitations { get; }

    /// <summary>
    /// Training backgrounds in the application
    /// </summary>
    DbSet<TrainingBackground> TrainingBackgrounds { get; }

    /// <summary>
    /// Training plans in the application
    /// </summary>
    DbSet<TrainingPlan> TrainingPlans { get; }

    /// <summary>
    /// Training weeks in the application
    /// </summary>
    DbSet<TrainingWeek> TrainingWeeks { get; }

    /// <summary>
    /// Workouts in the application
    /// </summary>
    DbSet<Workout> Workouts { get; }

    /// <summary>
    /// Workout exercises in the application
    /// </summary>
    DbSet<WorkoutExercise> WorkoutExercises { get; }

    /// <summary>
    /// Plan metadata in the application
    /// </summary>
    DbSet<PlanMetadata> PlanMetadatas { get; }

    /// <summary>
    /// Saves all changes made in this context to the database
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Number of state entries written to the database</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
