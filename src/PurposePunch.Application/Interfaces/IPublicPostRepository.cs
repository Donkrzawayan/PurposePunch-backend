using PurposePunch.Domain.Entities;

namespace PurposePunch.Application.Interfaces;

public interface IPublicPostRepository
{
    Task<PublicPost?> GetByOriginalDecisionIdAsync(int decisionId);
    Task CreateAsync(PublicPost post);
    Task UpdateAsync(PublicPost post);
}
