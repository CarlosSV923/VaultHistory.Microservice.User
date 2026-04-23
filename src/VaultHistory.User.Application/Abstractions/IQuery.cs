using MediatR;
using VaultHistory.User.Domain.Abstractions;

namespace VaultHistory.User.Application.Abstractions
{
    public interface IQuery : IRequest<Result>
    {
    }
    public interface IQuery<TResponse> : IRequest<Result<TResponse>>
    {
    }
}