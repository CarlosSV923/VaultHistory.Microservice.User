using VaultHistory.User.Application.Abstractions;
using VaultHistory.User.Domain.Abstractions;
using VaultHistory.User.Domain.Users.Interfaces;
using VaultHistory.User.Domain.Users;


namespace VaultHistory.User.Application.Queries.GetUserByEmail
{


    internal sealed class GetUserByEmailQueryHandler(IUserRepository userRepository) : IQueryHandler<GetUserByEmailQuery, Domain.Users.User>
    {
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<Result<Domain.Users.User>> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
        {
            try
            {

                var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

                if (user is null)
                {
                    return Result.Failure<Domain.Users.User>(UserErrors.UserNotFound);
                }

                return Result.Success(user);
            }
            catch (Exception)
            {
                return Result.Failure<Domain.Users.User>(Error.BuildInternalError("Query", "An error occurred while retrieving the user by email."));
            }

        }
    }
}