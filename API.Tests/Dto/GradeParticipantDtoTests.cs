using API.Dto.Participants;
using FluentValidation.TestHelper;
using Xunit;

namespace API.Tests.Dto;

public class GradeParticipantDtoTests
{
    private readonly GradeParticipantDtoValidator _validator = new();

    [Fact]
    public void ValidGradeParticipantDto_PassesValidation()
    {
        // arrange 
        var model = new GradeParticipantDto(
            Guid.NewGuid(), 
            Guid.NewGuid(), 
            5, 
            "This is a valid note within 250 characters limit."
        );

        // act
        var result = _validator.TestValidate(model);

        // assert
        result.ShouldNotHaveAnyValidationErrors();
    }
        
    [Theory]
    [InlineData(0, null)] // Invalid Grade (too low)
    [InlineData(7, null)] // Invalid Grade (too high)
    public void InvalidGradeParticipantDto_FailsValidation(int grade, string professorNote)
    {
        // Arrange
        var gradeParticipantDto = new GradeParticipantDto(
            Guid.NewGuid(),
            Guid.NewGuid(),
            grade,
            professorNote
        );

        // Act
        var result = _validator.TestValidate(gradeParticipantDto);

        // Assert
        result.ShouldHaveAnyValidationError();
    }
}