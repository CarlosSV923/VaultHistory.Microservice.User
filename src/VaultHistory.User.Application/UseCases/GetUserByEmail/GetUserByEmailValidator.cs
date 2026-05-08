using FluentValidation;

namespace VaultHistory.User.Application.UseCases.GetUserByEmail
{
    public class GetUserByEmailValidator : AbstractValidator<GetUserByEmailRequestDto>
    {
        public GetUserByEmailValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");
        }
    }
}