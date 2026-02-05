using Microsoft.EntityFrameworkCore;
using FitnessApp.Application.Common.Interfaces;
using FitnessApp.Domain.Entities;

namespace FitnessApp.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for PlanAdaptation entity operations
/// </summary>
public class PlanAdaptationRepository : IPlanAdaptationRepository
{
    private readonly IApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the PlanAdaptationRepository
    /// </summary>
    /// <param name="context">The database context</param>
    public PlanAdaptationRepository(IApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc />
    public async Task<PlanAdaptation> AddAsync(PlanAdaptation adaptation, CancellationToken cancellationToken = default)
    {
        if (adaptation == null)
            throw new ArgumentNullException(nameof(adaptation));

        await _context.PlanAdaptations.AddAsync(adaptation, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return adaptation;
    }

    /// <inheritdoc />
    public async Task<PlanAdaptation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.PlanAdaptations
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<PlanAdaptation>> GetByPlanIdAsync(Guid planId, CancellationToken cancellationToken = default)
    {
        return await _context.PlanAdaptations
            .Where(a => a.PlanId == planId)
            .OrderByDescending(a => a.AppliedAt)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<PlanAdaptation?> GetMostRecentByPlanIdAsync(Guid planId, CancellationToken cancellationToken = default)
    {
        return await _context.PlanAdaptations
            .Where(a => a.PlanId == planId)
            .OrderByDescending(a => a.AppliedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task UpdateAsync(PlanAdaptation adaptation, CancellationToken cancellationToken = default)
    {
        if (adaptation == null)
            throw new ArgumentNullException(nameof(adaptation));

        _context.PlanAdaptations.Update(adaptation);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(PlanAdaptation adaptation, CancellationToken cancellationToken = default)
    {
        if (adaptation == null)
            throw new ArgumentNullException(nameof(adaptation));

        _context.PlanAdaptations.Remove(adaptation);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
