using MediatR;
using PurposePunch.Application.Interfaces;

namespace PurposePunch.Application.Features.Decisions;

public record DeleteDecisionCommand(int Id) : IRequest<bool>;

public class DeleteDecisionHandler : IRequestHandler<DeleteDecisionCommand, bool>
{
    private readonly IDecisionRepository _repo;
    private readonly ICurrentUserService _currentUserService;

    public DeleteDecisionHandler(IDecisionRepository repo, ICurrentUserService currentUserService)
    {
        _repo = repo;
        _currentUserService = currentUserService;
    }

    public async Task<bool> Handle(DeleteDecisionCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (string.IsNullOrEmpty(userId))
            return false;

        var decision = await _repo.GetByIdAsync(request.Id);
        if (decision == null || decision.UserId != userId)
            return false;

        await _repo.DeleteAsync(decision);
        return true;
    }
}
