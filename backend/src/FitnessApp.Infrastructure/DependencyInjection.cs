using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FitnessApp.Application.Common.Interfaces;
using FitnessApp.Infrastructure.Persistence;
using FitnessApp.Infrastructure.Repositories;
using FitnessApp.Infrastructure.AI;
using FitnessApp.Infrastructure.Services;

namespace FitnessApp.Infrastructure;

/// <summary>
/// Dependency injection configuration for the Infrastructure layer
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers Infrastructure layer services
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Register Database Context
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Database connection string 'DefaultConnection' is not configured.");
        }

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorCodesToAdd: null);
            });
        });

        services.AddScoped<IApplicationDbContext>(provider => 
            provider.GetRequiredService<ApplicationDbContext>());

        // Register Repositories
        services.AddScoped<IUserProfileRepository, UserProfileRepository>();
        services.AddScoped<ITrainingPlanRepository, TrainingPlanRepository>();
        services.AddScoped<IWorkoutRepository, WorkoutRepository>();
        services.AddScoped<IExerciseRepository, ExerciseRepository>();
        services.AddScoped<ICompletionHistoryRepository, CompletionHistoryRepository>();
        services.AddScoped<IUserStreakRepository, UserStreakRepository>();
        services.AddScoped<IConversationRepository, ConversationRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();
        services.AddScoped<IPlanAdaptationRepository, PlanAdaptationRepository>();

        // Register Azure OpenAI Configuration
        services.Configure<AzureOpenAISettings>(
            configuration.GetSection(AzureOpenAISettings.SectionName));

        // Register LLM Client
        services.AddScoped<ILLMClient, AzureOpenAILLMClient>();

        // Register Services
        services.AddScoped<ITrainingPlanGenerationService, Application.Services.TrainingPlanGenerationService>();
        services.AddScoped<IProgressTrackingService, Application.Services.ProgressTrackingService>();
        services.AddScoped<IPlanAdaptationService, Application.Services.PlanAdaptationService>();
        services.AddScoped<IAuthenticationService, Application.Services.AuthenticationService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IEmailService, EmailService>();

        return services;
    }
}
