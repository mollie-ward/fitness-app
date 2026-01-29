using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FitnessApp.Application.Common.Interfaces;
using FitnessApp.Application.DTOs;
using FitnessApp.Application.Mapping;
using FitnessApp.Domain.Entities;
using FluentValidation;

namespace FitnessApp.API.Controllers;

/// <summary>
/// Controller for user profile operations
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/users")]
[ApiVersion("1.0")]
[Authorize]
public class UserProfileController : ControllerBase
{
    private readonly IUserProfileRepository _repository;
    private readonly IValidator<UserProfileDto> _profileValidator;
    private readonly IValidator<TrainingGoalDto> _goalValidator;
    private readonly IValidator<InjuryLimitationDto> _injuryValidator;
    private readonly ILogger<UserProfileController> _logger;

    public UserProfileController(
        IUserProfileRepository repository,
        IValidator<UserProfileDto> profileValidator,
        IValidator<TrainingGoalDto> goalValidator,
        IValidator<InjuryLimitationDto> injuryValidator,
        ILogger<UserProfileController> logger)
    {
        _repository = repository;
        _profileValidator = profileValidator;
        _goalValidator = goalValidator;
        _injuryValidator = injuryValidator;
        _logger = logger;
    }

    /// <summary>
    /// Create a new user profile during onboarding
    /// </summary>
    /// <param name="profileDto">Complete profile DTO with fitness levels, goals, schedule, injuries</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created profile with unique ID</returns>
    /// <response code="201">Profile created successfully</response>
    /// <response code="400">Validation errors in request</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="409">Profile already exists for this user</response>
    [HttpPost("profile")]
    [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UserProfileDto>> CreateProfile(
        [FromBody] UserProfileDto profileDto,
        CancellationToken cancellationToken)
    {
        // Validate the profile DTO
        var validationResult = await _profileValidator.ValidateAsync(profileDto, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BadRequest(new
            {
                message = "Validation failed",
                errors = validationResult.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage })
            });
        }

        var userId = GetCurrentUserId();

        // Check if profile already exists
        var existingProfile = await _repository.GetByUserIdAsync(userId, cancellationToken);
        if (existingProfile != null)
        {
            return Conflict(new { message = "A profile already exists for this user" });
        }

        // Create the profile entity
        var profile = new UserProfile
        {
            UserId = userId,
            Name = profileDto.Name,
            Email = profileDto.Email,
            HyroxLevel = profileDto.HyroxLevel,
            RunningLevel = profileDto.RunningLevel,
            StrengthLevel = profileDto.StrengthLevel,
            ScheduleAvailability = profileDto.ScheduleAvailability?.ToEntity()
        };

        // Add training background if provided
        if (profileDto.TrainingBackground != null)
        {
            profile.TrainingBackground = profileDto.TrainingBackground.ToEntity(profile.Id);
        }

        // Add training goals if provided
        if (profileDto.TrainingGoals != null && profileDto.TrainingGoals.Any())
        {
            profile.TrainingGoals = profileDto.TrainingGoals
                .Select(g => g.ToEntity(profile.Id))
                .ToList();
        }

        // Add injuries if provided
        if (profileDto.InjuryLimitations != null && profileDto.InjuryLimitations.Any())
        {
            profile.InjuryLimitations = profileDto.InjuryLimitations
                .Select(i => i.ToEntity(profile.Id))
                .ToList();
        }

