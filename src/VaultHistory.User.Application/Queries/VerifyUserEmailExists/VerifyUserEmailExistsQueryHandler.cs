using VaultHistory.User.Application.Abstractions;
using VaultHistory.User.Domain.Abstractions;
using VaultHistory.User.Domain.Users.Interfaces;

namespace VaultHistory.User.Application.Queries.VerifyUserEmailExists
{
    internal sealed class VerifyUserEmailExistsQueryHandler(IUserRepository userRepository) : IQueryHandler<VerifyUserEmailExistsQuery, bool>
    {
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<Result<bool>> Handle(VerifyUserEmailExistsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userRepository.VerifyEmailExistsAsync(request.Email, cancellationToken);
                return Result.Success(user);
            }
            catch (Exception)
            {
                return Result.Failure<bool>(Error.BuildInternalError("Query", "An error occurred while verifying if the user email exists."));
            }
        }
    }
}