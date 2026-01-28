using Xunit;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using FitnessApp.API.Configuration;

namespace FitnessApp.UnitTests.Configuration;

public class ConfigurationTests
{
    [Fact]
    public void JwtSettings_ShouldLoadFromConfiguration()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["JwtSettings:Secret"] = "TestSecretKey123456789012345678901234567890",
                ["JwtSettings:Issuer"] = "TestIssuer",
                ["JwtSettings:Audience"] = "TestAudience",
                ["JwtSettings:ExpirationInMinutes"] = "120"
            })
            .Build();

        // Act
        var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();

        // Assert
        jwtSettings.Should().NotBeNull();
        jwtSettings!.Secret.Should().Be("TestSecretKey123456789012345678901234567890");
        jwtSettings.Issuer.Should().Be("TestIssuer");
        jwtSettings.Audience.Should().Be("TestAudience");
        jwtSettings.ExpirationInMinutes.Should().Be(120);
    }

    [Fact]
    public void JwtSettings_ShouldHaveDefaultExpirationTime()
    {
        // Arrange
        var jwtSettings = new JwtSettings
        {
            Secret = "Test",
            Issuer = "Test",
            Audience = "Test"
        };

        // Assert
        jwtSettings.ExpirationInMinutes.Should().Be(60);
    }

    [Fact]
    public void CorsSettings_ShouldLoadFromConfiguration()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["CorsSettings:AllowedOrigins:0"] = "http://localhost:3000",
                ["CorsSettings:AllowedOrigins:1"] = "http://localhost:4200"
            })
            .Build();

        // Act
        var corsSettings = configuration.GetSection("CorsSettings").Get<CorsSettings>();

        // Assert
        corsSettings.Should().NotBeNull();
        corsSettings!.AllowedOrigins.Should().HaveCount(2);
        corsSettings.AllowedOrigins.Should().Contain("http://localhost:3000");
        corsSettings.AllowedOrigins.Should().Contain("http://localhost:4200");
    }

    [Fact]
    public void CorsSettings_ShouldHaveEmptyArrayByDefault()
    {
        // Arrange & Act
        var corsSettings = new CorsSettings();

        // Assert
        corsSettings.AllowedOrigins.Should().NotBeNull();
        corsSettings.AllowedOrigins.Should().BeEmpty();
    }

    [Fact]
    public void ConnectionString_ShouldBeAccessibleFromConfiguration()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Database=testdb;Username=test;Password=test"
            })
            .Build();

        // Act
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        // Assert
        connectionString.Should().NotBeNullOrEmpty();
        connectionString.Should().Contain("Database=testdb");
    }
}
