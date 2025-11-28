using MediatR;
using PurposePunch.Application.Interfaces;

namespace PurposePunch.Application.Features.PublicPosts;

public record UpvotePostCommand(int PostId) : IRequest<int?>;

public class UpvotePostHandler : IRequestHandler<UpvotePostCommand, int?>
{
    private readonly IPublicPostRepository _repo;

    public UpvotePostHandler(IPublicPostRepository repo)
    {
        _repo = repo;
    }

    public async Task<int?> Handle(UpvotePostCommand request, CancellationToken cancellationToken)
    {
        var post = await _repo.GetByIdAsync(request.PostId);
        if (post == null)
            return null;

        post.HelpfulCount++;

        await _repo.UpdateAsync(post);

        return post.HelpfulCount;
    }
}
