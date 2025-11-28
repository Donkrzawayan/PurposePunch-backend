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

    public async Task<(IEnumerable<PublicPost> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize)
    {
        var query = _context.PublicPosts
            .OrderByDescending(p => p.PublishedAt);

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<PublicPost?> GetByIdAsync(int id)
    {
        return await _context.PublicPosts.FindAsync(id);
    }

    public async Task IncrementUpvoteCountAsync(int id)
    {
        await _context.PublicPosts
            .Where(p => p.Id == id)
            .ExecuteUpdateAsync(s => s.SetProperty(
                p => p.HelpfulCount,
                p => p.HelpfulCount + 1));
    }
}
