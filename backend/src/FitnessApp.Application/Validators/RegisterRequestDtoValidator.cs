using FluentValidation;
using FitnessApp.Application.DTOs;

namespace FitnessApp.Application.Validators;

/// <summary>
/// Validator for RegisterRequestDto
/// </summary>
public class RegisterRequestDtoValidator : AbstractValidator<RegisterRequestDto>
{
    public RegisterRequestDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Invalid email address")
            .MaximumLength(256)
            .WithMessage("Email cannot exceed 256 characters");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required")
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters long")
            .Matches(@"[A-Z]")
            .WithMessage("Password must contain at least one uppercase letter")
            .Matches(@"[0-9]")
            .WithMessage("Password must contain at least one number")
            .Matches(@"[^a-zA-Z0-9]")
            .WithMessage("Password must contain at least one special character");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .MaximumLength(256)
            .WithMessage("Name cannot exceed 256 characters");
    }
}
