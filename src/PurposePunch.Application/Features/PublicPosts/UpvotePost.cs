using MediatR;
using PurposePunch.Application.Interfaces;

namespace PurposePunch.Application.Features.PublicPosts;

public record UpvotePostCommand(int PostId) : IRequest<int?>;

public class UpvotePostHandler : IRequestHandler<UpvotePostCommand, int?>
{
    private readonly IPublicPostRepository _repo;
    private readonly ICurrentUserService _currentUserService;
    private const string DevicePrefix = "DEVICE:";

    public UpvotePostHandler(IPublicPostRepository repo, ICurrentUserService currentUserService)
    {
        _repo = repo;
        _currentUserService = currentUserService;
    }

    public async Task<int?> Handle(UpvotePostCommand request, CancellationToken cancellationToken)
    {
        string voterIdentifier;

        if (!string.IsNullOrEmpty(_currentUserService.UserId))
            voterIdentifier = _currentUserService.UserId;
        else if (!string.IsNullOrEmpty(_currentUserService.DeviceId))
            voterIdentifier = $"{DevicePrefix}{_currentUserService.DeviceId}";
        else
            return null;

        var post = await _repo.GetByIdAsync(request.PostId);
        if (post == null)
            return null;

        bool added = await _repo.RegisterUpvoteAsync(request.PostId, voterIdentifier);

        return added ? post.UpvoteCount + 1 : post?.UpvoteCount;
    }
}
