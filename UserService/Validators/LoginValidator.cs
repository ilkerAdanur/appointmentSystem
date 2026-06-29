using FluentValidation;
using UserService.Dtos;

namespace UserService.Validators;

public class LoginValidator : AbstractValidator<LoginUserDto>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty();
    }
}