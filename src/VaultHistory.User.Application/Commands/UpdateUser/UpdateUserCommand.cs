using VaultHistory.User.Application.Abstractions;

namespace VaultHistory.User.Application.Commands.UpdateUser
{
    public record UpdateUserCommand(Domain.Users.User User) : ICommand;
}