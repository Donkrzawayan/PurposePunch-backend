using FluentValidation;
using MediatR;
using PurposePunch.Application.Interfaces;
using PurposePunch.Domain.Entities;
using PurposePunch.Domain.Enums;

namespace PurposePunch.Application.Features.Decisions;

public record CreateDecisionCommand(
    string Title,
    string Description,
    string ExpectedOutcome,
    Visibility Visibility,
    DateTime ExpectedReflectionDate
) : IRequest<Decision>;

public class CreateDecisionValidator : AbstractValidator<CreateDecisionCommand>
{
    public CreateDecisionValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(60).WithMessage("The title cannot be longer than 60 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.");

        RuleFor(x => x.ExpectedReflectionDate)
            .GreaterThan(_ => DateTime.UtcNow.AddHours(1)) // lambda to ensure its evaluated at runtime
            .WithMessage("Reflection date must be at least 1 hour in the future.");
    }
}

public class CreateDecisionHandler : IRequestHandler<CreateDecisionCommand, Decision>
{
    private readonly IDecisionRepository _repo;
    private readonly ICurrentUserService _currentUserService;

    public CreateDecisionHandler(IDecisionRepository repo, ICurrentUserService currentUserService)
    {
        _repo = repo;
        _currentUserService = currentUserService;
    }

    public async Task<Decision> Handle(CreateDecisionCommand cmd, CancellationToken ct)
    {
        var userId = _currentUserService.UserId;

        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException("User is not logged in.");

        var decision = new Decision
        {
            UserId = userId,
            Title = cmd.Title,
            Description = cmd.Description,
            ExpectedOutcome = cmd.ExpectedOutcome,
            Visibility = cmd.Visibility,
            CreatedAt = DateTime.UtcNow,
            ExpectedReflectionDate = cmd.ExpectedReflectionDate,
            Status = DecisionStatus.Active
        };

        await _repo.CreateAsync(decision);

        return decision;
    }
}
