using VaultHistory.User.Application.Abstractions;
using VaultHistory.User.Domain.Users;


namespace VaultHistory.User.Application.Queries.VerifyUserEmailExists
{
    public record VerifyUserEmailExistsQuery(Email Email) : IQuery;
}