using MediatR;
using PurposePunch.Application.Common.Models;
using PurposePunch.Application.Interfaces;
using PurposePunch.Domain.Entities;

namespace PurposePunch.Application.Features.PublicPosts;

public record GetPublicPostsQuery(int PageNumber = 1, int PageSize = 10) : IRequest<PagedResult<PublicPost>>;

public class GetPublicPostsHandler : IRequestHandler<GetPublicPostsQuery, PagedResult<PublicPost>>
{
    private readonly IPublicPostRepository _repo;
    private const int MaxPageSize = 50;

    public GetPublicPostsHandler(IPublicPostRepository repo) => _repo = repo;

    public async Task<PagedResult<PublicPost>> Handle(GetPublicPostsQuery request, CancellationToken cancellationToken)
    {
        var validPageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var validPageSize = request.PageSize < 1 ? 1 : request.PageSize;
        validPageSize = validPageSize > MaxPageSize ? MaxPageSize : validPageSize;

        var (items, totalCount) = await _repo.GetPagedAsync(validPageNumber, validPageSize);

        return new PagedResult<PublicPost>(items, totalCount, validPageNumber, validPageSize);
    }
}
