
using VaultHistory.User.Application.Abstractions;

namespace VaultHistory.User.Application.Commands.CreateUser
{
    public record CreateUserCommand(Domain.Users.User User) : ICommand;
}