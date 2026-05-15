using VaultHistory.User.Application.Abstractions;
using VaultHistory.User.Domain.Abstractions;
using VaultHistory.User.Domain.Users;
using VaultHistory.User.Domain.Users.Interfaces;

namespace VaultHistory.User.Application.Queries.GetUserById
{
    internal sealed class GetUserByIdQueryHandler(IUserRepository userRepository) : IQueryHandler<GetUserByIdQuery, Domain.Users.User>
    {
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<Result<Domain.Users.User>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {

            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

            if (user is null)
            {
                return Result.Failure<Domain.Users.User>(UserErrors.UserNotFound);
            }

            if (!user.IsActive)
            {
                return Result.Failure<Domain.Users.User>(UserErrors.UserInactive);
            }

            return Result.Success(user);

        }
    }
}