using MediatR;
using System.Runtime.CompilerServices;

namespace VaultHistory.User.Api.UnitTests.TestDoubles;

internal sealed class FakeMediator(Func<object, CancellationToken, object?> sendHandler) : IMediator
{
    public Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : IRequest
    {
        sendHandler(request!, cancellationToken);
        return Task.CompletedTask;
    }

    public Task<object?> Send(object request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(sendHandler(request, cancellationToken));
    }

    public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        var response = sendHandler(request, cancellationToken);
        return Task.FromResult((TResponse)response!);
    }

    public Task Publish(object notification, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification
    {
        return Task.CompletedTask;
    }

    public async IAsyncEnumerable<TResponse> CreateStream<TResponse>(IStreamRequest<TResponse> request, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        yield break;
    }

    public async IAsyncEnumerable<object?> CreateStream(object request, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        yield break;
    }
}
