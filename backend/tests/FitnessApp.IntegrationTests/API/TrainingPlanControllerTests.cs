using System.Net;
using System.Net.Http.Json;
using Xunit;
using FluentAssertions;
using FitnessApp.Application.DTOs;
using FitnessApp.Domain.Enums;
using FitnessApp.IntegrationTests.Helpers;

namespace FitnessApp.IntegrationTests.API;

public class TrainingPlanControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly Guid _testUserId;

    public TrainingPlanControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _testUserId = Guid.NewGuid();
    }

    private HttpClient CreateAuthenticatedClient(Guid? userId = null)
    {
        var client = _factory.CreateClient();
        var effectiveUserId = userId ?? _testUserId;
        // Add test user ID header for TestAuthHandler
        client.DefaultRequestHeaders.Add("X-Test-UserId", effectiveUserId.ToString());
        return client;
    }

    [Fact]
    public async Task GeneratePlan_WithValidProfile_ShouldReturnCreated()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        
        // Create a user profile first
        var profileDto = CreateValidProfileDto();
        await client.PostAsJsonAsync("/api/v1/users/profile", profileDto);

        // Act
        var response = await client.PostAsync("/api/v1/training/plans/generate", null);

        // Assert
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Request failed with {response.StatusCode}: {error}");
        }
        
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var plan = await response.Content.ReadFromJsonAsync<TrainingPlanSummaryDto>();
        plan.Should().NotBeNull();
        plan!.Id.Should().NotBe(Guid.Empty);
        plan.TotalWeeks.Should().BeGreaterThan(0);
        plan.Status.Should().Be(PlanStatus.Active);
    }

    [Fact]
    public async Task GeneratePlan_WithoutProfile_ShouldReturnNotFound()
    {
        // Arrange
        var client = CreateAuthenticatedClient();

        // Act - Try to generate plan without profile
        var response = await client.PostAsync("/api/v1/training/plans/generate", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GeneratePlan_WhenActivePlanExists_ShouldReturnConflict()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        var profileDto = CreateValidProfileDto();
        await client.PostAsJsonAsync("/api/v1/users/profile", profileDto);
        await client.PostAsync("/api/v1/training/plans/generate", null);

        // Act - Try to generate another plan
        var response = await client.PostAsync("/api/v1/training/plans/generate", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task GeneratePlan_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var unauthClient = _factory.CreateClient();

        // Act
        var response = await unauthClient.PostAsync("/api/v1/training/plans/generate", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetCurrentPlan_WhenPlanExists_ShouldReturnPlan()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        var profileDto = CreateValidProfileDto();
        await client.PostAsJsonAsync("/api/v1/users/profile", profileDto);
        await client.PostAsync("/api/v1/training/plans/generate", null);

        // Act
        var response = await client.GetAsync("/api/v1/training/plans/current");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var plan = await response.Content.ReadFromJsonAsync<TrainingPlanSummaryDto>();
        plan.Should().NotBeNull();
        plan!.Status.Should().Be(PlanStatus.Active);
    }

    [Fact]
    public async Task GetCurrentPlan_WhenNoPlanExists_ShouldReturnNotFound()
    {
        // Arrange
        var client = CreateAuthenticatedClient();

        // Act
        var response = await client.GetAsync("/api/v1/training/plans/current");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetPlanById_WithValidId_ShouldReturnDetailedPlan()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        var profileDto = CreateValidProfileDto();
        await client.PostAsJsonAsync("/api/v1/users/profile", profileDto);
        var generateResponse = await client.PostAsync("/api/v1/training/plans/generate", null);
        var createdPlan = await generateResponse.Content.ReadFromJsonAsync<TrainingPlanSummaryDto>();

        // Act
        var response = await client.GetAsync($"/api/v1/training/plans/{createdPlan!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var plan = await response.Content.ReadFromJsonAsync<TrainingPlanDetailDto>();
        plan.Should().NotBeNull();
        plan!.Id.Should().Be(createdPlan.Id);
        plan.TrainingWeeks.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetPlanById_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        var invalidId = Guid.NewGuid();

        // Act
        var response = await client.GetAsync($"/api/v1/training/plans/{invalidId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetPlanById_ForAnotherUser_ShouldReturnForbidden()
    {
        // Arrange - User 1 creates a plan
        var userId1 = Guid.NewGuid();
        var client1 = CreateAuthenticatedClient(userId1);
        var profileDto = CreateValidProfileDto();
        await client1.PostAsJsonAsync("/api/v1/users/profile", profileDto);
        var generateResponse = await client1.PostAsync("/api/v1/training/plans/generate", null);
        var createdPlan = await generateResponse.Content.ReadFromJsonAsync<TrainingPlanSummaryDto>();

        // Act - User 2 tries to access User 1's plan
        var userId2 = Guid.NewGuid();
        var client2 = CreateAuthenticatedClient(userId2);
        var response = await client2.GetAsync($"/api/v1/training/plans/{createdPlan!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetPlanWeek_WithValidWeekNumber_ShouldReturnWeek()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        var profileDto = CreateValidProfileDto();
        await client.PostAsJsonAsync("/api/v1/users/profile", profileDto);
        var generateResponse = await client.PostAsync("/api/v1/training/plans/generate", null);
        var createdPlan = await generateResponse.Content.ReadFromJsonAsync<TrainingPlanSummaryDto>();

        // Act
        var response = await client.GetAsync($"/api/v1/training/plans/{createdPlan!.Id}/weeks/1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var week = await response.Content.ReadFromJsonAsync<TrainingWeekDto>();
        week.Should().NotBeNull();
        week!.WeekNumber.Should().Be(1);
        week.Workouts.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetPlanWeek_WithInvalidWeekNumber_ShouldReturnNotFound()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        var profileDto = CreateValidProfileDto();
        await client.PostAsJsonAsync("/api/v1/users/profile", profileDto);
        var generateResponse = await client.PostAsync("/api/v1/training/plans/generate", null);
        var createdPlan = await generateResponse.Content.ReadFromJsonAsync<TrainingPlanSummaryDto>();

        // Act - Request week 999
        var response = await client.GetAsync($"/api/v1/training/plans/{createdPlan!.Id}/weeks/999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeletePlan_WithValidId_ShouldReturnNoContent()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        var profileDto = CreateValidProfileDto();
        await client.PostAsJsonAsync("/api/v1/users/profile", profileDto);
        var generateResponse = await client.PostAsync("/api/v1/training/plans/generate", null);
        var createdPlan = await generateResponse.Content.ReadFromJsonAsync<TrainingPlanSummaryDto>();

        // Act
        var response = await client.DeleteAsync($"/api/v1/training/plans/{createdPlan!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify plan is deleted
        var getResponse = await client.GetAsync("/api/v1/training/plans/current");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeletePlan_ForAnotherUser_ShouldReturnForbidden()
    {
        // Arrange - User 1 creates a plan
        var userId1 = Guid.NewGuid();
        var client1 = CreateAuthenticatedClient(userId1);
        var profileDto = CreateValidProfileDto();
        await client1.PostAsJsonAsync("/api/v1/users/profile", profileDto);
        var generateResponse = await client1.PostAsync("/api/v1/training/plans/generate", null);
        var createdPlan = await generateResponse.Content.ReadFromJsonAsync<TrainingPlanSummaryDto>();

        // Act - User 2 tries to delete User 1's plan
        var userId2 = Guid.NewGuid();
        var client2 = CreateAuthenticatedClient(userId2);
        var response = await client2.DeleteAsync($"/api/v1/training/plans/{createdPlan!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    private static UserProfileDto CreateValidProfileDto()
    {
        return new UserProfileDto
        {
            Name = "Test User",
            Email = "test@example.com",
            HyroxLevel = FitnessLevel.Intermediate,
            RunningLevel = FitnessLevel.Beginner,
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
                PreviousTrainingDetails = "Ran a few 5Ks",
                EquipmentFamiliarity = "Basic gym equipment"
            },
            TrainingGoals = new List<TrainingGoalDto>
            {
                new TrainingGoalDto
                {
                    GoalType = GoalType.HyroxRace,
                    Description = "Complete HYROX Race",
                    TargetDate = DateTime.UtcNow.AddMonths(3),
                    Priority = 1,
                    Status = GoalStatus.Active
                }
            }
        };
    }
}
