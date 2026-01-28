using System.Net;
using Xunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using FitnessApp.Infrastructure.Persistence;

namespace FitnessApp.IntegrationTests.API;

public class DatabaseTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public DatabaseTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task DatabaseContext_ShouldBeAccessible()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Act
        var canConnect = await context.Database.CanConnectAsync();

        // Assert
        canConnect.Should().BeTrue();
    }

    [Fact]
    public async Task Database_ShouldHaveUsersTable()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Act
        var users = context.Users.ToList();

        // Assert
        users.Should().NotBeNull();
    }
}
