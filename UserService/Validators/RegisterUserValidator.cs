using FluentValidation;
using UserService.Dtos;

namespace UserService.Validators;

public class RegisterUserValidator
    : AbstractValidator<RegisterUserDto>
{
    public RegisterUserValidator ()
    {
        RuleFor(x => x.Name)
        .NotEmpty()
        .MaximumLength(100);

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Email is invalid.");

        RuleFor(x =>x.Password)
        .NotEmpty()
        .MinimumLength(8)
        .Matches("[A-Z]")
        .WithMessage("Password must contain at least one uppercase letter.")
        .Matches("[a-z]")
        .WithMessage("Password must contain at least one lowercase letter.")
        .Matches("[0-9]")
        .WithMessage("Password must contain at least one number.");
        
    }
}