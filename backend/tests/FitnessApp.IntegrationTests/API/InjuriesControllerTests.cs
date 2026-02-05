using System.Net;
using System.Net.Http.Json;
using Xunit;
using FluentAssertions;
using FitnessApp.Application.DTOs;
using FitnessApp.Domain.Enums;
using FitnessApp.IntegrationTests.Helpers;

namespace FitnessApp.IntegrationTests.API;

/// <summary>
/// Integration tests for InjuriesController
/// Tests injury reporting, status updates, contraindications, and substitutions
/// </summary>
public class InjuriesControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly Guid _testUserId;

    public InjuriesControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _testUserId = Guid.NewGuid();
    }

    private HttpClient CreateAuthenticatedClient(Guid? userId = null)
    {
        var client = _factory.CreateClient();
        var effectiveUserId = userId ?? _testUserId;
        client.DefaultRequestHeaders.Add("X-Test-UserId", effectiveUserId.ToString());
        return client;
    }

    private async Task CreateUserProfile(HttpClient client)
    {
        var profileDto = new UserProfileDto
        {
            Name = "Test User",
            Email = "test@example.com",
            HyroxLevel = FitnessLevel.Intermediate,
            RunningLevel = FitnessLevel.Intermediate,
            StrengthLevel = FitnessLevel.Intermediate,
            ScheduleAvailability = new ScheduleAvailabilityDto
            {
                Monday = true,
                Wednesday = true,
                Friday = true,
                MinimumSessionsPerWeek = 2,
                MaximumSessionsPerWeek = 3
            },
            TrainingBackground = new TrainingBackgroundDto
            {
                HasStructuredTrainingExperience = true,
                PreviousTrainingDetails = "Some training history",
                EquipmentFamiliarity = "Basic gym equipment"
            }
        };

        var response = await client.PostAsJsonAsync("/api/v1/users/profile", profileDto);
        response.EnsureSuccessStatusCode();
    }

    #region POST /api/v1/injuries Tests

    [Fact]
    public async Task ReportInjury_WithValidData_ShouldReturnCreated()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        await CreateUserProfile(client);

        var injuryDto = new ReportInjuryDto
        {
            BodyPart = "Shoulder",
            InjuryType = InjuryType.Acute,
            Severity = "Moderate",
            MovementRestrictions = "Overhead"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/injuries", injuryDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("injury");
        content.Should().Contain("disclaimer");
        content.Should().Contain("Medical Disclaimer");
    }

    [Fact]
    public async Task ReportInjury_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var unauthClient = _factory.CreateClient();
        var injuryDto = new ReportInjuryDto
        {
            BodyPart = "Knee",
            InjuryType = InjuryType.Acute
        };

        // Act
        var response = await unauthClient.PostAsJsonAsync("/api/v1/injuries", injuryDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ReportInjury_WithSevereInjury_ShouldIncludeWarningRecommendations()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        await CreateUserProfile(client);

        var injuryDto = new ReportInjuryDto
        {
            BodyPart = "Back",
            InjuryType = InjuryType.Acute,
            Severity = "Severe",
            PainDescription = "Sharp",
            MovementRestrictions = "Rotation, Heavy Load"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/injuries", injuryDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("recommendations");
        content.Should().Contain("Severe");
    }

    #endregion

    #region PUT /api/v1/injuries/{injuryId}/status Tests

    [Fact]
    public async Task UpdateInjuryStatus_WithValidData_ShouldReturnOk()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        await CreateUserProfile(client);

        // Create injury first
        var injuryDto = new ReportInjuryDto
        {
            BodyPart = "Knee",
            InjuryType = InjuryType.Acute
        };
        var createResponse = await client.PostAsJsonAsync("/api/v1/injuries", injuryDto);
        createResponse.EnsureSuccessStatusCode();
        
        // Get the injury ID from response
        var createContent = await createResponse.Content.ReadAsStringAsync();
        var createJson = System.Text.Json.JsonDocument.Parse(createContent);
        var injuryId = Guid.Parse(createJson.RootElement.GetProperty("injury").GetProperty("id").GetString()!);

        var statusUpdate = new UpdateInjuryStatusDto
        {
            Status = InjuryStatus.Improving,
            Notes = "Feeling better after rest"
        };

        // Act
        var response = await client.PutAsJsonAsync($"/api/v1/injuries/{injuryId}/status", statusUpdate);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedInjury = await response.Content.ReadFromJsonAsync<InjuryLimitationDto>();
        updatedInjury.Should().NotBeNull();
        updatedInjury!.Status.Should().Be(InjuryStatus.Improving);
    }

    [Fact]
    public async Task UpdateInjuryStatus_WithUnauthorizedUser_ShouldReturnForbidden()
    {
        // Arrange
        var client1 = CreateAuthenticatedClient(Guid.NewGuid());
        await CreateUserProfile(client1);

        // Create injury with user 1
        var injuryDto = new ReportInjuryDto
        {
            BodyPart = "Ankle",
            InjuryType = InjuryType.Acute
        };
        var createResponse = await client1.PostAsJsonAsync("/api/v1/injuries", injuryDto);
        createResponse.EnsureSuccessStatusCode();
        
        var createContent = await createResponse.Content.ReadAsStringAsync();
        var createJson = System.Text.Json.JsonDocument.Parse(createContent);
        var injuryId = Guid.Parse(createJson.RootElement.GetProperty("injury").GetProperty("id").GetString()!);

        // Try to update with different user
        var client2 = CreateAuthenticatedClient(Guid.NewGuid());
        await CreateUserProfile(client2);

        var statusUpdate = new UpdateInjuryStatusDto
        {
            Status = InjuryStatus.Resolved
        };

        // Act
        var response = await client2.PutAsJsonAsync($"/api/v1/injuries/{injuryId}/status", statusUpdate);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    #endregion

    #region PUT /api/v1/injuries/{injuryId}/resolve Tests

    [Fact]
    public async Task MarkInjuryResolved_WithValidInjury_ShouldReturnOk()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        await CreateUserProfile(client);

        // Create injury first
        var injuryDto = new ReportInjuryDto
        {
            BodyPart = "Shoulder",
            InjuryType = InjuryType.Acute
        };
        var createResponse = await client.PostAsJsonAsync("/api/v1/injuries", injuryDto);
        createResponse.EnsureSuccessStatusCode();
        
        var createContent = await createResponse.Content.ReadAsStringAsync();
        var createJson = System.Text.Json.JsonDocument.Parse(createContent);
        var injuryId = Guid.Parse(createJson.RootElement.GetProperty("injury").GetProperty("id").GetString()!);

        // Act
        var response = await client.PutAsync($"/api/v1/injuries/{injuryId}/resolve", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var resolvedInjury = await response.Content.ReadFromJsonAsync<InjuryLimitationDto>();
        resolvedInjury.Should().NotBeNull();
        resolvedInjury!.Status.Should().Be(InjuryStatus.Resolved);
    }

    [Fact]
    public async Task MarkInjuryResolved_WithNonExistentInjury_ShouldReturnNotFound()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        await CreateUserProfile(client);
        var fakeInjuryId = Guid.NewGuid();

        // Act
        var response = await client.PutAsync($"/api/v1/injuries/{fakeInjuryId}/resolve", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region GET /api/v1/injuries/active Tests

    [Fact]
    public async Task GetActiveInjuries_WithMultipleInjuries_ShouldReturnActiveAndImprovingOnly()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        await CreateUserProfile(client);

        // Create multiple injuries
        await client.PostAsJsonAsync("/api/v1/injuries", new ReportInjuryDto
        {
            BodyPart = "Shoulder",
            InjuryType = InjuryType.Acute
        });

        var knee = await client.PostAsJsonAsync("/api/v1/injuries", new ReportInjuryDto
        {
            BodyPart = "Knee",
            InjuryType = InjuryType.Chronic
        });
        knee.EnsureSuccessStatusCode();
        
        var kneeContent = await knee.Content.ReadAsStringAsync();
        var kneeJson = System.Text.Json.JsonDocument.Parse(kneeContent);
        var kneeInjuryId = Guid.Parse(kneeJson.RootElement.GetProperty("injury").GetProperty("id").GetString()!);

        // Mark knee as resolved
        await client.PutAsync($"/api/v1/injuries/{kneeInjuryId}/resolve", null);

        // Act
        var response = await client.GetAsync("/api/v1/injuries/active");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var activeInjuries = await response.Content.ReadFromJsonAsync<List<InjuryLimitationDto>>();
        activeInjuries.Should().NotBeNull();
        activeInjuries!.Should().HaveCount(1); // Only shoulder should be active
        activeInjuries.Should().Contain(i => i.BodyPart == "Shoulder");
        activeInjuries.Should().NotContain(i => i.BodyPart == "Knee");
    }

    [Fact]
    public async Task GetActiveInjuries_WithNoInjuries_ShouldReturnEmptyList()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        await CreateUserProfile(client);

        // Act
        var response = await client.GetAsync("/api/v1/injuries/active");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var activeInjuries = await response.Content.ReadFromJsonAsync<List<InjuryLimitationDto>>();
        activeInjuries.Should().NotBeNull();
        activeInjuries!.Should().BeEmpty();
    }

    #endregion

    #region GET /api/v1/injuries/contraindications Tests

    [Fact]
    public async Task GetContraindications_WithActiveInjury_ShouldReturnDisclaimer()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        await CreateUserProfile(client);

        await client.PostAsJsonAsync("/api/v1/injuries", new ReportInjuryDto
        {
            BodyPart = "Shoulder",
            InjuryType = InjuryType.Acute,
            MovementRestrictions = "Overhead"
        });

        // Act
        var response = await client.GetAsync("/api/v1/injuries/contraindications");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("disclaimer");
        content.Should().Contain("Medical Disclaimer");
    }

    [Fact]
    public async Task GetContraindications_WithNoInjuries_ShouldReturnClearedMessage()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        await CreateUserProfile(client);

        // Act
        var response = await client.GetAsync("/api/v1/injuries/contraindications");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("cleared for all exercises");
    }

    #endregion

    #region GET /api/v1/injuries/substitutes/{exerciseId} Tests

    [Fact]
    public async Task GetSubstituteExercise_ShouldIncludeDisclaimer()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        await CreateUserProfile(client);

        // Use a random exercise ID for testing
        var exerciseId = Guid.NewGuid();

        // Act
        var response = await client.GetAsync($"/api/v1/injuries/substitutes/{exerciseId}");

        // Assert
        // Should return OK even if exercise not found (returns null substitute with message)
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("disclaimer");
    }

    #endregion
}
