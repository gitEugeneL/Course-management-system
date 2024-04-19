using API.Dto.Users;
using FluentValidation.TestHelper;
using Xunit;

namespace API.Tests.Dto;

public class CreateUserDtoTests
{
    private readonly CreateUserValidator _validator = new();

    [Fact]
    public void ValidCreateUserDto_PassesValidation()
    {
        // Arrange
        var model = new CreateUserDto("test@example.com", "strongPassword", "John", "Doe", "1234567890");

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("", "strongPassword", "John", "Doe", "1234567890")] // Empty Email
    [InlineData("test@example.com", "", "John", "Doe", "1234567890")] // Empty Password
    [InlineData("test@example.com", "weak", "John", "Doe", "1234567890")] // Password too short
    [InlineData("test@example.com", "thisisaverylongpasswordthatexceedsthemaximumlength", "John", "Doe", "1234567890")] // Password too long
    [InlineData("notanemail", "strongPassword", "John", "Doe", "1234567890")] // Invalid Email format
    [InlineData("test@example.com", "strongPassword", "", "Doe", "1234567890")] // Empty FirstName
    [InlineData("test@example.com", "strongPassword", "John", "", "1234567890")] // Empty LastName
    [InlineData("test@example.com", "strongPassword", "John", "Doe", "")] // Empty UniversityNumber
    [InlineData("test@example.com", "strongPassword", "John", "Doe", "12345678901")] // UniversityNumber too long
    public void InvalidCreateUserDto_FailsValidation(string email, string password, string firstName, string lastName, string universityNumber)
    {
        // Arrange
        var model = new CreateUserDto(email, password, firstName, lastName, universityNumber);

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveAnyValidationError();
    }
}
