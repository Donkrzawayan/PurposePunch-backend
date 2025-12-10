using MediatR;
using PurposePunch.Application.DTOs;
using PurposePunch.Application.Interfaces;
using PurposePunch.Domain.Entities;

namespace PurposePunch.Application.Features.PublicPosts;

public record GetPublicPostByIdQuery(int Id) : IRequest<PublicPostDto?>;

public class GetPublicPostByIdHandler : IRequestHandler<GetPublicPostByIdQuery, PublicPostDto?>
{
    private readonly IPublicPostRepository _repo;
    private readonly ICurrentUserService _currentUserService;

    public GetPublicPostByIdHandler(IPublicPostRepository repo, ICurrentUserService currentUserService)
    {
        _repo = repo;
        _currentUserService = currentUserService;
    }

    public async Task<PublicPostDto?> Handle(GetPublicPostByIdQuery request, CancellationToken cancellationToken)
    {
        var post = await _repo.GetByIdAsync(request.Id);
        if (post == null)
            return null;

        var voterId = _currentUserService.VoterIdentifier;
        bool isUpvoted = voterId != null && await _repo.IsUpvotedByUserAsync(post.Id, voterId);

        return new PublicPostDto(
            post.Id,
            post.AuthorNickname,
            post.Title,
            post.Description,
            post.ActualOutcome,
            post.LessonsLearned,
            post.Satisfaction,
            post.UpvoteCount,
            post.PublishedAt,
            isUpvoted
        );
    }
}
