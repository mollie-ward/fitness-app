using System.Net;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using Xunit;
using FluentAssertions;
using FitnessApp.Application.DTOs;
using FitnessApp.Domain.Enums;
using FitnessApp.IntegrationTests.Helpers;

namespace FitnessApp.IntegrationTests.API;

public class UserProfileControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly Guid _testUserId;

    public UserProfileControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _testUserId = Guid.NewGuid();
    }

    private HttpClient CreateAuthenticatedClient(Guid? userId = null)
    {
        var client = _factory.CreateClient();
        var authToken = JwtTokenHelper.GenerateToken(userId ?? _testUserId);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
        return client;
    }

    [Fact]
    public async Task CreateProfile_WithValidData_ShouldReturnCreated()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        var profileDto = CreateValidProfileDto();

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/users/profile", profileDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdProfile = await response.Content.ReadFromJsonAsync<UserProfileDto>();
        createdProfile.Should().NotBeNull();
        createdProfile!.Id.Should().NotBe(Guid.Empty);
        createdProfile.Name.Should().Be(profileDto.Name);
        createdProfile.Email.Should().Be(profileDto.Email);
    }

    [Fact]
    public async Task CreateProfile_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var profileDto = CreateValidProfileDto();
        var unauthClient = _factory.CreateClient();

        // Act
        var response = await unauthClient.PostAsJsonAsync("/api/v1/users/profile", profileDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateProfile_Duplicate_ShouldReturnConflict()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        var profileDto = CreateValidProfileDto();
        await client.PostAsJsonAsync("/api/v1/users/profile", profileDto);

        // Act - Try to create again
        var response = await client.PostAsJsonAsync("/api/v1/users/profile", profileDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task CreateProfile_WithInvalidEmail_ShouldReturnBadRequest()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        var profileDto = CreateValidProfileDto();
        profileDto.Email = "invalid-email";

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/users/profile", profileDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateProfile_WithInvalidSchedule_ShouldReturnBadRequest()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        var profileDto = CreateValidProfileDto();
        profileDto.ScheduleAvailability!.MinimumSessionsPerWeek = 5;
        profileDto.ScheduleAvailability!.MaximumSessionsPerWeek = 2; // Max < Min

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/users/profile", profileDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetProfile_WhenProfileExists_ShouldReturnProfile()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        var profileDto = CreateValidProfileDto();
        await client.PostAsJsonAsync("/api/v1/users/profile", profileDto);

        // Act
        var response = await client.GetAsync("/api/v1/users/profile");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var profile = await response.Content.ReadFromJsonAsync<UserProfileDto>();
        profile.Should().NotBeNull();
        profile!.Name.Should().Be(profileDto.Name);
    }

    [Fact]
    public async Task GetProfile_WhenProfileDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange - New user ID that hasn't created a profile
        var newUserId = Guid.NewGuid();
        var client = CreateAuthenticatedClient(newUserId);

        // Act
        var response = await client.GetAsync("/api/v1/users/profile");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateProfile_WithValidData_ShouldReturnOk()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        var profileDto = CreateValidProfileDto();
        await client.PostAsJsonAsync("/api/v1/users/profile", profileDto);
        
        profileDto.Name = "Updated Name";
        profileDto.HyroxLevel = FitnessLevel.Advanced;

        // Act
        var response = await client.PutAsJsonAsync("/api/v1/users/profile", profileDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedProfile = await response.Content.ReadFromJsonAsync<UserProfileDto>();
        updatedProfile.Should().NotBeNull();
        updatedProfile!.Name.Should().Be("Updated Name");
        updatedProfile.HyroxLevel.Should().Be(FitnessLevel.Advanced);
    }

    [Fact]
    public async Task UpdateProfile_WhenProfileDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        var profileDto = CreateValidProfileDto();

        // Act
        var response = await client.PutAsJsonAsync("/api/v1/users/profile", profileDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetGoals_WhenProfileExists_ShouldReturnGoals()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        var profileDto = CreateValidProfileDto();
        profileDto.TrainingGoals = new List<TrainingGoalDto>
        {
            new TrainingGoalDto
            {
                GoalType = GoalType.HyroxRace,
                Description = "Complete HYROX",
                TargetDate = DateTime.UtcNow.AddMonths(3),
                Priority = 1
            }
        };
        await client.PostAsJsonAsync("/api/v1/users/profile", profileDto);

        // Act
        var response = await client.GetAsync("/api/v1/users/profile/goals");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var goals = await response.Content.ReadFromJsonAsync<List<TrainingGoalDto>>();
        goals.Should().NotBeNull();
        goals!.Should().HaveCount(1);
        goals[0].Description.Should().Be("Complete HYROX");
    }

    [Fact]
    public async Task CreateGoal_WithValidData_ShouldReturnCreated()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        var profileDto = CreateValidProfileDto();
        await client.PostAsJsonAsync("/api/v1/users/profile", profileDto);

        var goalDto = new TrainingGoalDto
        {
            GoalType = GoalType.RunningDistance,
            Description = "Run a 5K",
            TargetDate = DateTime.UtcNow.AddMonths(2),
            Priority = 1
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/users/profile/goals", goalDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdGoal = await response.Content.ReadFromJsonAsync<TrainingGoalDto>();
        createdGoal.Should().NotBeNull();
        createdGoal!.Id.Should().NotBe(Guid.Empty);
        createdGoal.Description.Should().Be("Run a 5K");
    }

    [Fact]
    public async Task CreateGoal_WithPastTargetDate_ShouldReturnBadRequest()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        var profileDto = CreateValidProfileDto();
        await client.PostAsJsonAsync("/api/v1/users/profile", profileDto);

        var goalDto = new TrainingGoalDto
        {
            GoalType = GoalType.RunningDistance,
            Description = "Run a 5K",
            TargetDate = DateTime.UtcNow.AddDays(-1), // Past date
            Priority = 1
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/users/profile/goals", goalDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateGoal_WithoutProfile_ShouldReturnNotFound()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        var goalDto = new TrainingGoalDto
        {
            GoalType = GoalType.GeneralFitness,
            Description = "Improve fitness",
            Priority = 1
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/users/profile/goals", goalDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateGoal_WithValidData_ShouldReturnOk()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        var profileDto = CreateValidProfileDto();
        var createResponse = await client.PostAsJsonAsync("/api/v1/users/profile", profileDto);
        var profile = await createResponse.Content.ReadFromJsonAsync<UserProfileDto>();

        var goalDto = new TrainingGoalDto
        {
            GoalType = GoalType.HyroxRace,
            Description = "Complete HYROX",
            Priority = 1
        };
        var goalResponse = await client.PostAsJsonAsync("/api/v1/users/profile/goals", goalDto);
        var createdGoal = await goalResponse.Content.ReadFromJsonAsync<TrainingGoalDto>();

        createdGoal!.Description = "Complete HYROX Elite";
        createdGoal.Status = GoalStatus.Completed;

        // Act
        var response = await client.PutAsJsonAsync($"/api/v1/users/profile/goals/{createdGoal.Id}", createdGoal);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedGoal = await response.Content.ReadFromJsonAsync<TrainingGoalDto>();
        updatedGoal.Should().NotBeNull();
        updatedGoal!.Description.Should().Be("Complete HYROX Elite");
        updatedGoal.Status.Should().Be(GoalStatus.Completed);
    }

    [Fact]
    public async Task UpdateGoal_NonExistent_ShouldReturnNotFound()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        var profileDto = CreateValidProfileDto();
        await client.PostAsJsonAsync("/api/v1/users/profile", profileDto);

        var goalDto = new TrainingGoalDto
        {
            GoalType = GoalType.HyroxRace,
            Description = "Complete HYROX",
            Priority = 1
        };

        // Act
        var response = await client.PutAsJsonAsync($"/api/v1/users/profile/goals/{Guid.NewGuid()}", goalDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteGoal_ShouldReturnNoContent()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        var profileDto = CreateValidProfileDto();
        await client.PostAsJsonAsync("/api/v1/users/profile", profileDto);

        var goalDto = new TrainingGoalDto
        {
            GoalType = GoalType.StrengthMilestone,
            Description = "Deadlift 200kg",
            Priority = 1
        };
        var goalResponse = await client.PostAsJsonAsync("/api/v1/users/profile/goals", goalDto);
        var createdGoal = await goalResponse.Content.ReadFromJsonAsync<TrainingGoalDto>();

        // Act
        var response = await client.DeleteAsync($"/api/v1/users/profile/goals/{createdGoal!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        
        // Verify goal is deleted
        var goalsResponse = await client.GetAsync("/api/v1/users/profile/goals");
        var goals = await goalsResponse.Content.ReadFromJsonAsync<List<TrainingGoalDto>>();
        goals.Should().NotContain(g => g.Id == createdGoal.Id);
    }

    [Fact]
    public async Task DeleteGoal_NonExistent_ShouldReturnNotFound()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        var profileDto = CreateValidProfileDto();
        await client.PostAsJsonAsync("/api/v1/users/profile", profileDto);

        // Act
        var response = await client.DeleteAsync($"/api/v1/users/profile/goals/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateInjury_WithValidData_ShouldReturnCreated()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        var profileDto = CreateValidProfileDto();
        await client.PostAsJsonAsync("/api/v1/users/profile", profileDto);

        var injuryDto = new InjuryLimitationDto
        {
            BodyPart = "Knee",
            InjuryType = InjuryType.Chronic,
            ReportedDate = DateTime.UtcNow,
            Status = InjuryStatus.Active,
            MovementRestrictions = "No deep squats"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/users/profile/injuries", injuryDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdInjury = await response.Content.ReadFromJsonAsync<InjuryLimitationDto>();
        createdInjury.Should().NotBeNull();
        createdInjury!.Id.Should().NotBe(Guid.Empty);
        createdInjury.BodyPart.Should().Be("Knee");
    }

    [Fact]
    public async Task CreateInjury_WithFutureReportedDate_ShouldReturnBadRequest()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        var profileDto = CreateValidProfileDto();
        await client.PostAsJsonAsync("/api/v1/users/profile", profileDto);

        var injuryDto = new InjuryLimitationDto
        {
            BodyPart = "Shoulder",
            InjuryType = InjuryType.Acute,
            ReportedDate = DateTime.UtcNow.AddDays(1), // Future date
            Status = InjuryStatus.Active
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/users/profile/injuries", injuryDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateInjury_WithValidData_ShouldReturnOk()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        var profileDto = CreateValidProfileDto();
        await client.PostAsJsonAsync("/api/v1/users/profile", profileDto);

        var injuryDto = new InjuryLimitationDto
        {
            BodyPart = "Ankle",
            InjuryType = InjuryType.Acute,
            ReportedDate = DateTime.UtcNow.AddDays(-7),
            Status = InjuryStatus.Active
        };
        var injuryResponse = await client.PostAsJsonAsync("/api/v1/users/profile/injuries", injuryDto);
        var createdInjury = await injuryResponse.Content.ReadFromJsonAsync<InjuryLimitationDto>();

        createdInjury!.Status = InjuryStatus.Improving;
        createdInjury.MovementRestrictions = "Light activities only";

        // Act
        var response = await client.PutAsJsonAsync($"/api/v1/users/profile/injuries/{createdInjury.Id}", createdInjury);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedInjury = await response.Content.ReadFromJsonAsync<InjuryLimitationDto>();
        updatedInjury.Should().NotBeNull();
        updatedInjury!.Status.Should().Be(InjuryStatus.Improving);
        updatedInjury.MovementRestrictions.Should().Be("Light activities only");
    }

    [Fact]
    public async Task UpdateInjury_NonExistent_ShouldReturnNotFound()
    {
        // Arrange
        var client = CreateAuthenticatedClient();
        var profileDto = CreateValidProfileDto();
        await client.PostAsJsonAsync("/api/v1/users/profile", profileDto);

        var injuryDto = new InjuryLimitationDto
        {
            BodyPart = "Back",
            InjuryType = InjuryType.Chronic,
            ReportedDate = DateTime.UtcNow,
            Status = InjuryStatus.Active
        };

        // Act
        var response = await client.PutAsJsonAsync($"/api/v1/users/profile/injuries/{Guid.NewGuid()}", injuryDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
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
            }
        };
    }
}
