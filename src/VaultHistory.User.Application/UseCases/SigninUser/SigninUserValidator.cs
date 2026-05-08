using FluentValidation;

namespace VaultHistory.User.Application.UseCases.SigninUser
{
    public class SigninUserValidator : AbstractValidator<SigninUserRequestDto>
    {
        public SigninUserValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.");
        }
    }
}