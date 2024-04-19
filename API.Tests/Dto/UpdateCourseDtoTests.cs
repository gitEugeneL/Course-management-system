using API.Dto.Courses;
using FluentValidation.TestHelper;
using Xunit;

namespace API.Tests.Dto;

public class UpdateCourseDtoTests
{
    private readonly UpdateCourseDtoValidator _validator = new();

    [Theory]
    [InlineData("Updated description", 50, false)] // Valid data
    [InlineData(null, null, true)] // Valid data with null fields
    public void ValidUpdateCourseDto_PassesValidation(string description, int? maxParticipants, bool? finalize)
    {
        // Arrange
        var model = new UpdateCourseDto(Guid.NewGuid(), description, maxParticipants, finalize);

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("Updated description", 0, false)] // MaxParticipants less than or equal to zero
    [InlineData("Updated description", 101, false)] // MaxParticipants greater than 100
    public void InvalidUpdateCourseDto_FailsValidation(string description, int? maxParticipants, bool? finalize)
    {
        // Arrange
        var model = new UpdateCourseDto(Guid.NewGuid(), description, maxParticipants, finalize);

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveAnyValidationError();
    }
}