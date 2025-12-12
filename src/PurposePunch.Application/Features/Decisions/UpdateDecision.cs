using FluentValidation;
using MediatR;
using PurposePunch.Application.Interfaces;
using PurposePunch.Domain.Entities;
using PurposePunch.Domain.Enums;

namespace PurposePunch.Application.Features.Decisions;

public record UpdateDecisionCommand(
    int Id,
    string Title,
    string Description,
    string ExpectedOutcome,
    Visibility Visibility,
    DateTime? ExpectedReflectionDate,

    // PHASE 2: Reflection
    string? ActualOutcome,
    string? LessonsLearned,
    string? PrivateNotes,
    SatisfactionScale? Satisfaction
) : IRequest<bool>;

public class UpdateDecisionValidator : AbstractValidator<UpdateDecisionCommand>
{
    public UpdateDecisionValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(60).WithMessage("Title must not exceed 60 characters.");

        RuleFor(x => x.Description).NotEmpty().WithMessage("Description is required.");

        RuleFor(x => x.Visibility).IsInEnum().WithMessage("Invalid visibility level.");

        RuleFor(x => x.ExpectedReflectionDate)
            .GreaterThan(_ => DateTime.UtcNow.AddHours(1)) // lambda to ensure it is evaluated at runtime
            .When(x => x.ExpectedReflectionDate.HasValue)
            .WithMessage("Reflection date must be at least 1 hour in the future or missing.");
    }
}

public class UpdateDecisionHandler : IRequestHandler<UpdateDecisionCommand, bool>
{
    private readonly IDecisionRepository _repo;
    private readonly ICurrentUserService _currentUserService;

    public UpdateDecisionHandler(IDecisionRepository repo, ICurrentUserService currentUserService)
    {
        _repo = repo;
        _currentUserService = currentUserService;
    }

    public async Task<bool> Handle(UpdateDecisionCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException("User is not logged in.");

        var decision = await _repo.GetByIdAsync(request.Id);

        if (decision == null || decision.UserId != userId)
            return false;

        decision.Title = request.Title.Trim();
        decision.Description = request.Description.Trim();
        decision.ExpectedOutcome = request.ExpectedOutcome.Trim();
        decision.Visibility = request.Visibility;
        if (request.ExpectedReflectionDate.HasValue)
            decision.ExpectedReflectionDate = request.ExpectedReflectionDate.Value;

        // PHASE 2: Reflection
        decision.ActualOutcome = request.ActualOutcome?.Trim();
        decision.LessonsLearned = request.LessonsLearned?.Trim();
        decision.PrivateNotes = request.PrivateNotes?.Trim();
        decision.Satisfaction = request.Satisfaction;

        decision = UpdateDecisionStatus(request, decision);

        await _repo.UpdateAsync(decision);

        return true;
    }

    private static Decision UpdateDecisionStatus(UpdateDecisionCommand request, Decision decision)
    {
        bool hasReflectionContent = !string.IsNullOrWhiteSpace(request.ActualOutcome)
                                    || !string.IsNullOrWhiteSpace(request.LessonsLearned)
                                    || request.Satisfaction.HasValue;

        if (hasReflectionContent
            && (decision.Status == DecisionStatus.Active || decision.Status == DecisionStatus.Abandoned))
        {
            decision.Status = DecisionStatus.Reflected;
            decision.ReflectedAt = DateTime.UtcNow;
        }
        else if (!hasReflectionContent && decision.Status == DecisionStatus.Reflected)
        {
            if (decision.ExpectedReflectionDate > DateTime.UtcNow)
                decision.Status = DecisionStatus.Active;
            else
                decision.Status = DecisionStatus.Abandoned;
            decision.ReflectedAt = null;
        }

        return decision;
    }
}
