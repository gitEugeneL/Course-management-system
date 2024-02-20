using FluentValidation;

namespace API.Dto.Auth;

public sealed record LoginDto(
    string Email,
    string Password
);

public sealed class LoginValidator : AbstractValidator<LoginDto>
{
    public LoginValidator()
    {
        RuleFor(l => l.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(l => l.Password)
            .NotEmpty()
            .MinimumLength(8)
            .MaximumLength(20);
    }
}

