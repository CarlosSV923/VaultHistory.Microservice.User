using FluentValidation;

namespace VaultHistory.User.Application.UseCases.GetUserById
{
    public class GetUserByIdValidator : AbstractValidator<GetUserByIdRequestDto>
    {
        public GetUserByIdValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required.")
                .Must(id => Guid.TryParse(id, out _)).WithMessage("UserId must be a valid GUID.");
        }
    }
}