using MediatR;
using PurposePunch.Application.Interfaces;
using PurposePunch.Domain.Entities;

namespace PurposePunch.Application.Features.Decisions;

public record CreateDecisionCommand(string Title, string Description) : IRequest<Decision>;

public class CreateDecisionHandler : IRequestHandler<CreateDecisionCommand, Decision>
{
    private readonly IDecisionRepository _repo;

    public CreateDecisionHandler(IDecisionRepository repo) => _repo = repo;

    public async Task<Decision> Handle(CreateDecisionCommand cmd, CancellationToken ct)
    {
        var decision = new Decision
        {
            Title = cmd.Title,
            Description = cmd.Description
        };

        await _repo.CreateAsync(decision);
        return decision;
    }
}
