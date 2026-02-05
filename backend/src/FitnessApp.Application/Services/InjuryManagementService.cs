using Microsoft.Extensions.Logging;
using FitnessApp.Application.Common.Interfaces;
using FitnessApp.Application.DTOs;
using FitnessApp.Domain.Entities;
using FitnessApp.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace FitnessApp.Application.Services;

/// <summary>
/// Service implementation for managing user injuries and limitations
/// Handles injury reporting, contraindication logic, and exercise substitutions
/// </summary>
public class InjuryManagementService : IInjuryManagementService
{
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IExerciseRepository _exerciseRepository;
    private readonly IApplicationDbContext _context;
    private readonly IPlanAdaptationService _planAdaptationService;
    private readonly ILogger<InjuryManagementService> _logger;

    // Medical disclaimer constant
    private const string MedicalDisclaimer = "This is not medical advice. Please consult a healthcare professional for proper diagnosis and treatment.";

    public InjuryManagementService(
        IUserProfileRepository userProfileRepository,
        IExerciseRepository exerciseRepository,
        IApplicationDbContext context,
        IPlanAdaptationService planAdaptationService,
        ILogger<InjuryManagementService> logger)
    {
        _userProfileRepository = userProfileRepository;
        _exerciseRepository = exerciseRepository;
        _context = context;
        _planAdaptationService = planAdaptationService;
        _logger = logger;
    }

    public async Task<InjuryLimitationDto> ReportInjuryAsync(
        Guid userId,
        ReportInjuryDto injuryDetails,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Reporting injury for user {UserId} - Body part: {BodyPart}", 
            userId, injuryDetails.BodyPart);

        // Get user profile
        var profile = await _userProfileRepository.GetByUserIdAsync(userId, cancellationToken);
        if (profile == null)
        {
            throw new InvalidOperationException($"User profile not found for user {userId}");
        }

        // Create injury entity
        var injury = new InjuryLimitation
        {
            UserProfileId = profile.Id,
            BodyPart = injuryDetails.BodyPart,
            InjuryType = injuryDetails.InjuryType,
            ReportedDate = DateTime.UtcNow,
            Status = InjuryStatus.Active,
            MovementRestrictions = BuildMovementRestrictions(injuryDetails)
        };

        // Save injury
        var savedInjury = await _userProfileRepository.AddInjuryAsync(injury, cancellationToken);

        _logger.LogInformation("Injury {InjuryId} reported for user {UserId}", savedInjury.Id, userId);

        // Trigger plan adaptation
        try
        {
            await _planAdaptationService.AdaptForInjuryAsync(userId, savedInjury.Id, cancellationToken);
            _logger.LogInformation("Plan adaptation triggered for injury {InjuryId}", savedInjury.Id);
        }
        catch (Exception ex)
        {
            // Log the error but don't fail the injury reporting
            _logger.LogWarning(ex, "Plan adaptation failed for injury {InjuryId}, but injury was recorded", savedInjury.Id);
        }

