using Microsoft.EntityFrameworkCore;
using PurposePunch.Application.Interfaces;
using PurposePunch.Domain.Entities;
using PurposePunch.Infrastructure.Persistence;

namespace PurposePunch.Infrastructure.Repositories;

public class PublicPostRepository : IPublicPostRepository
{
    private readonly AppDbContext _context;

    public PublicPostRepository(AppDbContext context) => _context = context;

    public async Task<PublicPost?> GetByOriginalDecisionIdAsync(int decisionId)
    {
        return await _context.PublicPosts
            .FirstOrDefaultAsync(p => p.OriginalDecisionId == decisionId);
    }

    public async Task CreateAsync(PublicPost post)
    {
        await _context.PublicPosts.AddAsync(post);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(PublicPost post)
    {
        _context.PublicPosts.Update(post);
        await _context.SaveChangesAsync();
    }
}
