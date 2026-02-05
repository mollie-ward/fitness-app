using Xunit;
using FluentAssertions;
using FitnessApp.Application.DTOs;
using FitnessApp.Application.Validators;

namespace FitnessApp.UnitTests.Validators;

public class RegisterRequestDtoValidatorTests
{
    private readonly RegisterRequestDtoValidator _validator;

    public RegisterRequestDtoValidatorTests()
    {
        _validator = new RegisterRequestDtoValidator();
    }

    [Fact]
    public void Validate_WithValidData_ShouldPass()
    {
        // Arrange
        var request = new RegisterRequestDto
        {
            Email = "test@example.com",
            Password = "Test123!@#",
            Name = "Test User"
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("notanemail")]
    [InlineData("@example.com")]
    public void Validate_WithInvalidEmail_ShouldFail(string email)
    {
        // Arrange
        var request = new RegisterRequestDto
        {
            Email = email,
            Password = "Test123!@#",
            Name = "Test User"
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Theory]
    [InlineData("short")]
    [InlineData("12345678")]  // No uppercase
    [InlineData("UPPERCASE123")]  // No lowercase (implicit in other tests)
    [InlineData("NoNumbers!")]  // No numbers
    [InlineData("NoSpecial123")]  // No special chars
    public void Validate_WithWeakPassword_ShouldFail(string password)
    {
        // Arrange
        var request = new RegisterRequestDto
        {
            Email = "test@example.com",
            Password = password,
            Name = "Test User"
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Password");
    }

    [Fact]
    public void Validate_WithEmptyName_ShouldFail()
    {
        // Arrange
        var request = new RegisterRequestDto
        {
            Email = "test@example.com",
            Password = "Test123!@#",
            Name = ""
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public void Validate_WithTooLongEmail_ShouldFail()
    {
        // Arrange
        var longEmail = new string('a', 250) + "@example.com";  // Over 256 chars
        var request = new RegisterRequestDto
        {
            Email = longEmail,
            Password = "Test123!@#",
            Name = "Test User"
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }
}
