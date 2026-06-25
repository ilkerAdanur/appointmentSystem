using AppointmentService.Dtos;
using FluentValidation;

namespace AppointmentService.Validators;

public class CreateAppointmentDtoValidator
    : AbstractValidator<CreateAppointmentDto>
{
    public CreateAppointmentDtoValidator()
    {
        RuleFor(x => x.AppointmentDate)
            .Must(BeOverNow)
            .WithMessage("Appointment date must be in the future.");
        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required.");
        RuleFor(x => x.UserId)
            .GreaterThan(0)
            .WithMessage("UserId must be greater than 0.");
    }

    private bool BeOverNow(DateTime date)
    {
        return date > DateTime.Now;
    }
}