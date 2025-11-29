using MediatR;
using PurposePunch.Application.DTOs;
using PurposePunch.Application.Interfaces;
using PurposePunch.Domain.Entities;

namespace PurposePunch.Application.Features.PublicPosts;

public record GetPublicPostByIdQuery(int Id) : IRequest<PublicPostDto?>;

public class GetPublicPostByIdHandler : IRequestHandler<GetPublicPostByIdQuery, PublicPostDto?>
{
    private readonly IPublicPostRepository _repo;

    public GetPublicPostByIdHandler(IPublicPostRepository repo) => _repo = repo;

    public async Task<PublicPostDto?> Handle(GetPublicPostByIdQuery request, CancellationToken cancellationToken)
    {
        var post = await _repo.GetByIdAsync(request.Id);

        if (post == null)
            return null;

        return new PublicPostDto(
            post.Id,
            post.AuthorNickname,
            post.Title,
            post.Description,
            post.ActualOutcome,
            post.LessonsLearned,
            post.Satisfaction,
            post.UpvoteCount,
            post.PublishedAt
        );
    }
}
