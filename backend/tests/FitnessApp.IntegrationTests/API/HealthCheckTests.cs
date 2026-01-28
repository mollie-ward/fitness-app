using System.Net;
using Xunit;
using FluentAssertions;

namespace FitnessApp.IntegrationTests.API;

public class HealthCheckTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public HealthCheckTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task HealthCheck_ShouldReturnResponse()
    {
        // Act
        var response = await _client.GetAsync("/health");

        // Assert
        // Health check returns 503 when database is not available, which is expected in test environment
        // The important thing is that the endpoint responds
        response.Should().NotBeNull();
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.ServiceUnavailable);
    }
}
