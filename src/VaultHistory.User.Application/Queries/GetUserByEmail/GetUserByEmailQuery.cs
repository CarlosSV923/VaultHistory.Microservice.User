using VaultHistory.User.Application.Abstractions;
using VaultHistory.User.Domain.Users;

namespace VaultHistory.User.Application.Queries.GetUserByEmail
{
    public record GetUserByEmailQuery(Email Email) : IQuery<Domain.Users.User>;
}