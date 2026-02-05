using FluentValidation;
using FitnessApp.Application.DTOs;

namespace FitnessApp.Application.Validators;

/// <summary>
/// Validator for ChangePasswordRequestDto
/// </summary>
public class ChangePasswordRequestDtoValidator : AbstractValidator<ChangePasswordRequestDto>
{
    public ChangePasswordRequestDtoValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty()
            .WithMessage("Current password is required");

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .WithMessage("New password is required")
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters long")
            .Matches(@"[A-Z]")
            .WithMessage("Password must contain at least one uppercase letter")
            .Matches(@"[0-9]")
            .WithMessage("Password must contain at least one number")
            .Matches(@"[^a-zA-Z0-9]")
            .WithMessage("Password must contain at least one special character");
    }
}
