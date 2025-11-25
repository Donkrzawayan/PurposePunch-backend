using Microsoft.EntityFrameworkCore;
using PurposePunch.Application.Interfaces;
using PurposePunch.Domain.Entities;
using PurposePunch.Infrastructure.Persistence;

namespace PurposePunch.Infrastructure.Repositories;

public class DecisionRepository : IDecisionRepository
{
    private readonly AppDbContext _context;

    public DecisionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Decision>> GetAllAsync()
    {
        return await _context.Decisions.ToListAsync();
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
}
