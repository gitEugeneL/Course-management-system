using FluentValidation;

namespace API.Dto.Auth;

public sealed record RefreshDto(string RefreshToken);

public sealed class RefreshValidator : AbstractValidator<RefreshDto>
{
    public RefreshValidator()
    {
        RuleFor(r => r.RefreshToken)
            .NotEmpty();
    }
}