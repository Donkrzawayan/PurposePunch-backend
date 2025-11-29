using MediatR;
using PurposePunch.Application.DTOs;
using PurposePunch.Application.Interfaces;

namespace PurposePunch.Application.Features.Decisions;

public record GetDecisionByIdQuery(int Id) : IRequest<DecisionDto?>;

public class GetDecisionByIdQueryHandler : IRequestHandler<GetDecisionByIdQuery, DecisionDto?>
{
    private readonly IDecisionRepository _repo;
    private readonly ICurrentUserService _currentUserService;

    public GetDecisionByIdQueryHandler(IDecisionRepository repo, ICurrentUserService currentUserService)
    {
        _repo = repo;
        _currentUserService = currentUserService;
    }

    public async Task<DecisionDto?> Handle(GetDecisionByIdQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        if (string.IsNullOrEmpty(userId))
            return null;

        var decision = await _repo.GetByIdAsync(request.Id);

        if (decision == null || decision.UserId != userId)
            return null;

        return new DecisionDto(
            decision.Id,
            decision.Title,
            decision.Description,
            decision.ExpectedOutcome,
            decision.Visibility,
            decision.CreatedAt,
            decision.ExpectedReflectionDate,
            decision.Status,
            decision.ActualOutcome,
            decision.LessonsLearned,
            decision.PrivateNotes,
            decision.Satisfaction,
            decision.ReflectedAt
        );
    }
}
