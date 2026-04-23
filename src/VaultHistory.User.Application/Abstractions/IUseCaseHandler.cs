using MediatR;
using VaultHistory.User.Domain.Abstractions;

namespace VaultHistory.User.Application.Abstractions
{
    public interface IUseCaseHandler<TUseCase> : IRequestHandler<TUseCase, Result> where TUseCase : IUseCase
    {
    }

    public interface IUseCaseHandler<TUseCase, TResponse> : IRequestHandler<TUseCase, Result<TResponse>> where TUseCase : IUseCase<TResponse>
    {
    }
}