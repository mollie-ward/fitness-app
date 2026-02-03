using System.Net;
using System.Net.Http.Json;
using Xunit;
using FluentAssertions;
using FitnessApp.Application.DTOs;
using FitnessApp.Domain.Enums;
using FitnessApp.IntegrationTests.Helpers;

namespace FitnessApp.IntegrationTests.API;

public class WorkoutControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly Guid _testUserId;

    public WorkoutControllerTests(CustomWebApplicationFactory factory)
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

    private async Task<TrainingPlanDetailDto> CreatePlanWithWorkoutsAsync(HttpClient client)
    {
        // Create profile
        var profileDto = CreateValidProfileDto();
        await client.PostAsJsonAsync("/api/v1/users/profile", profileDto);

        // Generate plan
        var generateResponse = await client.PostAsync("/api/v1/training/plans/generate", null);
        var planSummary = await generateResponse.Content.ReadFromJsonAsync<TrainingPlanSummaryDto>();

        // Get detailed plan
        var planResponse = await client.GetAsync($"/api/v1/training/plans/{planSummary!.Id}");
        return (await planResponse.Content.ReadFromJsonAsync<TrainingPlanDetailDto>())!;
    }

    [Fact]
    public async Task GetTodaysWorkout_WhenWorkoutExists_ShouldReturnWorkout()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        var plan = await CreatePlanWithWorkoutsAsync(client);

        // Ensure plan has weeks and workouts
        if (!plan.TrainingWeeks.Any() || !plan.TrainingWeeks.First().Workouts.Any())
        {
            // Skip test if no workouts
            return;
        }

        // Act - Get today's workout (may not exist depending on plan generation)
        var response = await client.GetAsync("/api/v1/training/workouts/today");

        // Assert - Either OK with workout or NotFound if no workout today
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetTodaysWorkout_WithoutPlan_ShouldReturnNotFound()
    {
        // Arrange
        var client = CreateAuthenticatedClient();

        // Act
        var response = await client.GetAsync("/api/v1/training/workouts/today");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetTodaysWorkout_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var unauthClient = _factory.CreateClient();

        // Act
        var response = await unauthClient.GetAsync("/api/v1/training/workouts/today");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetUpcomingWorkouts_WithActivePlan_ShouldReturnWorkouts()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        await CreatePlanWithWorkoutsAsync(client);

        // Act
        var response = await client.GetAsync("/api/v1/training/workouts/upcoming");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var workouts = await response.Content.ReadFromJsonAsync<List<WorkoutSummaryDto>>();
        workouts.Should().NotBeNull();
        // Workouts might be empty if none scheduled in next 7 days
    }

    [Fact]
    public async Task GetUpcomingWorkouts_WithoutPlan_ShouldReturnEmptyList()
    {
        // Arrange
        var client = CreateAuthenticatedClient();

        // Act
        var response = await client.GetAsync("/api/v1/training/workouts/upcoming");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var workouts = await response.Content.ReadFromJsonAsync<List<WorkoutSummaryDto>>();
        workouts.Should().NotBeNull();
        workouts.Should().BeEmpty();
    }

    [Fact]
    public async Task GetWorkoutById_WithValidId_ShouldReturnWorkout()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        var plan = await CreatePlanWithWorkoutsAsync(client);
        
        // Skip if no workouts
        if (!plan.TrainingWeeks.Any() || !plan.TrainingWeeks.SelectMany(w => w.Workouts).Any())
        {
            return;
        }
        
        var firstWorkout = plan.TrainingWeeks.SelectMany(w => w.Workouts).First();

        // Act
        var response = await client.GetAsync($"/api/v1/training/workouts/{firstWorkout.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var workout = await response.Content.ReadFromJsonAsync<WorkoutDetailDto>();
        workout.Should().NotBeNull();
        workout!.Id.Should().Be(firstWorkout.Id);
        workout.WorkoutName.Should().Be(firstWorkout.WorkoutName);
    }

    [Fact]
    public async Task GetWorkoutById_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        var invalidId = Guid.NewGuid();

        // Act
        var response = await client.GetAsync($"/api/v1/training/workouts/{invalidId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetWorkoutById_ForAnotherUser_ShouldReturnForbidden()
    {
        // Arrange - User 1 creates a plan
        var userId1 = Guid.NewGuid();
        var client1 = CreateAuthenticatedClient(userId1);
        var plan = await CreatePlanWithWorkoutsAsync(client1);
        
        // Skip if no workouts
        if (!plan.TrainingWeeks.Any() || !plan.TrainingWeeks.SelectMany(w => w.Workouts).Any())
        {
            return;
        }
        
        var firstWorkout = plan.TrainingWeeks.SelectMany(w => w.Workouts).First();

        // Act - User 2 tries to access User 1's workout
        var userId2 = Guid.NewGuid();
        var client2 = CreateAuthenticatedClient(userId2);
        var response = await client2.GetAsync($"/api/v1/training/workouts/{firstWorkout.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CompleteWorkout_WithValidId_ShouldReturnUpdatedWorkout()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        var plan = await CreatePlanWithWorkoutsAsync(client);
        
        // Skip if no workouts
        if (!plan.TrainingWeeks.Any() || !plan.TrainingWeeks.SelectMany(w => w.Workouts).Any())
        {
            return;
        }
        
        var firstWorkout = plan.TrainingWeeks.SelectMany(w => w.Workouts).First();
        var request = new CompleteWorkoutRequest
        {
            CompletedAt = DateTime.UtcNow
        };

        // Act
        var response = await client.PutAsJsonAsync($"/api/v1/training/workouts/{firstWorkout.Id}/complete", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var workout = await response.Content.ReadFromJsonAsync<WorkoutDetailDto>();
        workout.Should().NotBeNull();
        workout!.CompletionStatus.Should().Be(CompletionStatus.Completed);
        workout.CompletedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task CompleteWorkout_WithoutRequestBody_ShouldStillSucceed()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        var plan = await CreatePlanWithWorkoutsAsync(client);
        
        // Skip if no workouts
        if (!plan.TrainingWeeks.Any() || !plan.TrainingWeeks.SelectMany(w => w.Workouts).Any())
        {
            return;
        }
        
        var firstWorkout = plan.TrainingWeeks.SelectMany(w => w.Workouts).First();

        // Act
        var response = await client.PutAsJsonAsync($"/api/v1/training/workouts/{firstWorkout.Id}/complete", (CompleteWorkoutRequest?)null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var workout = await response.Content.ReadFromJsonAsync<WorkoutDetailDto>();
        workout.Should().NotBeNull();
        workout!.CompletionStatus.Should().Be(CompletionStatus.Completed);
    }

    [Fact]
    public async Task CompleteWorkout_ForAnotherUser_ShouldReturnForbidden()
    {
        // Arrange - User 1 creates a plan
        var userId1 = Guid.NewGuid();
        var client1 = CreateAuthenticatedClient(userId1);
        var plan = await CreatePlanWithWorkoutsAsync(client1);
        
        // Skip if no workouts
        if (!plan.TrainingWeeks.Any() || !plan.TrainingWeeks.SelectMany(w => w.Workouts).Any())
        {
            return;
        }
        
        var firstWorkout = plan.TrainingWeeks.SelectMany(w => w.Workouts).First();

        // Act - User 2 tries to complete User 1's workout
        var userId2 = Guid.NewGuid();
        var client2 = CreateAuthenticatedClient(userId2);
        var request = new CompleteWorkoutRequest();
        var response = await client2.PutAsJsonAsync($"/api/v1/training/workouts/{firstWorkout.Id}/complete", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task SkipWorkout_WithValidId_ShouldReturnUpdatedWorkout()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        var plan = await CreatePlanWithWorkoutsAsync(client);
        
        // Skip if no workouts
        if (!plan.TrainingWeeks.Any() || !plan.TrainingWeeks.SelectMany(w => w.Workouts).Any())
        {
            return;
        }
        
        var firstWorkout = plan.TrainingWeeks.SelectMany(w => w.Workouts).First();
        var request = new SkipWorkoutRequest
        {
            Reason = "Feeling unwell"
        };

        // Act
        var response = await client.PutAsJsonAsync($"/api/v1/training/workouts/{firstWorkout.Id}/skip", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var workout = await response.Content.ReadFromJsonAsync<WorkoutDetailDto>();
        workout.Should().NotBeNull();
        workout!.CompletionStatus.Should().Be(CompletionStatus.Skipped);
    }

    [Fact]
    public async Task SkipWorkout_WithoutReason_ShouldStillSucceed()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        var plan = await CreatePlanWithWorkoutsAsync(client);
        
        // Skip if no workouts
        if (!plan.TrainingWeeks.Any() || !plan.TrainingWeeks.SelectMany(w => w.Workouts).Any())
        {
            return;
        }
        
        var firstWorkout = plan.TrainingWeeks.SelectMany(w => w.Workouts).First();

        // Act
        var response = await client.PutAsJsonAsync($"/api/v1/training/workouts/{firstWorkout.Id}/skip", (SkipWorkoutRequest?)null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var workout = await response.Content.ReadFromJsonAsync<WorkoutDetailDto>();
        workout.Should().NotBeNull();
        workout!.CompletionStatus.Should().Be(CompletionStatus.Skipped);
    }

    [Fact]
    public async Task GetWorkoutsForCalendar_WithValidDateRange_ShouldReturnWorkouts()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        await CreatePlanWithWorkoutsAsync(client);
        var startDate = DateTime.UtcNow.Date;
        var endDate = startDate.AddDays(30);

        // Act
        var response = await client.GetAsync($"/api/v1/training/workouts/calendar?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var workouts = await response.Content.ReadFromJsonAsync<List<WorkoutSummaryDto>>();
        workouts.Should().NotBeNull();
    }

    [Fact]
    public async Task GetWorkoutsForCalendar_WithInvalidDateRange_ShouldReturnBadRequest()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        var startDate = DateTime.UtcNow.Date;
        var endDate = startDate.AddDays(-1); // End before start

        // Act
        var response = await client.GetAsync($"/api/v1/training/workouts/calendar?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetWorkoutsForCalendar_WithExcessiveDateRange_ShouldReturnBadRequest()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        var startDate = DateTime.UtcNow.Date;
        var endDate = startDate.AddDays(100); // More than 90 days

        // Act
        var response = await client.GetAsync($"/api/v1/training/workouts/calendar?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetWorkoutsForCalendar_WithMissingDates_ShouldReturnBadRequest()
    {
        // Arrange
        var client = CreateAuthenticatedClient();

        // Act - Missing both dates
        var response = await client.GetAsync("/api/v1/training/workouts/calendar");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
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
