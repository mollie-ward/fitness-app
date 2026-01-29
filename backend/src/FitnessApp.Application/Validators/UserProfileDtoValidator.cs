using FluentValidation;
using FitnessApp.Application.DTOs;

namespace FitnessApp.Application.Validators;

/// <summary>
/// Validator for UserProfileDto
/// </summary>
public class UserProfileDtoValidator : AbstractValidator<UserProfileDto>
{
    public UserProfileDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .MaximumLength(200)
            .WithMessage("Name cannot exceed 200 characters");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Invalid email address")
            .MaximumLength(320)
            .WithMessage("Email cannot exceed 320 characters");

        RuleFor(x => x.HyroxLevel)
            .IsInEnum()
            .WithMessage("Invalid HYROX fitness level");

        RuleFor(x => x.RunningLevel)
            .IsInEnum()
            .WithMessage("Invalid running fitness level");

        RuleFor(x => x.StrengthLevel)
            .IsInEnum()
            .WithMessage("Invalid strength fitness level");

        RuleFor(x => x.ScheduleAvailability)
            .SetValidator(new ScheduleAvailabilityDtoValidator()!)
            .When(x => x.ScheduleAvailability != null);

        RuleForEach(x => x.TrainingGoals)
            .SetValidator(new TrainingGoalDtoValidator());

        RuleForEach(x => x.InjuryLimitations)
            .SetValidator(new InjuryLimitationDtoValidator());
    }
}
