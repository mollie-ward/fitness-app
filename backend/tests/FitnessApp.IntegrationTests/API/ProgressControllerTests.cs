using System.Net;
using System.Net.Http.Json;
using Xunit;
using FluentAssertions;
using FitnessApp.Application.DTOs;
using FitnessApp.Domain.Enums;
using FitnessApp.IntegrationTests.Helpers;

namespace FitnessApp.IntegrationTests.API;

public class ProgressControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly Guid _testUserId;

    public ProgressControllerTests(CustomWebApplicationFactory factory)
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

    private async Task<(Guid workoutId, Guid planId)> CreateTestWorkout(HttpClient client)
    {
        // Create a user profile
        var profileDto = CreateValidProfileDto();
        var profileResponse = await client.PostAsJsonAsync("/api/v1/users/profile", profileDto);
        profileResponse.EnsureSuccessStatusCode();

        // Generate a training plan
        var planResponse = await client.PostAsync("/api/v1/training/plans/generate", null);
        
        if (!planResponse.IsSuccessStatusCode)
        {
            var error = await planResponse.Content.ReadAsStringAsync();
            throw new Exception($"Failed to generate plan: {planResponse.StatusCode} - {error}");
        }

        var plan = await planResponse.Content.ReadFromJsonAsync<TrainingPlanSummaryDto>();
        if (plan == null || plan.Id == Guid.Empty)
        {
            throw new Exception("Plan was not created successfully");
        }

        // Get today's workout or upcoming workouts
        var workoutsResponse = await client.GetAsync("/api/v1/training/workouts/upcoming");
        workoutsResponse.EnsureSuccessStatusCode();
        
        var workouts = await workoutsResponse.Content.ReadFromJsonAsync<List<WorkoutSummaryDto>>();

        if (workouts == null || !workouts.Any())
        {
            throw new Exception("No workouts were created");
        }

        var workout = workouts.First();
        if (workout == null || workout.Id == Guid.Empty)
        {
            throw new Exception("Workout ID is invalid");
        }

        return (workout.Id, plan.Id);
    }

    [Fact]
    public async Task CompleteWorkout_WithValidRequest_ShouldReturn200()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        var (workoutId, _) = await CreateTestWorkout(client);
        
        var completionDto = new WorkoutCompletionDto
        {
            CompletedAt = DateTime.UtcNow,
            Duration = 45,
            Notes = "Great workout!"
        };

        // Act
        var response = await client.PutAsJsonAsync($"/api/v1/progress/workouts/{workoutId}/complete", completionDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<WorkoutDetailDto>();
        result.Should().NotBeNull();
        result!.CompletionStatus.Should().Be(CompletionStatus.Completed);
    }

    [Fact]
    public async Task CompleteWorkout_WithFutureDate_ShouldReturn400()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        var (workoutId, _) = await CreateTestWorkout(client);
        
        var completionDto = new WorkoutCompletionDto
        {
            CompletedAt = DateTime.UtcNow.AddDays(1), // Future date
            Duration = 45
        };

        // Act
        var response = await client.PutAsJsonAsync($"/api/v1/progress/workouts/{workoutId}/complete", completionDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CompleteWorkout_NonExistentWorkout_ShouldReturn404()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        var nonExistentWorkoutId = Guid.NewGuid();
        
        var completionDto = new WorkoutCompletionDto
        {
            CompletedAt = DateTime.UtcNow,
            Duration = 45
        };

        // Act
        var response = await client.PutAsJsonAsync($"/api/v1/progress/workouts/{nonExistentWorkoutId}/complete", completionDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CompleteWorkout_AlreadyCompleted_ShouldReturn400()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        var (workoutId, _) = await CreateTestWorkout(client);
        
        var completionDto = new WorkoutCompletionDto
        {
            CompletedAt = DateTime.UtcNow,
            Duration = 45
        };

        // Complete it once
        await client.PutAsJsonAsync($"/api/v1/progress/workouts/{workoutId}/complete", completionDto);

        // Act - Try to complete again
        var response = await client.PutAsJsonAsync($"/api/v1/progress/workouts/{workoutId}/complete", completionDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CompleteWorkout_WithoutAuthentication_ShouldReturn401()
    {
        // Arrange
        var unauthClient = _factory.CreateClient();
        var completionDto = new WorkoutCompletionDto
        {
            CompletedAt = DateTime.UtcNow,
            Duration = 45
        };

        // Act
        var response = await unauthClient.PutAsJsonAsync($"/api/v1/progress/workouts/{Guid.NewGuid()}/complete", completionDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UndoCompletion_WithCompletedWorkout_ShouldReturn200()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        var (workoutId, _) = await CreateTestWorkout(client);
        
        // Complete the workout first
        var completionDto = new WorkoutCompletionDto
        {
            CompletedAt = DateTime.UtcNow,
            Duration = 45
        };
        await client.PutAsJsonAsync($"/api/v1/progress/workouts/{workoutId}/complete", completionDto);

        // Act
        var response = await client.PutAsync($"/api/v1/progress/workouts/{workoutId}/undo", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<WorkoutDetailDto>();
        result.Should().NotBeNull();
        result!.CompletionStatus.Should().Be(CompletionStatus.NotStarted);
    }

    [Fact]
    public async Task UndoCompletion_NotCompleted_ShouldReturn400()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        var (workoutId, _) = await CreateTestWorkout(client);

        // Act - Try to undo without completing first
        var response = await client.PutAsync($"/api/v1/progress/workouts/{workoutId}/undo", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetWeeklyStats_WithCompletions_ShouldReturnStats()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        var (workoutId, _) = await CreateTestWorkout(client);
        
        // Complete a workout
        var completionDto = new WorkoutCompletionDto
        {
            CompletedAt = DateTime.UtcNow,
            Duration = 45
        };
        await client.PutAsJsonAsync($"/api/v1/progress/workouts/{workoutId}/complete", completionDto);

        // Act
        var response = await client.GetAsync("/api/v1/progress/stats/weekly");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var stats = await response.Content.ReadFromJsonAsync<CompletionStatsDto>();
        stats.Should().NotBeNull();
        stats!.CompletedCount.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetMonthlyStats_ShouldReturnStats()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        await CreateTestWorkout(client);

        // Act
        var response = await client.GetAsync("/api/v1/progress/stats/monthly");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var stats = await response.Content.ReadFromJsonAsync<CompletionStatsDto>();
        stats.Should().NotBeNull();
        stats!.PeriodStart.Should().BeCloseTo(new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1), TimeSpan.FromDays(1));
    }

    [Fact]
    public async Task GetOverallStats_ShouldReturnStats()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        await CreateTestWorkout(client);

        // Act
        var response = await client.GetAsync("/api/v1/progress/stats/overall");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var stats = await response.Content.ReadFromJsonAsync<OverallStatsDto>();
        stats.Should().NotBeNull();
    }

    [Fact]
    public async Task GetStreak_WithNoCompletions_ShouldReturnEmptyStreak()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        await CreateTestWorkout(client);

        // Act
        var response = await client.GetAsync("/api/v1/progress/streak");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var streak = await response.Content.ReadFromJsonAsync<StreakInfoDto>();
        streak.Should().NotBeNull();
        streak!.CurrentStreak.Should().Be(0);
    }

    [Fact]
    public async Task GetStreak_WithCompletions_ShouldReturnStreak()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        var (workoutId, _) = await CreateTestWorkout(client);
        
        // Complete a workout
        var completionDto = new WorkoutCompletionDto
        {
            CompletedAt = DateTime.UtcNow,
            Duration = 45
        };
        await client.PutAsJsonAsync($"/api/v1/progress/workouts/{workoutId}/complete", completionDto);

        // Act
        var response = await client.GetAsync("/api/v1/progress/streak");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var streak = await response.Content.ReadFromJsonAsync<StreakInfoDto>();
        streak.Should().NotBeNull();
        streak!.CurrentStreak.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task GetHistory_WithDefaultRange_ShouldReturnHistory()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        await CreateTestWorkout(client);

        // Act
        var response = await client.GetAsync("/api/v1/progress/history");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var history = await response.Content.ReadFromJsonAsync<List<CompletionHistoryDto>>();
        history.Should().NotBeNull();
    }

    [Fact]
    public async Task GetHistory_WithCustomRange_ShouldReturnFilteredHistory()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        var (workoutId, _) = await CreateTestWorkout(client);
        
        // Complete a workout
        var completionDto = new WorkoutCompletionDto
        {
            CompletedAt = DateTime.UtcNow,
            Duration = 45
        };
        await client.PutAsJsonAsync($"/api/v1/progress/workouts/{workoutId}/complete", completionDto);

        var startDate = DateTime.UtcNow.AddDays(-7);
        var endDate = DateTime.UtcNow;

        // Act
        var response = await client.GetAsync($"/api/v1/progress/history?startDate={startDate:O}&endDate={endDate:O}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var history = await response.Content.ReadFromJsonAsync<List<CompletionHistoryDto>>();
        history.Should().NotBeNull();
    }

    [Fact]
    public async Task GetHistory_WithInvalidRange_ShouldReturn400()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        var startDate = DateTime.UtcNow;
        var endDate = DateTime.UtcNow.AddDays(-7); // End before start

        // Act
        var response = await client.GetAsync($"/api/v1/progress/history?startDate={startDate:O}&endDate={endDate:O}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetHistory_WithRangeTooLarge_ShouldReturn400()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        var startDate = DateTime.UtcNow.AddDays(-400); // More than 365 days
        var endDate = DateTime.UtcNow;

        // Act
        var response = await client.GetAsync($"/api/v1/progress/history?startDate={startDate:O}&endDate={endDate:O}");

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
            },
            InjuryLimitations = new List<InjuryLimitationDto>()
        };
    }
}
