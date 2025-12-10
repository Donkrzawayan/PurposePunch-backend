using PurposePunch.Domain.Entities;

namespace PurposePunch.Application.Interfaces;

public interface IPublicPostRepository
{
    Task<PublicPost?> GetByOriginalDecisionIdAsync(int decisionId);
    Task CreateAsync(PublicPost post);
    Task UpdateAsync(PublicPost post);
    Task<(IEnumerable<PublicPost> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize);
    Task<PublicPost?> GetByIdAsync(int id);
    Task<bool> RegisterUpvoteAsync(int id, string voterId);
    Task<bool> IsUpvotedByUserAsync(int postId, string voterId);
    Task<HashSet<int>> GetUpvotedPostIdsForUserAsync(IEnumerable<int> postIds, string voterId);
}
