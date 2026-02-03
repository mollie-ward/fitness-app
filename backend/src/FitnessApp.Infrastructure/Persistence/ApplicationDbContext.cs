using Microsoft.EntityFrameworkCore;
using FitnessApp.Application.Common.Interfaces;
using FitnessApp.Domain.Entities;
using FitnessApp.Domain.Common;

namespace FitnessApp.Infrastructure.Persistence;

/// <summary>
/// Entity Framework Core database context for the fitness application
/// </summary>
public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    /// <summary>
    /// Initializes a new instance of the ApplicationDbContext
    /// </summary>
    /// <param name="options">Database context options</param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Users in the application
    /// </summary>
    public DbSet<User> Users => Set<User>();

    /// <summary>
    /// User profiles in the application
    /// </summary>
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();

    /// <summary>
    /// Training goals in the application
    /// </summary>
    public DbSet<TrainingGoal> TrainingGoals => Set<TrainingGoal>();

    /// <summary>
    /// Injury limitations in the application
    /// </summary>
    public DbSet<InjuryLimitation> InjuryLimitations => Set<InjuryLimitation>();

    /// <summary>
    /// Training backgrounds in the application
    /// </summary>
    public DbSet<TrainingBackground> TrainingBackgrounds => Set<TrainingBackground>();

    /// <summary>
    /// Exercises in the application
    /// </summary>
    public DbSet<Exercise> Exercises => Set<Exercise>();

    /// <summary>
    /// Muscle groups in the application
    /// </summary>
    public DbSet<MuscleGroup> MuscleGroups => Set<MuscleGroup>();

    /// <summary>
    /// Equipment in the application
    /// </summary>
    public DbSet<Equipment> Equipment => Set<Equipment>();

    /// <summary>
    /// Contraindications in the application
    /// </summary>
    public DbSet<Contraindication> Contraindications => Set<Contraindication>();

    /// <summary>
    /// Exercise progressions in the application
    /// </summary>
    public DbSet<ExerciseProgression> ExerciseProgressions => Set<ExerciseProgression>();

    /// <summary>
    /// Exercise movement patterns in the application
    /// </summary>
    public DbSet<ExerciseMovementPattern> ExerciseMovementPatterns => Set<ExerciseMovementPattern>();

    /// <summary>
    /// Exercise muscle groups in the application
    /// </summary>
    public DbSet<ExerciseMuscleGroup> ExerciseMuscleGroups => Set<ExerciseMuscleGroup>();

    /// <summary>
    /// Exercise equipment in the application
    /// </summary>
    public DbSet<ExerciseEquipment> ExerciseEquipments => Set<ExerciseEquipment>();

    /// <summary>
    /// Exercise contraindications in the application
    /// </summary>
    public DbSet<ExerciseContraindication> ExerciseContraindications => Set<ExerciseContraindication>();

    /// <summary>
    /// Training plans in the application
    /// </summary>
    public DbSet<TrainingPlan> TrainingPlans => Set<TrainingPlan>();

    /// <summary>
    /// Training weeks in the application
    /// </summary>
    public DbSet<TrainingWeek> TrainingWeeks => Set<TrainingWeek>();

    /// <summary>
    /// Workouts in the application
    /// </summary>
    public DbSet<Workout> Workouts => Set<Workout>();

    /// <summary>
    /// Workout exercises in the application
    /// </summary>
    public DbSet<WorkoutExercise> WorkoutExercises => Set<WorkoutExercise>();

    /// <summary>
    /// Plan metadata in the application
    /// </summary>
    public DbSet<PlanMetadata> PlanMetadatas => Set<PlanMetadata>();

    /// <summary>
    /// Completion history records in the application
    /// </summary>
    public DbSet<CompletionHistory> CompletionHistories => Set<CompletionHistory>();

    /// <summary>
    /// User streak information in the application
    /// </summary>
    public DbSet<UserStreak> UserStreaks => Set<UserStreak>();

    /// <summary>
    /// Configures the model and relationships
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure User entity
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(256);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(256);
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // Configure UserProfile entity
        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UserId).IsRequired();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(256);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(256);
            entity.HasIndex(e => e.UserId).IsUnique();

            // Configure owned type for ScheduleAvailability
            entity.OwnsOne(e => e.ScheduleAvailability, sa =>
            {
                sa.Property(s => s.MinimumSessionsPerWeek).IsRequired();
                sa.Property(s => s.MaximumSessionsPerWeek).IsRequired();
            });

            // Configure one-to-one relationship with TrainingBackground
            entity.HasOne(e => e.TrainingBackground)
                .WithOne(tb => tb.UserProfile)
                .HasForeignKey<TrainingBackground>(tb => tb.UserProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure one-to-many relationship with TrainingGoals
            entity.HasMany(e => e.TrainingGoals)
                .WithOne(tg => tg.UserProfile)
                .HasForeignKey(tg => tg.UserProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure one-to-many relationship with InjuryLimitations
            entity.HasMany(e => e.InjuryLimitations)
                .WithOne(il => il.UserProfile)
                .HasForeignKey(il => il.UserProfileId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure TrainingGoal entity
        modelBuilder.Entity<TrainingGoal>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Priority).IsRequired();
            entity.HasIndex(e => e.UserProfileId);
        });

        // Configure InjuryLimitation entity
        modelBuilder.Entity<InjuryLimitation>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.BodyPart).IsRequired().HasMaxLength(100);
            entity.Property(e => e.MovementRestrictions).HasMaxLength(1000);
            entity.HasIndex(e => e.UserProfileId);
        });

        // Configure TrainingBackground entity
        modelBuilder.Entity<TrainingBackground>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PreviousTrainingDetails).HasMaxLength(1000);
            entity.Property(e => e.EquipmentFamiliarity).HasMaxLength(1000);
            entity.Property(e => e.TrainingHistoryDetails).HasMaxLength(1000);
            entity.HasIndex(e => e.UserProfileId).IsUnique();
        });

        // Configure Exercise entity
        modelBuilder.Entity<Exercise>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(2000);
            entity.Property(e => e.Instructions).HasMaxLength(5000);
            entity.Property(e => e.PrimaryDiscipline).IsRequired();
            entity.Property(e => e.DifficultyLevel).IsRequired();
            entity.Property(e => e.IntensityLevel).IsRequired();
            
            // Create indexes for common queries
            entity.HasIndex(e => e.PrimaryDiscipline);
            entity.HasIndex(e => e.DifficultyLevel);
            entity.HasIndex(e => new { e.PrimaryDiscipline, e.DifficultyLevel });
            entity.HasIndex(e => e.Name);
        });

        // Configure MuscleGroup entity
        modelBuilder.Entity<MuscleGroup>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Category).HasMaxLength(100);
            entity.HasIndex(e => e.Name).IsUnique();
        });

        // Configure Equipment entity
        modelBuilder.Entity<Equipment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.HasIndex(e => e.Name).IsUnique();
        });

        // Configure Contraindication entity
        modelBuilder.Entity<Contraindication>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.InjuryType).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.MovementRestriction).HasMaxLength(200);
            entity.HasIndex(e => e.InjuryType);
        });

        // Configure ExerciseProgression entity
        modelBuilder.Entity<ExerciseProgression>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Notes).HasMaxLength(500);
            
            entity.HasOne(e => e.BaseExercise)
                .WithMany(ex => ex.ProgressionsAsBase)
                .HasForeignKey(e => e.BaseExerciseId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.RegressionExercise)
                .WithMany(ex => ex.ProgressionsAsRegression)
                .HasForeignKey(e => e.RegressionExerciseId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.ProgressionExercise)
                .WithMany(ex => ex.ProgressionsAsProgression)
                .HasForeignKey(e => e.ProgressionExerciseId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.AlternativeExercise)
                .WithMany(ex => ex.ProgressionsAsAlternative)
                .HasForeignKey(e => e.AlternativeExerciseId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.BaseExerciseId);
        });

        // Configure ExerciseMovementPattern join entity
        modelBuilder.Entity<ExerciseMovementPattern>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.MovementPattern).IsRequired();
            
            entity.HasOne(e => e.Exercise)
                .WithMany(ex => ex.ExerciseMovementPatterns)
                .HasForeignKey(e => e.ExerciseId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.ExerciseId, e.MovementPattern }).IsUnique();
            entity.HasIndex(e => e.MovementPattern);
        });

        // Configure ExerciseMuscleGroup join entity
        modelBuilder.Entity<ExerciseMuscleGroup>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasOne(e => e.Exercise)
                .WithMany(ex => ex.ExerciseMuscleGroups)
                .HasForeignKey(e => e.ExerciseId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.MuscleGroup)
                .WithMany(mg => mg.ExerciseMuscleGroups)
                .HasForeignKey(e => e.MuscleGroupId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.ExerciseId, e.MuscleGroupId }).IsUnique();
            entity.HasIndex(e => e.MuscleGroupId);
        });

        // Configure ExerciseEquipment join entity
        modelBuilder.Entity<ExerciseEquipment>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasOne(e => e.Exercise)
                .WithMany(ex => ex.ExerciseEquipments)
                .HasForeignKey(e => e.ExerciseId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Equipment)
                .WithMany(eq => eq.ExerciseEquipments)
                .HasForeignKey(e => e.EquipmentId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.ExerciseId, e.EquipmentId }).IsUnique();
            entity.HasIndex(e => e.EquipmentId);
        });

        // Configure ExerciseContraindication join entity
        modelBuilder.Entity<ExerciseContraindication>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Severity).HasMaxLength(50);
            entity.Property(e => e.RecommendedSubstitutes).HasMaxLength(500);
            
            entity.HasOne(e => e.Exercise)
                .WithMany(ex => ex.ExerciseContraindications)
                .HasForeignKey(e => e.ExerciseId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Contraindication)
                .WithMany(c => c.ExerciseContraindications)
                .HasForeignKey(e => e.ContraindicationId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.ExerciseId, e.ContraindicationId }).IsUnique();
            entity.HasIndex(e => e.ContraindicationId);
        });

        // Configure TrainingPlan entity
        modelBuilder.Entity<TrainingPlan>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UserId).IsRequired();
            entity.Property(e => e.PlanName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.StartDate).IsRequired();
            entity.Property(e => e.EndDate).IsRequired();
            entity.Property(e => e.TotalWeeks).IsRequired();
            entity.Property(e => e.TrainingDaysPerWeek).IsRequired();
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.CurrentWeek).IsRequired();
            entity.Property(e => e.IsDeleted).IsRequired().HasDefaultValue(false);
            
            // Relationships
            entity.HasOne(e => e.UserProfile)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.PrimaryGoal)
                .WithMany()
                .HasForeignKey(e => e.PrimaryGoalId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasMany(e => e.TrainingWeeks)
                .WithOne(tw => tw.TrainingPlan)
                .HasForeignKey(tw => tw.PlanId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false);

            entity.HasOne(e => e.PlanMetadata)
                .WithOne(pm => pm.TrainingPlan)
                .HasForeignKey<PlanMetadata>(pm => pm.PlanId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false);

            // Indexes for efficient queries
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => new { e.UserId, e.Status });
            entity.HasIndex(e => e.IsDeleted);
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Configure TrainingWeek entity
        modelBuilder.Entity<TrainingWeek>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PlanId).IsRequired();
            entity.Property(e => e.WeekNumber).IsRequired();
            entity.Property(e => e.Phase).IsRequired();
            entity.Property(e => e.StartDate).IsRequired();
            entity.Property(e => e.EndDate).IsRequired();
            entity.Property(e => e.FocusArea).HasMaxLength(500);

            entity.HasMany(e => e.Workouts)
                .WithOne(w => w.TrainingWeek)
                .HasForeignKey(w => w.WeekId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.PlanId);
            entity.HasIndex(e => new { e.PlanId, e.WeekNumber }).IsUnique();
        });

        // Configure Workout entity
        modelBuilder.Entity<Workout>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.WeekId).IsRequired();
            entity.Property(e => e.DayOfWeek).IsRequired();
            entity.Property(e => e.ScheduledDate).IsRequired();
            entity.Property(e => e.Discipline).IsRequired();
            entity.Property(e => e.WorkoutName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.Property(e => e.IntensityLevel).IsRequired();
            entity.Property(e => e.IsKeyWorkout).IsRequired();
            entity.Property(e => e.CompletionStatus).IsRequired();

            entity.HasMany(e => e.WorkoutExercises)
                .WithOne(we => we.Workout)
                .HasForeignKey(we => we.WorkoutId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.WeekId);
            entity.HasIndex(e => e.ScheduledDate);
            entity.HasIndex(e => new { e.WeekId, e.DayOfWeek });
        });

        // Configure WorkoutExercise join entity
        modelBuilder.Entity<WorkoutExercise>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.WorkoutId).IsRequired();
            entity.Property(e => e.ExerciseId).IsRequired();
            entity.Property(e => e.OrderInWorkout).IsRequired();
            entity.Property(e => e.IntensityGuidance).HasMaxLength(200);
            entity.Property(e => e.Notes).HasMaxLength(1000);

            entity.HasOne(e => e.Exercise)
                .WithMany()
                .HasForeignKey(e => e.ExerciseId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.WorkoutId);
            entity.HasIndex(e => e.ExerciseId);
            entity.HasIndex(e => new { e.WorkoutId, e.OrderInWorkout }).IsUnique();
        });

        // Configure PlanMetadata entity
        modelBuilder.Entity<PlanMetadata>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PlanId).IsRequired();
            entity.Property(e => e.AlgorithmVersion).HasMaxLength(50);

            entity.HasIndex(e => e.PlanId).IsUnique();
        });

        // Configure CompletionHistory entity
        modelBuilder.Entity<CompletionHistory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UserId).IsRequired();
            entity.Property(e => e.WorkoutId).IsRequired();
            entity.Property(e => e.CompletedAt).IsRequired();
            entity.Property(e => e.Notes).HasMaxLength(1000);

            entity.HasOne(e => e.UserProfile)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Workout)
                .WithMany()
                .HasForeignKey(e => e.WorkoutId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.WorkoutId);
            entity.HasIndex(e => new { e.UserId, e.CompletedAt });
            entity.HasIndex(e => e.CompletedAt);
        });

        // Configure UserStreak entity
        modelBuilder.Entity<UserStreak>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UserId).IsRequired();
            entity.Property(e => e.CurrentStreak).IsRequired();
            entity.Property(e => e.LongestStreak).IsRequired();
            entity.Property(e => e.CurrentWeeklyStreak).IsRequired();
            entity.Property(e => e.LongestWeeklyStreak).IsRequired();

            entity.HasOne(e => e.UserProfile)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.UserId).IsUnique();
        });
    }

    /// <summary>
    /// Saves changes and automatically sets audit fields
    /// </summary>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is BaseEntity && (
                e.State == EntityState.Added ||
                e.State == EntityState.Modified));

        foreach (var entityEntry in entries)
        {
            var entity = (BaseEntity)entityEntry.Entity;

            if (entityEntry.State == EntityState.Added)
            {
                entity.CreatedAt = DateTime.UtcNow;
            }
            else
            {
                entity.UpdatedAt = DateTime.UtcNow;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