        return MapToDto(savedInjury);
    }

    public async Task<InjuryLimitationDto> UpdateInjuryStatusAsync(
        Guid userId,
        Guid injuryId,
        UpdateInjuryStatusDto statusUpdate,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating injury {InjuryId} status for user {UserId} to {Status}", 
            injuryId, userId, statusUpdate.Status);

        // Get injury
        var injury = await _userProfileRepository.GetInjuryByIdAsync(injuryId, cancellationToken);
        if (injury == null)
        {
            throw new InvalidOperationException($"Injury {injuryId} not found");
        }

        // Verify ownership
        var profile = await _userProfileRepository.GetByUserIdAsync(userId, cancellationToken);
        if (profile == null || injury.UserProfileId != profile.Id)
        {
            throw new UnauthorizedAccessException($"User {userId} does not have access to injury {injuryId}");
        }

        // Update status
        var oldStatus = injury.Status;
        injury.Status = statusUpdate.Status;

        var updatedInjury = await _userProfileRepository.UpdateInjuryAsync(injury, cancellationToken);

        _logger.LogInformation("Injury {InjuryId} status updated from {OldStatus} to {NewStatus}", 
            injuryId, oldStatus, statusUpdate.Status);

        // Trigger plan re-evaluation if status changed
        if (oldStatus != statusUpdate.Status && 
            (statusUpdate.Status == InjuryStatus.Improving || statusUpdate.Status == InjuryStatus.Resolved))
        {
            try
            {
                await _planAdaptationService.AdaptForInjuryAsync(userId, injuryId, cancellationToken);
                _logger.LogInformation("Plan re-evaluation triggered for injury {InjuryId} status change", injuryId);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Plan re-evaluation failed for injury {InjuryId}", injuryId);
            }
        }

        return MapToDto(updatedInjury);
    }

    public async Task<InjuryLimitationDto> MarkInjuryResolvedAsync(
        Guid userId,
        Guid injuryId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Marking injury {InjuryId} as resolved for user {UserId}", injuryId, userId);

        var statusUpdate = new UpdateInjuryStatusDto
        {
            Status = InjuryStatus.Resolved
        };

        return await UpdateInjuryStatusAsync(userId, injuryId, statusUpdate, cancellationToken);
    }

    public async Task<IEnumerable<InjuryLimitationDto>> GetActiveInjuriesAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting active injuries for user {UserId}", userId);

        var profile = await _userProfileRepository.GetProfileWithInjuriesAsync(userId, cancellationToken);
        if (profile == null)
        {
            return Enumerable.Empty<InjuryLimitationDto>();
        }

        var activeInjuries = profile.InjuryLimitations
            .Where(i => i.Status == InjuryStatus.Active || i.Status == InjuryStatus.Improving)
            .Select(MapToDto)
            .ToList();

        _logger.LogInformation("Found {Count} active injuries for user {UserId}", activeInjuries.Count, userId);

        return activeInjuries;
    }

    public async Task<IEnumerable<ContraindicatedExerciseDto>> GetContraindicatedExercisesAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting contraindicated exercises for user {UserId}", userId);

        // Get active injuries
        var profile = await _userProfileRepository.GetProfileWithInjuriesAsync(userId, cancellationToken);
        if (profile == null || !profile.InjuryLimitations.Any())
        {
            return Enumerable.Empty<ContraindicatedExerciseDto>();
        }

        var activeInjuries = profile.InjuryLimitations
            .Where(i => i.Status == InjuryStatus.Active || i.Status == InjuryStatus.Improving)
            .ToList();

        if (!activeInjuries.Any())
        {
            return Enumerable.Empty<ContraindicatedExerciseDto>();
        }

        // Get contraindications from database
        var contraindicatedExercises = new List<ContraindicatedExerciseDto>();

        foreach (var injury in activeInjuries)
        {
            // Query exercises with contraindications matching this injury
            var exercisesWithContraindications = await _context.Exercises
                .Include(e => e.ExerciseContraindications)
                    .ThenInclude(ec => ec.Contraindication)
                .Where(e => e.ExerciseContraindications.Any(ec => 
                    ec.Contraindication!.InjuryType.ToLower().Contains(injury.BodyPart.ToLower()) ||
                    (injury.MovementRestrictions != null && 
                     ec.Contraindication!.MovementRestriction != null &&
                     injury.MovementRestrictions.Contains(ec.Contraindication.MovementRestriction))))
                .ToListAsync(cancellationToken);

            foreach (var exercise in exercisesWithContraindications)
            {
                var contraindication = exercise.ExerciseContraindications
                    .FirstOrDefault(ec => 
                        ec.Contraindication!.InjuryType.ToLower().Contains(injury.BodyPart.ToLower()) ||
                        (injury.MovementRestrictions != null && 
                         ec.Contraindication!.MovementRestriction != null &&
                         injury.MovementRestrictions.Contains(ec.Contraindication.MovementRestriction)));

                if (contraindication != null)
                {
                    contraindicatedExercises.Add(new ContraindicatedExerciseDto
                    {
                        ExerciseId = exercise.Id,
                        ExerciseName = exercise.Name,
                        Reason = $"Contraindicated due to {injury.BodyPart} injury",
                        AffectedBodyPart = injury.BodyPart,
                        Severity = contraindication.Severity ?? DetermineSeverity(injury)
                    });
                }
            }

            // Also check by movement patterns
            var movementPatternContraindications = await GetContraindicationsByMovementPattern(
                injury.BodyPart, 
                injury.MovementRestrictions,
                cancellationToken);

            contraindicatedExercises.AddRange(movementPatternContraindications);
        }

        // Remove duplicates
        var uniqueContraindications = contraindicatedExercises
            .GroupBy(c => c.ExerciseId)
            .Select(g => g.First())
            .ToList();

        _logger.LogInformation("Found {Count} contraindicated exercises for user {UserId}", 
            uniqueContraindications.Count, userId);

        return uniqueContraindications;
    }

    public async Task<SubstituteExerciseDto?> GetSubstituteExerciseAsync(
        Guid exerciseId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting substitute for exercise {ExerciseId} for user {UserId}", 
            exerciseId, userId);

        // Get the original exercise
        var originalExercise = await _exerciseRepository.GetByIdAsync(exerciseId, cancellationToken);
        if (originalExercise == null)
        {
            throw new InvalidOperationException($"Exercise {exerciseId} not found");
        }

        // Get user's active injuries
        var activeInjuries = await GetActiveInjuriesAsync(userId, cancellationToken);
        var injuries = activeInjuries.ToList();

        if (!injuries.Any())
        {
            _logger.LogInformation("No active injuries for user {UserId}, no substitute needed", userId);
            return null;
        }

        // Try to find a suitable substitute
        var substitute = await FindSubstituteExercise(originalExercise, injuries, cancellationToken);

        if (substitute == null)
        {
            _logger.LogWarning("No suitable substitute found for exercise {ExerciseId}", exerciseId);
            return null;
        }

        var substituteDto = new SubstituteExerciseDto
        {
            OriginalExerciseId = originalExercise.Id,
            OriginalExerciseName = originalExercise.Name,
            SubstituteExerciseId = substitute.Id,
            SubstituteExerciseName = substitute.Name,
            Reason = $"Alternative for {originalExercise.Name} due to injury",
            Description = substitute.Description,
            DifficultyLevel = substitute.DifficultyLevel,
            Instructions = substitute.Instructions
        };

        _logger.LogInformation("Found substitute {SubstituteName} for exercise {OriginalName}", 
            substitute.Name, originalExercise.Name);

        return substituteDto;
    }

    #region Helper Methods

    private string BuildMovementRestrictions(ReportInjuryDto injuryDetails)
    {
        var restrictions = new List<string>();

        if (!string.IsNullOrWhiteSpace(injuryDetails.MovementRestrictions))
        {
            restrictions.Add(injuryDetails.MovementRestrictions);
        }

        if (!string.IsNullOrWhiteSpace(injuryDetails.Severity))
        {
            restrictions.Add($"Severity: {injuryDetails.Severity}");
        }

        if (!string.IsNullOrWhiteSpace(injuryDetails.PainDescription))
        {
            restrictions.Add($"Pain: {injuryDetails.PainDescription}");
        }

        return string.Join("; ", restrictions);
    }

    private string DetermineSeverity(InjuryLimitation injury)
    {
        // Simple heuristic - could be enhanced based on injury data
        if (injury.MovementRestrictions?.Contains("Severe", StringComparison.OrdinalIgnoreCase) ?? false)
        {
            return "Absolute";
        }

        return injury.InjuryType == InjuryType.Chronic ? "Relative" : "Moderate";
    }

    private async Task<List<ContraindicatedExerciseDto>> GetContraindicationsByMovementPattern(
        string bodyPart,
        string? movementRestrictions,
        CancellationToken cancellationToken)
    {
        var contraindications = new List<ContraindicatedExerciseDto>();

        if (string.IsNullOrWhiteSpace(movementRestrictions))
        {
            return contraindications;
        }

        // Map body parts to movement patterns
        var restrictedPatterns = MapBodyPartToMovementPatterns(bodyPart, movementRestrictions);

        if (!restrictedPatterns.Any())
        {
            return contraindications;
        }

        // Get exercises with these movement patterns
        var exercises = await _context.Exercises
            .Include(e => e.ExerciseMovementPatterns)
            .Where(e => e.ExerciseMovementPatterns.Any(emp => 
                restrictedPatterns.Contains(emp.MovementPattern)))
            .ToListAsync(cancellationToken);

        foreach (var exercise in exercises)
        {
            contraindications.Add(new ContraindicatedExerciseDto
            {
                ExerciseId = exercise.Id,
                ExerciseName = exercise.Name,
                Reason = $"Restricted movement pattern for {bodyPart}",
                AffectedBodyPart = bodyPart,
                Severity = "Moderate"
            });
        }

        return contraindications;
    }

    private List<MovementPattern> MapBodyPartToMovementPatterns(string bodyPart, string movementRestrictions)
    {
        var patterns = new List<MovementPattern>();
        var bodyPartLower = bodyPart.ToLower();
        var restrictionsLower = movementRestrictions.ToLower();

        // Shoulder injuries
        if (bodyPartLower.Contains("shoulder"))
        {
            if (restrictionsLower.Contains("overhead") || restrictionsLower.Contains("press"))
            {
                patterns.Add(MovementPattern.Push);
            }
            if (restrictionsLower.Contains("pull"))
            {
                patterns.Add(MovementPattern.Pull);
            }
        }

        // Knee injuries
        if (bodyPartLower.Contains("knee"))
        {
            if (restrictionsLower.Contains("squat") || restrictionsLower.Contains("lunge"))
            {
                patterns.Add(MovementPattern.Squat);
            }
            if (restrictionsLower.Contains("impact") || restrictionsLower.Contains("jump"))
            {
                // Cardio often involves impact
                patterns.Add(MovementPattern.Cardio);
            }
        }

        // Lower back injuries
        if (bodyPartLower.Contains("back") || bodyPartLower.Contains("spine"))
        {
            if (restrictionsLower.Contains("hinge") || restrictionsLower.Contains("heavy") || 
                restrictionsLower.Contains("rotation"))
            {
                patterns.Add(MovementPattern.Hinge);
            }
        }

        // Ankle injuries
        if (bodyPartLower.Contains("ankle") || bodyPartLower.Contains("foot"))
        {
            if (restrictionsLower.Contains("impact") || restrictionsLower.Contains("run") || 
                restrictionsLower.Contains("jump"))
            {
                patterns.Add(MovementPattern.Cardio);
            }
        }

        return patterns;
    }

    private async Task<Exercise?> FindSubstituteExercise(
        Exercise originalExercise,
        List<InjuryLimitationDto> injuries,
        CancellationToken cancellationToken)
    {
        // Strategy 1: Try to find an alternative from ExerciseProgression
        var progressions = await _context.ExerciseProgressions
            .Include(ep => ep.AlternativeExercise)
                .ThenInclude(e => e!.ExerciseContraindications)
                    .ThenInclude(ec => ec.Contraindication)
            .Include(ep => ep.RegressionExercise)
                .ThenInclude(e => e!.ExerciseContraindications)
                    .ThenInclude(ec => ec.Contraindication)
            .Where(ep => ep.BaseExerciseId == originalExercise.Id)
            .ToListAsync(cancellationToken);

        // Check alternatives first
        foreach (var progression in progressions)
        {
            if (progression.AlternativeExercise != null && 
                !IsExerciseContraindicated(progression.AlternativeExercise, injuries))
            {
                return progression.AlternativeExercise;
            }
        }

        // Check regressions (easier variations)
        foreach (var progression in progressions)
        {
            if (progression.RegressionExercise != null && 
                !IsExerciseContraindicated(progression.RegressionExercise, injuries))
            {
                return progression.RegressionExercise;
            }
        }

        // Strategy 2: Find exercises with same muscle groups and discipline but different movement patterns
        var muscleGroups = originalExercise.ExerciseMuscleGroups.Select(emg => emg.MuscleGroup).ToList();
        
        var similarExercises = await _context.Exercises
            .Include(e => e.ExerciseMuscleGroups)
            .Include(e => e.ExerciseContraindications)
                .ThenInclude(ec => ec.Contraindication)
            .Include(e => e.ExerciseMovementPatterns)
            .Where(e => e.Id != originalExercise.Id &&
                        e.PrimaryDiscipline == originalExercise.PrimaryDiscipline &&
                        e.ExerciseMuscleGroups.Any(emg => muscleGroups.Contains(emg.MuscleGroup)))
            .ToListAsync(cancellationToken);

        // Find first non-contraindicated exercise with similar difficulty
        var substitute = similarExercises
            .Where(e => !IsExerciseContraindicated(e, injuries))
            .OrderBy(e => Math.Abs((int)e.DifficultyLevel - (int)originalExercise.DifficultyLevel))
            .FirstOrDefault();

        return substitute;
    }

    private bool IsExerciseContraindicated(Exercise exercise, List<InjuryLimitationDto> injuries)
    {
        foreach (var injury in injuries)
        {
            // Check explicit contraindications
            var hasContraindication = exercise.ExerciseContraindications
                .Any(ec => ec.Contraindication!.InjuryType.ToLower().Contains(injury.BodyPart.ToLower()) ||
                          (injury.MovementRestrictions != null && 
                           ec.Contraindication!.MovementRestriction != null &&
                           injury.MovementRestrictions.Contains(ec.Contraindication.MovementRestriction)));

            if (hasContraindication)
            {
                return true;
            }

            // Check movement patterns
            if (!string.IsNullOrWhiteSpace(injury.MovementRestrictions))
            {
                var restrictedPatterns = MapBodyPartToMovementPatterns(
                    injury.BodyPart, 
                    injury.MovementRestrictions);

                var hasRestrictedPattern = exercise.ExerciseMovementPatterns
                    .Any(emp => restrictedPatterns.Contains(emp.MovementPattern));

                if (hasRestrictedPattern)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private InjuryLimitationDto MapToDto(InjuryLimitation injury)
    {
        return new InjuryLimitationDto
        {
            Id = injury.Id,
            BodyPart = injury.BodyPart,
            InjuryType = injury.InjuryType,
            ReportedDate = injury.ReportedDate,
            Status = injury.Status,
            MovementRestrictions = injury.MovementRestrictions
        };
    }

    #endregion
}
