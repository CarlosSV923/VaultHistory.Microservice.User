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
            try
            {
                var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

                if (user is null)
                {
                    return Result.Failure<Domain.Users.User>(UserErrors.UserNotFound);
                }

                return Result.Success(user);
            }
            catch (Exception)
            {
                return Result.Failure<Domain.Users.User>(Error.BuildInternalError("Query", "An error occurred while retrieving the user by ID."));
            }
        }
    }
}