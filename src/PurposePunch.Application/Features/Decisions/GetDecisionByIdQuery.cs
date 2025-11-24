using MediatR;
using PurposePunch.Application.Interfaces;
using PurposePunch.Domain.Entities;

namespace PurposePunch.Application.Features.Decisions;

public record GetDecisionByIdQuery(int Id) : IRequest<Decision?>;

public class GetDecisionByIdQueryHandler : IRequestHandler<GetDecisionByIdQuery, Decision?>
{
    private readonly IDecisionRepository _repo;

    public GetDecisionByIdQueryHandler(IDecisionRepository repo) => _repo = repo;

    public Task<Decision?> Handle(GetDecisionByIdQuery request, CancellationToken cancellationToken)
    {
        return _repo.GetByIdAsync(request.Id);
    }
}
