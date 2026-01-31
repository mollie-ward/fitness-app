using FitnessApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FitnessApp.Infrastructure.Persistence.SeedData;

/// <summary>
/// Extension methods for seeding exercise database
/// </summary>
public static class ExerciseSeedDataExtensions
{
    /// <summary>
    /// Seeds the exercise database if it's empty
    /// </summary>
    public static async Task SeedExerciseDatabaseAsync(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

        try
        {
            // Check if database already has exercises
            if (await context.Exercises.AnyAsync())
            {
                logger.LogInformation("Exercise database already seeded, skipping");
                return;
            }

            logger.LogInformation("Starting exercise database seeding");

            // Seed in order: Tags first, then exercises, then relationships
            await ExerciseSeedData.SeedAllAsync(context);

            await context.SaveChangesAsync();
            logger.LogInformation("Exercise database seeded successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error seeding exercise database");
            throw;
        }
    }
}
