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
