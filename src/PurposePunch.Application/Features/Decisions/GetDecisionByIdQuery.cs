using MediatR;
using PurposePunch.Application.Interfaces;
using PurposePunch.Domain.Entities;

namespace PurposePunch.Application.Features.Decisions;

public record GetDecisionByIdQuery(int Id) : IRequest<Decision?>;

public class GetDecisionByIdQueryHandler : IRequestHandler<GetDecisionByIdQuery, Decision?>
{
    private readonly IDecisionRepository _repo;
    private readonly ICurrentUserService _currentUserService;

    public GetDecisionByIdQueryHandler(IDecisionRepository repo, ICurrentUserService currentUserService)
    {
        _repo = repo;
        _currentUserService = currentUserService;
    }

    public async Task<Decision?> Handle(GetDecisionByIdQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        if (string.IsNullOrEmpty(userId))
            return null;

        var decision = await _repo.GetByIdAsync(request.Id);

        if (decision == null || decision.UserId != userId)
            return null;

        return decision;
    }
}
