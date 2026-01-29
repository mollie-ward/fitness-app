using FluentValidation;
using FitnessApp.Application.DTOs;

namespace FitnessApp.Application.Validators;

/// <summary>
/// Validator for ScheduleAvailabilityDto
/// </summary>
public class ScheduleAvailabilityDtoValidator : AbstractValidator<ScheduleAvailabilityDto>
{
    public ScheduleAvailabilityDtoValidator()
    {
        RuleFor(x => x.MinimumSessionsPerWeek)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Minimum sessions per week must be at least 1");

        RuleFor(x => x.MaximumSessionsPerWeek)
            .GreaterThanOrEqualTo(x => x.MinimumSessionsPerWeek)
            .WithMessage("Maximum sessions per week must be at least equal to minimum sessions");

        RuleFor(x => x)
            .Must(HaveAtLeastOneDay)
            .WithMessage("At least one training day must be selected");

        RuleFor(x => x)
            .Must(MaxSessionsNotExceedAvailableDays)
            .WithMessage("Maximum sessions cannot exceed available days");
    }

    private bool HaveAtLeastOneDay(ScheduleAvailabilityDto dto)
    {
        var availableDays = GetAvailableDaysCount(dto);
        return availableDays > 0;
    }

    private bool MaxSessionsNotExceedAvailableDays(ScheduleAvailabilityDto dto)
    {
        var availableDays = GetAvailableDaysCount(dto);
        return dto.MaximumSessionsPerWeek <= availableDays;
    }

    private int GetAvailableDaysCount(ScheduleAvailabilityDto dto)
    {
        return (dto.Monday ? 1 : 0) +
               (dto.Tuesday ? 1 : 0) +
               (dto.Wednesday ? 1 : 0) +
               (dto.Thursday ? 1 : 0) +
               (dto.Friday ? 1 : 0) +
               (dto.Saturday ? 1 : 0) +
               (dto.Sunday ? 1 : 0);
    }
}
