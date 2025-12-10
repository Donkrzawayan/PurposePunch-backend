using MediatR;
using PurposePunch.Application.Common.Models;
using PurposePunch.Application.DTOs;
using PurposePunch.Application.Interfaces;

namespace PurposePunch.Application.Features.PublicPosts;

public record GetPublicPostsQuery(int PageNumber = 1, int PageSize = 10) : IRequest<PagedResult<PublicPostDto>>;

public class GetPublicPostsHandler : IRequestHandler<GetPublicPostsQuery, PagedResult<PublicPostDto>>
{
    private readonly IPublicPostRepository _repo;
    private readonly ICurrentUserService _currentUserService;
    private const int MaxPageSize = 50;

    public GetPublicPostsHandler(IPublicPostRepository repo, ICurrentUserService currentUserService)
    {
        _repo = repo;
        _currentUserService = currentUserService;
    }

    public async Task<PagedResult<PublicPostDto>> Handle(GetPublicPostsQuery request, CancellationToken cancellationToken)
    {
        var validPageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var validPageSize = request.PageSize < 1 ? 1 : request.PageSize;
        validPageSize = validPageSize > MaxPageSize ? MaxPageSize : validPageSize;

        var (items, totalCount) = await _repo.GetPagedAsync(validPageNumber, validPageSize);

        var voterId = _currentUserService.VoterIdentifier;
        HashSet<int> upvotedPostIds;

        if (string.IsNullOrEmpty(voterId))
            upvotedPostIds = new HashSet<int>();
        else
        {
            var postIds = items.Select(e => e.Id).ToList();
            upvotedPostIds = await _repo.GetUpvotedPostIdsForUserAsync(postIds, voterId);
        }

        var dtos = items.Select(i => new PublicPostDto(
            i.Id,
            i.AuthorNickname,
            i.Title,
            i.Description,
            i.ActualOutcome,
            i.LessonsLearned,
            i.Satisfaction,
            i.UpvoteCount,
            i.PublishedAt,
            upvotedPostIds.Contains(i.Id)
        )).ToList();

        return new PagedResult<PublicPostDto>(dtos, totalCount, validPageNumber, validPageSize);
    }
}
