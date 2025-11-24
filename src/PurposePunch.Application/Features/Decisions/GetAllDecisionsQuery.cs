using MediatR;
using PurposePunch.Application.Interfaces;
using PurposePunch.Domain.Entities;

namespace PurposePunch.Application.Features.Decisions;

public record GetAllDecisionsQuery() : IRequest<IEnumerable<Decision>>;

public class GetAllDecisionsQueryHandler : IRequestHandler<GetAllDecisionsQuery, IEnumerable<Decision>>
{
    private readonly IDecisionRepository _repo;

    public GetAllDecisionsQueryHandler(IDecisionRepository repo) => _repo = repo;

    public Task<IEnumerable<Decision>> Handle(GetAllDecisionsQuery request, CancellationToken cancellationToken)
    {
        return _repo.GetAllAsync();
    }
}
