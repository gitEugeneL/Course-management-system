using FluentValidation;

namespace API.Models.Dto.Users;

public sealed record CreateUserDto(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string UniversityNumber
);

public sealed class CreateUserValidator : AbstractValidator<CreateUserDto>
{
    public CreateUserValidator()
    {
        RuleFor(u => u.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(u => u.Password)
            .NotEmpty()
            .MinimumLength(8)
            .MaximumLength(20);

        RuleFor(u => u.FirstName)
            .NotEmpty();

        RuleFor(u => u.LastName)
            .NotEmpty();

        RuleFor(u => u.UniversityNumber)
            .NotEmpty()
            .MaximumLength(10);
    }
}