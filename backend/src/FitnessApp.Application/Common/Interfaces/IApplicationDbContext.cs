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
    /// Exercises in the application
    /// </summary>
    DbSet<Exercise> Exercises { get; }

    /// <summary>
    /// Exercise progressions in the application
    /// </summary>
    DbSet<ExerciseProgression> ExerciseProgressions { get; }

    /// <summary>
    /// Exercise movement patterns in the application
    /// </summary>
    DbSet<ExerciseMovementPattern> ExerciseMovementPatterns { get; }

    /// <summary>
    /// Exercise contraindications in the application
    /// </summary>
    DbSet<ExerciseContraindication> ExerciseContraindications { get; }

    /// <summary>
    /// Contraindications in the application
    /// </summary>
    DbSet<Contraindication> Contraindications { get; }

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
    /// Completion history records in the application
    /// </summary>
    DbSet<CompletionHistory> CompletionHistories { get; }

    /// <summary>
    /// User streak information in the application
    /// </summary>
    DbSet<UserStreak> UserStreaks { get; }

    /// <summary>
    /// Plan adaptations in the application
    /// </summary>
    DbSet<PlanAdaptation> PlanAdaptations { get; }

    /// <summary>
    /// Refresh tokens in the application
    /// </summary>
    DbSet<RefreshToken> RefreshTokens { get; }

    /// <summary>
    /// Email verification tokens in the application
    /// </summary>
    DbSet<EmailVerificationToken> EmailVerificationTokens { get; }

    /// <summary>
    /// Password reset tokens in the application
    /// </summary>
    DbSet<PasswordResetToken> PasswordResetTokens { get; }

    /// <summary>
    /// Saves all changes made in this context to the database
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Number of state entries written to the database</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
