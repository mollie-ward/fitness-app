using FluentValidation;
using FitnessApp.Application.DTOs;

namespace FitnessApp.Application.Validators;

/// <summary>
/// Validator for LoginRequestDto
/// </summary>
public class LoginRequestDtoValidator : AbstractValidator<LoginRequestDto>
{
    public LoginRequestDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Invalid email address");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required");
    }
}
