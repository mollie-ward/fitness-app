using FluentValidation;
using FitnessApp.Application.DTOs;

namespace FitnessApp.Application.Validators;

/// <summary>
/// Validator for TrainingGoalDto
/// </summary>
public class TrainingGoalDtoValidator : AbstractValidator<TrainingGoalDto>
{
    public TrainingGoalDtoValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Goal description is required")
            .MaximumLength(500)
            .WithMessage("Goal description cannot exceed 500 characters");

        RuleFor(x => x.TargetDate)
            .Must(BeInTheFutureIfSpecified)
            .WithMessage("Target date must be in the future if specified");

        RuleFor(x => x.Priority)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Priority must be at least 1");

        RuleFor(x => x.GoalType)
            .IsInEnum()
            .WithMessage("Invalid goal type");

        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Invalid goal status");
    }

    private bool BeInTheFutureIfSpecified(DateTime? targetDate)
    {
        if (!targetDate.HasValue)
            return true;

        return targetDate.Value.Date >= DateTime.UtcNow.Date;
    }
}
