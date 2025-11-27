using MediatR;
using PurposePunch.Application.Interfaces;
using PurposePunch.Domain.Entities;
using PurposePunch.Domain.Enums;

namespace PurposePunch.Application.Features.Decisions;

public record PublishDecisionCommand(int DecisionId) : IRequest<int?>;

public class PublishDecisionHandler : IRequestHandler<PublishDecisionCommand, int?>
{
    private readonly IDecisionRepository _decisionRepo;
    private readonly IPublicPostRepository _postRepo;
    private readonly ICurrentUserService _currentUserService;
    private readonly IIdentityService _identityService;

    public PublishDecisionHandler(
        IDecisionRepository decisionRepo,
        IPublicPostRepository postRepo,
        ICurrentUserService currentUserService,
        IIdentityService identityService)
    {
        _decisionRepo = decisionRepo;
        _postRepo = postRepo;
        _currentUserService = currentUserService;
        _identityService = identityService;
    }

    public async Task<int?> Handle(PublishDecisionCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (string.IsNullOrEmpty(userId))
            return null;

        var decision = await _decisionRepo.GetByIdAsync(request.DecisionId);
        if (decision == null || decision.UserId != userId)
            return null;
        if (decision.Status != DecisionStatus.Reflected)
            throw new InvalidOperationException("Cannot publish a decision that has not been reflected upon yet.");
        if (decision.Visibility == Visibility.Private)
            throw new InvalidOperationException("Set visibility to public before publishing.");

        var nickname = await _identityService.GetNicknameAsync(userId);
        if (string.IsNullOrEmpty(nickname))
            throw new InvalidOperationException("User has no nickname.");

        var existingPost = await _postRepo.GetByOriginalDecisionIdAsync(decision.Id);
        if (existingPost != null)
        {
            existingPost.Title = decision.Title;
            existingPost.Description = decision.Description;
            existingPost.ActualOutcome = decision.ActualOutcome;
            existingPost.LessonsLearned = decision.LessonsLearned;
            existingPost.Satisfaction = decision.Satisfaction;
            existingPost.AuthorNickname = nickname;

            await _postRepo.UpdateAsync(existingPost);
            return existingPost.Id;
        }

        var newPost = new PublicPost
        {
            OriginalDecisionId = decision.Id,
            AuthorNickname = nickname,
            Title = decision.Title,
            Description = decision.Description,
            ActualOutcome = decision.ActualOutcome,
            LessonsLearned = decision.LessonsLearned,
            Satisfaction = decision.Satisfaction,
            PublishedAt = DateTime.UtcNow
        };

        await _postRepo.CreateAsync(newPost);
        return newPost.Id;
    }
}