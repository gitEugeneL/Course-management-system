using FluentValidation;

namespace API.Dto.Participants;

public sealed record GradeParticipantDto(
    Guid UserId,
    Guid CourseId,
    int Grade,
    string? ProfessorNote
);

public sealed class GradeParticipantDtoValidator : AbstractValidator<GradeParticipantDto>
{
    public GradeParticipantDtoValidator()
    {
        RuleFor(g => g.UserId)
            .NotEmpty();

        RuleFor(g => g.CourseId)
            .NotEmpty();

        RuleFor(g => g.Grade)
            .NotEmpty()
            .GreaterThan(1)
            .LessThanOrEqualTo(6);

        RuleFor(g => g.ProfessorNote)
            .MaximumLength(250);
    }
}