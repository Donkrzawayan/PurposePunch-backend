using PurposePunch.Domain.Entities;

namespace PurposePunch.Application.Interfaces;

public interface IDecisionRepository
{
    Task<Decision?> GetByIdAsync(int id);
    Task<IEnumerable<Decision>> GetAllByUserIdAsync(string userId);
    Task CreateAsync(Decision decision);
    Task UpdateAsync(Decision decision);

    Task<int> MarkExpiredAsAbandonedAsync(DateTime cutoffDate);
}
