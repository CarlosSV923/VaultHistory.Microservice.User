using MediatR;
using VaultHistory.User.Domain.Abstractions;

namespace VaultHistory.User.Application.Abstractions
{
    public interface IUseCase : IRequest<Result>, IUseCaseBase
    {
    }

    public interface IUseCase<TResponse> : IRequest<Result<TResponse>>, IUseCaseBase
    {
    }

    public interface IUseCaseBase
    {
    }
}