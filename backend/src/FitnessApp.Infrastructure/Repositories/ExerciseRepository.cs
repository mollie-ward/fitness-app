using FitnessApp.Application.Common.Interfaces;
using FitnessApp.Domain.Entities;
using FitnessApp.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace FitnessApp.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for exercise operations
/// </summary>
public class ExerciseRepository : IExerciseRepository
{
    private readonly IApplicationDbContext _context;

    public ExerciseRepository(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public async Task<Exercise?> GetByIdAsync(Guid exerciseId, CancellationToken cancellationToken = default)
    {
        return await _context.Exercises
            .Include(e => e.ExerciseContraindications)
            .Include(e => e.ExerciseEquipments)
            .Include(e => e.ExerciseMuscleGroups)
            .Include(e => e.ExerciseMovementPatterns)
            .FirstOrDefaultAsync(e => e.Id == exerciseId, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Exercise>> GetExercisesByCriteriaAsync(
        Discipline? discipline = null,
        DifficultyLevel? difficultyLevel = null,
        SessionType? sessionType = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Exercises.AsQueryable();

        if (discipline.HasValue)
            query = query.Where(e => e.PrimaryDiscipline == discipline.Value);

        if (difficultyLevel.HasValue)
            query = query.Where(e => e.DifficultyLevel == difficultyLevel.Value);

        if (sessionType.HasValue)
            query = query.Where(e => e.SessionType == sessionType.Value);

        return await query
            .Include(e => e.ExerciseContraindications)
            .Include(e => e.ExerciseEquipments)
            .Include(e => e.ExerciseMuscleGroups)
            .Include(e => e.ExerciseMovementPatterns)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Exercise>> GetSafeExercisesAsync(
        IEnumerable<InjuryType> injuryTypes,
        Discipline? discipline = null,
        DifficultyLevel? difficultyLevel = null,
        CancellationToken cancellationToken = default)
    {
        // Convert enum to strings because Contraindication.InjuryType is stored as string in DB
        // Note: For performance optimization with large datasets, consider adding an enum column to Contraindication table
        var injuryTypeStrings = injuryTypes.Select(it => it.ToString()).ToList();
        
        // Get exercises that don't have contraindications matching the injury types
        var query = _context.Exercises
            .Include(e => e.ExerciseContraindications)
            .ThenInclude(ec => ec.Contraindication)
            .Where(e => !e.ExerciseContraindications
                .Any(ec => injuryTypeStrings.Contains(ec.Contraindication!.InjuryType)));

        if (discipline.HasValue)
            query = query.Where(e => e.PrimaryDiscipline == discipline.Value);

        if (difficultyLevel.HasValue)
            query = query.Where(e => e.DifficultyLevel == difficultyLevel.Value);

        return await query
            .Include(e => e.ExerciseEquipments)
            .Include(e => e.ExerciseMuscleGroups)
            .Include(e => e.ExerciseMovementPatterns)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Exercise>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Exercises
            .Include(e => e.ExerciseContraindications)
            .Include(e => e.ExerciseEquipments)
            .Include(e => e.ExerciseMuscleGroups)
            .Include(e => e.ExerciseMovementPatterns)
            .ToListAsync(cancellationToken);
    }
}
