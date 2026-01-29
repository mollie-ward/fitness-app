using FluentValidation;
using FitnessApp.Application.DTOs;

namespace FitnessApp.Application.Validators;

/// <summary>
/// Validator for InjuryLimitationDto
/// </summary>
public class InjuryLimitationDtoValidator : AbstractValidator<InjuryLimitationDto>
{
    public InjuryLimitationDtoValidator()
    {
        RuleFor(x => x.BodyPart)
            .NotEmpty()
            .WithMessage("Body part is required")
            .MaximumLength(200)
            .WithMessage("Body part cannot exceed 200 characters");

        RuleFor(x => x.InjuryType)
            .IsInEnum()
            .WithMessage("Invalid injury type");

        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Invalid injury status");

        RuleFor(x => x.ReportedDate)
            .NotEmpty()
            .WithMessage("Reported date is required")
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("Reported date cannot be in the future");

        RuleFor(x => x.MovementRestrictions)
            .MaximumLength(1000)
            .When(x => x.MovementRestrictions != null)
            .WithMessage("Movement restrictions cannot exceed 1000 characters");
    }
}
