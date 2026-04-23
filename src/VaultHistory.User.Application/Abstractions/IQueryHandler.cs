using MediatR;
using VaultHistory.User.Domain.Abstractions;

namespace VaultHistory.User.Application.Abstractions
{
    public interface IQueryHandler<TQuery> : IRequestHandler<TQuery, Result> where TQuery : IQuery
    {
    }

    public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>> where TQuery : IQuery<TResponse>
    {
    }
}