using Microsoft.EntityFrameworkCore;
using PurposePunch.Application.Interfaces;
using PurposePunch.Domain.Entities;
using PurposePunch.Domain.Enums;
using PurposePunch.Infrastructure.Persistence;

namespace PurposePunch.Infrastructure.Repositories;

public class DecisionRepository : IDecisionRepository
{
    private readonly AppDbContext _context;

    public DecisionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Decision>> GetAllByUserIdAsync(string userId)
    {
        return await _context.Decisions.Where(d => d.UserId == userId)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync();
    }

    public async Task<Decision?> GetByIdAsync(int id)
    {
        return await _context.Decisions.FindAsync(id);
    }

    public async Task CreateAsync(Decision decision)
    {
        await _context.Decisions.AddAsync(decision);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Decision decision)
    {
        _context.Decisions.Update(decision);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Decision decision)
    {
        _context.Decisions.Remove(decision);
        await _context.SaveChangesAsync();
    }

    #region Background Job Methods
    public async Task<int> MarkExpiredAsAbandonedAsync(DateTime cutoffDate)
    {
        return await _context.Decisions
            .Where(d => d.Status == DecisionStatus.Active && d.ExpectedReflectionDate < cutoffDate)
            .ExecuteUpdateAsync(s => s.SetProperty(
                d => d.Status, DecisionStatus.Abandoned));
    }
    #endregion
}
