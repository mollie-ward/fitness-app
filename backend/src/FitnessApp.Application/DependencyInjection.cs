using Microsoft.Extensions.DependencyInjection;
using FitnessApp.Application.Common.Interfaces;
using FitnessApp.Application.Services;

namespace FitnessApp.Application;

/// <summary>
/// Dependency injection configuration for the Application layer
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers Application layer services
    /// </summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register Services
        services.AddScoped<ITrainingPlanGenerationService, TrainingPlanGenerationService>();

        return services;
    }
}
