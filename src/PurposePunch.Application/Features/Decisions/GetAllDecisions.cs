using MediatR;
using PurposePunch.Application.DTOs;
using PurposePunch.Application.Interfaces;

namespace PurposePunch.Application.Features.Decisions;

public record GetAllDecisionsQuery() : IRequest<IEnumerable<DecisionDto>>;

public class GetAllDecisionsQueryHandler : IRequestHandler<GetAllDecisionsQuery, IEnumerable<DecisionDto>>
{
    private readonly IDecisionRepository _repo;
    private readonly ICurrentUserService _currentUserService;

    public GetAllDecisionsQueryHandler(IDecisionRepository repo, ICurrentUserService currentUserService)
    {
        _repo = repo;
        _currentUserService = currentUserService;
    }

    public async Task<IEnumerable<DecisionDto>> Handle(GetAllDecisionsQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        if (string.IsNullOrEmpty(userId))
            return Enumerable.Empty<DecisionDto>();

        var decisions = await _repo.GetAllByUserIdAsync(userId);
        return decisions.Select(d => new DecisionDto(
            d.Id,
            d.Title,
            d.Description,
            d.ExpectedOutcome,
            d.Visibility,
            d.CreatedAt,
            d.ExpectedReflectionDate,
            d.Status,
            d.ActualOutcome,
            d.LessonsLearned,
            d.PrivateNotes,
            d.Satisfaction,
            d.ReflectedAt
        )).ToList();
    }
}
