using FluentValidation;

namespace API.Dto.Courses;

public sealed record UpdateCourseDto(
    Guid CourseId,
    string? Description,
    int? MaxParticipant,
    bool? Finalize = false
);

public sealed class UpdateCourseDtoValidator : AbstractValidator<UpdateCourseDto>
{
    public UpdateCourseDtoValidator()
    {
        RuleFor(c => c.CourseId)
            .NotEmpty();
        
        RuleFor(c => c.Description)
            .MaximumLength(250);

        RuleFor(c => c.MaxParticipant)
            .GreaterThan(1)
            .LessThanOrEqualTo(100);
    }
}