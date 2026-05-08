using FluentValidation;
namespace VaultHistory.User.Application.UseCases.ChangePasswordUser
{
    public class ChangePasswordUserValidator : AbstractValidator<ChangePasswordUserRequestDto>
    {
        public ChangePasswordUserValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required.");

            RuleFor(x => x.CurrentPassword)
                .NotEmpty().WithMessage("Current password is required.");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("New password is required.")
                .MinimumLength(6).WithMessage("New password must be at least 6 characters long.");
        }
    }
}