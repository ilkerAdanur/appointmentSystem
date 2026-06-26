using UserService.Dtos;
using FluentValidation;

namespace UserService.Validators;

public class CreateUserDtoValidator 
        :AbstractValidator<CreateUserDto>
{
    public CreateUserDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Email is invalid.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.");
    }
}