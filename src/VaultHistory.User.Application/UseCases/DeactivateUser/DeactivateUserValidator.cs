using FluentValidation;

namespace VaultHistory.User.Application.UseCases.DeactivateUser
{
    public class DeactivateUserValidator : AbstractValidator<DeactivateUserRequestDto>
    {
        public DeactivateUserValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required.");
        }
    }
}