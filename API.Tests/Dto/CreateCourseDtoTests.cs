using API.Dto.Courses;
using FluentValidation.TestHelper;
using Xunit;

namespace API.Tests.Dto;

public class CreateCourseDtoTests
{
    private readonly CreateCourseValidator _validator = new();
    
    [Theory]
    [InlineData("Math", "Advanced Mathematics course", 50)] // Valid data
    [InlineData("English", "English language course", 30)] // Valid data
    public void ValidCreateCourseDto_PassesValidation(string name, string description, int maxParticipants)
    {
        // Arrange
        var model = new CreateCourseDto(name, description, maxParticipants);

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("", "Advanced Mathematics course", 50)] // Empty Name
    [InlineData("Math", "Advanced Mathematics course", 0)] // MaxParticipants less than or equal to zero
    [InlineData("Math", "Advanced Mathematics course", 101)] // MaxParticipants greater than 100
    public void InvalidCreateCourseDto_FailsValidation(string name, string description, int maxParticipants)
    {
        // Arrange
        var model = new CreateCourseDto(name, description, maxParticipants);

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveAnyValidationError();
    }
}