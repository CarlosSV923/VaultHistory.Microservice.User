using VaultHistory.User.Application.Abstractions;
using VaultHistory.User.Domain.Users;

namespace VaultHistory.User.Application.Queries.GetUserById
{
    public record GetUserByIdQuery(UserId UserId) : IQuery<Domain.Users.User>;
}