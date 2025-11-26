using FluentValidation;
using MediatR;
using PurposePunch.Application.Interfaces;

namespace PurposePunch.Application.Features.Auth;

public record LoginCommand(string Email, string Password) : IRequest<string>;

public class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}

public class LoginHandler : IRequestHandler<LoginCommand, string>
{
    private readonly IIdentityService _identityService;

    public LoginHandler(IIdentityService identityService) => _identityService = identityService;

    public async Task<string> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        return await _identityService.LoginAsync(request.Email, request.Password);
    }
}
