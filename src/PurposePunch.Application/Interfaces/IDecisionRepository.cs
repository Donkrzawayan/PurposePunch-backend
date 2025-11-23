using PurposePunch.Domain.Entities;

namespace PurposePunch.Application.Interfaces;

public interface IDecisionRepository
{
    Task<Decision?> GetByIdAsync(int id);
    Task<IEnumerable<Decision>> GetAllAsync();
    Task<Decision?> CreateAsync(Decision decision);
}
