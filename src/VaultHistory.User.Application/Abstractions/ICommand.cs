using MediatR;
using VaultHistory.User.Domain.Abstractions;

namespace VaultHistory.User.Application.Abstractions
{
    public interface ICommand : IRequest<Result>
    {
    }

    public interface ICommand<TResponse> : IRequest<Result<TResponse>>
    {
    }
}