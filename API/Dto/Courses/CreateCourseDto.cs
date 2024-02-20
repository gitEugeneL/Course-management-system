using FluentValidation;

namespace API.Dto.Courses;

public sealed record CreateCourseDto(
    string Name,
    string Description,
    int MaxParticipants
);

public sealed class CreateCourseValidator : AbstractValidator<CreateCourseDto>
{
    public CreateCourseValidator()
    {
        RuleFor(c => c.Name)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(c => c.Description)
            .MaximumLength(250);

        RuleFor(c => c.MaxParticipants)
            .NotEmpty()
            .GreaterThan(1)
            .LessThanOrEqualTo(100);
    }
}
