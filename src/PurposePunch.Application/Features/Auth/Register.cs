using FluentValidation;
using MediatR;
using PurposePunch.Application.Interfaces;

namespace PurposePunch.Application.Features.Auth;

public record RegisterCommand(string Email, string Password) : IRequest<string[]>;

public class RegisterValidator : AbstractValidator<RegisterCommand>
{
    public RegisterValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
    }
}

public class RegisterHandler : IRequestHandler<RegisterCommand, string[]>
{
    private readonly IIdentityService _identityService;

    public RegisterHandler(IIdentityService identityService) => _identityService = identityService;

    public async Task<string[]> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var (isSuccess, errors) = await _identityService.RegisterAsync(request.Email, request.Password);

        if (!isSuccess)
            return errors;

        return Array.Empty<string>();
    }
}
