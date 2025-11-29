using MediatR;
using PurposePunch.Application.Interfaces;
using PurposePunch.Domain.Entities;

namespace PurposePunch.Application.Features.Decisions;

public record GetAllDecisionsQuery() : IRequest<IEnumerable<Decision>>;

public class GetAllDecisionsQueryHandler : IRequestHandler<GetAllDecisionsQuery, IEnumerable<Decision>>
{
    private readonly IDecisionRepository _repo;
    private readonly ICurrentUserService _currentUserService;

    public GetAllDecisionsQueryHandler(IDecisionRepository repo, ICurrentUserService currentUserService)
    {
        _repo = repo;
        _currentUserService = currentUserService;
    }

    public async Task<IEnumerable<Decision>> Handle(GetAllDecisionsQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        if (string.IsNullOrEmpty(userId))
            return Enumerable.Empty<Decision>();

        return await _repo.GetAllByUserIdAsync(userId);
    }
}
