using FluentValidation;

namespace VaultHistory.User.Application.UseCases.UpdateUser
{
    public class UpdateUserValidator : AbstractValidator<UpdateUserRequestDto>
    {
        public UpdateUserValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required.")
                .Must(userId => Guid.TryParse(userId, out _)).WithMessage("UserId must be a valid GUID.");

            RuleFor(x => x.FirstName)
                .MaximumLength(50).WithMessage("FirstName cannot exceed 50 characters.");

            RuleFor(x => x.LastName)
                .MaximumLength(50).WithMessage("LastName cannot exceed 50 characters.");

            RuleFor(x => x.DateOfBirth)
                .LessThan(DateOnly.FromDateTime(DateTime.UtcNow)).WithMessage("DateOfBirth must be in the past.");
        }
    }
}