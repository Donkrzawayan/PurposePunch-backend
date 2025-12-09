using FluentValidation;
using FluentValidation.Results;
using MediatR;
using PurposePunch.Application.Interfaces;

namespace PurposePunch.Application.Features.Auth;

public record RegisterCommand(string Email, string Password) : IRequest;

public class RegisterValidator : AbstractValidator<RegisterCommand>
{
    public RegisterValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
    }
}

public class RegisterHandler : IRequestHandler<RegisterCommand>
{
    private readonly IIdentityService _identityService;

    public RegisterHandler(IIdentityService identityService) => _identityService = identityService;

    public async Task Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var (isSuccess, errors) = await _identityService.RegisterAsync(request.Email, request.Password);

        if (!isSuccess)
        {
            var failures = errors
                .Select(error => new ValidationFailure("Registration", error))
                .ToList();
            throw new ValidationException(failures);
        }
    }
}