        try
        {
            var createdProfile = await _repository.CreateAsync(profile, cancellationToken);
            var result = createdProfile.ToDto();
            
            _logger.LogInformation("Created profile {ProfileId} for user {UserId}", createdProfile.Id, userId);
            
            return CreatedAtAction(
                nameof(GetProfile),
                new { version = "1" },
                result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Conflict creating profile for user {UserId}", userId);
            return Conflict(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get authenticated user's complete profile
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Profile DTO with all related data</returns>
    /// <response code="200">Profile retrieved successfully</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="404">Profile not found</response>
    [HttpGet("profile")]
    [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserProfileDto>> GetProfile(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        
        var profile = await _repository.GetCompleteProfileAsync(userId, cancellationToken);
        
        if (profile == null)
        {
            _logger.LogWarning("Profile not found for user {UserId}", userId);
            return NotFound(new { message = "Profile not found" });
        }

        return Ok(profile.ToDto());
    }

    /// <summary>
    /// Update existing user profile
    /// </summary>
    /// <param name="profileDto">Updated profile DTO</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated profile</returns>
    /// <response code="200">Profile updated successfully</response>
    /// <response code="400">Validation errors in request</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="404">Profile not found</response>
    [HttpPut("profile")]
    [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserProfileDto>> UpdateProfile(
        [FromBody] UserProfileDto profileDto,
        CancellationToken cancellationToken)
    {
        // Validate the profile DTO
        var validationResult = await _profileValidator.ValidateAsync(profileDto, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BadRequest(new
            {
                message = "Validation failed",
                errors = validationResult.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage })
            });
        }

        var userId = GetCurrentUserId();
        
        var existingProfile = await _repository.GetCompleteProfileAsync(userId, cancellationToken);
        
        if (existingProfile == null)
        {
            _logger.LogWarning("Profile not found for user {UserId}", userId);
            return NotFound(new { message = "Profile not found" });
        }

        // Update profile fields
        existingProfile.UpdateFromDto(profileDto);

        // Update training background
        if (profileDto.TrainingBackground != null)
        {
            if (existingProfile.TrainingBackground == null)
            {
                existingProfile.TrainingBackground = profileDto.TrainingBackground.ToEntity(existingProfile.Id);
            }
            else
            {
                existingProfile.TrainingBackground.HasStructuredTrainingExperience = profileDto.TrainingBackground.HasStructuredTrainingExperience;
                existingProfile.TrainingBackground.PreviousTrainingDetails = profileDto.TrainingBackground.PreviousTrainingDetails;
                existingProfile.TrainingBackground.EquipmentFamiliarity = profileDto.TrainingBackground.EquipmentFamiliarity;
                existingProfile.TrainingBackground.TrainingHistoryDetails = profileDto.TrainingBackground.TrainingHistoryDetails;
            }
        }

        var updatedProfile = await _repository.UpdateAsync(existingProfile, cancellationToken);
        
        _logger.LogInformation("Updated profile {ProfileId} for user {UserId}", updatedProfile.Id, userId);
        
        return Ok(updatedProfile.ToDto());
    }

    /// <summary>
    /// Get all training goals for authenticated user
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of goals with details</returns>
    /// <response code="200">Goals retrieved successfully</response>
    /// <response code="401">User is not authenticated</response>
    [HttpGet("profile/goals")]
    [ProducesResponseType(typeof(IEnumerable<TrainingGoalDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<TrainingGoalDto>>> GetGoals(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        
        var profile = await _repository.GetProfileWithGoalsAsync(userId, cancellationToken);
        
        if (profile == null)
        {
            return Ok(new List<TrainingGoalDto>());
        }

        var goals = profile.TrainingGoals.Select(g => g.ToDto()).ToList();
        return Ok(goals);
    }

    /// <summary>
    /// Add new training goal to profile
    /// </summary>
    /// <param name="goalDto">Goal DTO</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created goal</returns>
    /// <response code="201">Goal created successfully</response>
    /// <response code="400">Validation errors in request</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="404">Profile not found</response>
    [HttpPost("profile/goals")]
    [ProducesResponseType(typeof(TrainingGoalDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TrainingGoalDto>> CreateGoal(
        [FromBody] TrainingGoalDto goalDto,
        CancellationToken cancellationToken)
    {
        // Validate the goal DTO
        var validationResult = await _goalValidator.ValidateAsync(goalDto, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BadRequest(new
            {
                message = "Validation failed",
                errors = validationResult.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage })
            });
        }

        var userId = GetCurrentUserId();
        
        var profile = await _repository.GetByUserIdAsync(userId, cancellationToken);
        
        if (profile == null)
        {
            _logger.LogWarning("Profile not found for user {UserId}", userId);
            return NotFound(new { message = "Profile not found" });
        }

        var goal = goalDto.ToEntity(profile.Id);
        var createdGoal = await _repository.AddGoalAsync(goal, cancellationToken);
        
        _logger.LogInformation("Created goal {GoalId} for profile {ProfileId}", createdGoal.Id, profile.Id);
        
        return CreatedAtAction(
            nameof(GetGoals),
            new { version = "1" },
            createdGoal.ToDto());
    }

    /// <summary>
    /// Update specific training goal
    /// </summary>
    /// <param name="goalId">Goal ID</param>
    /// <param name="goalDto">Updated goal DTO</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated goal</returns>
    /// <response code="200">Goal updated successfully</response>
    /// <response code="400">Validation errors in request</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="404">Goal not found</response>
    [HttpPut("profile/goals/{goalId}")]
    [ProducesResponseType(typeof(TrainingGoalDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TrainingGoalDto>> UpdateGoal(
        Guid goalId,
        [FromBody] TrainingGoalDto goalDto,
        CancellationToken cancellationToken)
    {
        // Validate the goal DTO
        var validationResult = await _goalValidator.ValidateAsync(goalDto, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BadRequest(new
            {
                message = "Validation failed",
                errors = validationResult.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage })
            });
        }

        var userId = GetCurrentUserId();
        
        var existingGoal = await _repository.GetGoalByIdAsync(goalId, cancellationToken);
        
        if (existingGoal == null)
        {
            _logger.LogWarning("Goal {GoalId} not found", goalId);
            return NotFound(new { message = "Goal not found" });
        }

        // Verify the goal belongs to the current user's profile
        var profile = await _repository.GetByUserIdAsync(userId, cancellationToken);
        if (profile == null || existingGoal.UserProfileId != profile.Id)
        {
            _logger.LogWarning("Goal {GoalId} does not belong to user {UserId}", goalId, userId);
            return NotFound(new { message = "Goal not found" });
        }

        existingGoal.UpdateFromDto(goalDto);
        var updatedGoal = await _repository.UpdateGoalAsync(existingGoal, cancellationToken);
        
        _logger.LogInformation("Updated goal {GoalId}", goalId);
        
        return Ok(updatedGoal.ToDto());
    }

    /// <summary>
    /// Remove goal from profile
    /// </summary>
    /// <param name="goalId">Goal ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content</returns>
    /// <response code="204">Goal deleted successfully</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="404">Goal not found</response>
    [HttpDelete("profile/goals/{goalId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteGoal(Guid goalId, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        
        var existingGoal = await _repository.GetGoalByIdAsync(goalId, cancellationToken);
        
        if (existingGoal == null)
        {
            _logger.LogWarning("Goal {GoalId} not found", goalId);
            return NotFound(new { message = "Goal not found" });
        }

        // Verify the goal belongs to the current user's profile
        var profile = await _repository.GetByUserIdAsync(userId, cancellationToken);
        if (profile == null || existingGoal.UserProfileId != profile.Id)
        {
            _logger.LogWarning("Goal {GoalId} does not belong to user {UserId}", goalId, userId);
            return NotFound(new { message = "Goal not found" });
        }

        await _repository.DeleteGoalAsync(goalId, cancellationToken);
        
        _logger.LogInformation("Deleted goal {GoalId}", goalId);
        
        return NoContent();
    }

    /// <summary>
    /// Add injury or limitation to profile
    /// </summary>
    /// <param name="injuryDto">Injury DTO</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created injury record</returns>
    /// <response code="201">Injury created successfully</response>
    /// <response code="400">Validation errors in request</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="404">Profile not found</response>
    [HttpPost("profile/injuries")]
    [ProducesResponseType(typeof(InjuryLimitationDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<InjuryLimitationDto>> CreateInjury(
        [FromBody] InjuryLimitationDto injuryDto,
        CancellationToken cancellationToken)
    {
        // Validate the injury DTO
        var validationResult = await _injuryValidator.ValidateAsync(injuryDto, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BadRequest(new
            {
                message = "Validation failed",
                errors = validationResult.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage })
            });
        }

        var userId = GetCurrentUserId();
        
        var profile = await _repository.GetByUserIdAsync(userId, cancellationToken);
        
        if (profile == null)
        {
            _logger.LogWarning("Profile not found for user {UserId}", userId);
            return NotFound(new { message = "Profile not found" });
        }

        var injury = injuryDto.ToEntity(profile.Id);
        var createdInjury = await _repository.AddInjuryAsync(injury, cancellationToken);
        
        _logger.LogInformation("Created injury {InjuryId} for profile {ProfileId}", createdInjury.Id, profile.Id);
        
        return CreatedAtAction(
            nameof(GetProfile),
            new { version = "1" },
            createdInjury.ToDto());
    }

    /// <summary>
    /// Update injury status (e.g., mark as healing, resolved)
    /// </summary>
    /// <param name="injuryId">Injury ID</param>
    /// <param name="injuryDto">Updated injury DTO</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated injury</returns>
    /// <response code="200">Injury updated successfully</response>
    /// <response code="400">Validation errors in request</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="404">Injury not found</response>
    [HttpPut("profile/injuries/{injuryId}")]
    [ProducesResponseType(typeof(InjuryLimitationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<InjuryLimitationDto>> UpdateInjury(
        Guid injuryId,
        [FromBody] InjuryLimitationDto injuryDto,
        CancellationToken cancellationToken)
    {
        // Validate the injury DTO
        var validationResult = await _injuryValidator.ValidateAsync(injuryDto, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BadRequest(new
            {
                message = "Validation failed",
                errors = validationResult.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage })
            });
        }

        var userId = GetCurrentUserId();
        
        var existingInjury = await _repository.GetInjuryByIdAsync(injuryId, cancellationToken);
        
        if (existingInjury == null)
        {
            _logger.LogWarning("Injury {InjuryId} not found", injuryId);
            return NotFound(new { message = "Injury not found" });
        }

        // Verify the injury belongs to the current user's profile
        var profile = await _repository.GetByUserIdAsync(userId, cancellationToken);
        if (profile == null || existingInjury.UserProfileId != profile.Id)
        {
            _logger.LogWarning("Injury {InjuryId} does not belong to user {UserId}", injuryId, userId);
            return NotFound(new { message = "Injury not found" });
        }

        existingInjury.UpdateFromDto(injuryDto);
        var updatedInjury = await _repository.UpdateInjuryAsync(existingInjury, cancellationToken);
        
        _logger.LogInformation("Updated injury {InjuryId}", injuryId);
        
        return Ok(updatedInjury.ToDto());
    }

    /// <summary>
    /// Gets the current user ID from JWT claims
    /// </summary>
    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                         ?? User.FindFirst("sub")?.Value
                         ?? User.FindFirst("userId")?.Value;
        
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("User ID not found in token");
        }

        return userId;
    }
}
