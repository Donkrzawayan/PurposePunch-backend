using FluentValidation;
using MediatR;
using PurposePunch.Application.Interfaces;
using PurposePunch.Domain.Entities;

namespace PurposePunch.Application.Features.Decisions;

public record CreateDecisionCommand(string Title, string Description) : IRequest<Decision>;

public class CreateDecisionValidator : AbstractValidator<CreateDecisionCommand>
{
    public CreateDecisionValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(60).WithMessage("The title cannot be longer than 60 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.");
    }
}

public class CreateDecisionHandler : IRequestHandler<CreateDecisionCommand, Decision>
{
    private readonly IDecisionRepository _repo;

    public CreateDecisionHandler(IDecisionRepository repo) => _repo = repo;

    public async Task<Decision> Handle(CreateDecisionCommand cmd, CancellationToken ct)
    {
        var decision = new Decision
        {
            Title = cmd.Title,
            Description = cmd.Description,
            UserId = "temp-user-id"
        };

        await _repo.CreateAsync(decision);
        return decision;
    }
}
