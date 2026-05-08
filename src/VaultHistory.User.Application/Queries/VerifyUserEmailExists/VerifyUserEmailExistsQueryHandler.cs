using VaultHistory.User.Application.Abstractions;
using VaultHistory.User.Domain.Abstractions;
using VaultHistory.User.Domain.Users;
using VaultHistory.User.Domain.Users.Interfaces;

namespace VaultHistory.User.Application.Queries.VerifyUserEmailExists
{
    internal sealed class VerifyUserEmailExistsQueryHandler(IUserRepository userRepository) : IQueryHandler<VerifyUserEmailExistsQuery>
    {
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<Result> Handle(VerifyUserEmailExistsQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.VerifyEmailExistsAsync(request.Email, cancellationToken);
            if (!user)
            {
                return Result.Failure(UserErrors.UserNotFound);
            }
            return Result.Success();
        }
    }
}