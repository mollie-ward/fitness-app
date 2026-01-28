using Xunit;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FitnessApp.Infrastructure;
using FitnessApp.Infrastructure.Persistence;
using FitnessApp.Application.Common.Interfaces;

namespace FitnessApp.UnitTests.Infrastructure;

public class DependencyInjectionTests
{
    [Fact]
    public void AddInfrastructure_ShouldRegisterApplicationDbContext()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Database=test;Username=test;Password=test"
            })
            .Build();

        // Act
        services.AddInfrastructure(configuration);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var dbContext = serviceProvider.GetService<ApplicationDbContext>();
        dbContext.Should().NotBeNull();
    }

    [Fact]
    public void AddInfrastructure_ShouldRegisterIApplicationDbContext()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Database=test;Username=test;Password=test"
            })
            .Build();

        // Act
        services.AddInfrastructure(configuration);

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var dbContext = serviceProvider.GetService<IApplicationDbContext>();
        dbContext.Should().NotBeNull();
    }

    [Fact]
    public void AddInfrastructure_WithMissingConnectionString_ShouldThrowException()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>())
            .Build();

        // Act & Assert
        var act = () => services.AddInfrastructure(configuration);
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*connection string*");
    }

    [Fact]
    public void AddInfrastructure_ShouldRegisterDbContextAsScoped()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Database=test;Username=test;Password=test"
            })
            .Build();

        // Act
        services.AddInfrastructure(configuration);

        // Assert
        var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(ApplicationDbContext));
        descriptor.Should().NotBeNull();
        descriptor!.Lifetime.Should().Be(ServiceLifetime.Scoped);
    }
}
