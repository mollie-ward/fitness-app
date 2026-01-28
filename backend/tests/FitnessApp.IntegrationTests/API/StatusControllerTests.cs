using System.Net;
using System.Net.Http.Json;
using Xunit;
using FluentAssertions;

namespace FitnessApp.IntegrationTests.API;

public class StatusControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public StatusControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetStatus_ShouldReturnOk()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/status");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetStatus_ShouldReturnValidJson()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/status");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        content.Should().Contain("status");
        content.Should().Contain("timestamp");
    }

    [Fact]
    public async Task GetStatus_Error_ShouldReturnInternalServerError()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/status/error");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task GetStatus_Error_ShouldReturnJsonErrorResponse()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/status/error");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        content.Should().Contain("message");
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");
    }
}
