using FluentValidation;
using MediatR;
using VaultHistory.User.Application.Abstractions;
using VaultHistory.User.Application.Exceptions;
using ValidationException = VaultHistory.User.Application.Exceptions.ValidationException;

namespace VaultHistory.User.Application.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse>(
        IEnumerable<IValidator<TRequest>> validators
    ) : IPipelineBehavior<TRequest, TResponse> where TRequest : IUseCaseBase
    {
        public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (!validators.Any())
            {
                return next(cancellationToken);
            }

            var context = new ValidationContext<TRequest>(request);

            var validationResults = validators
                .Select(v => v.Validate(context))
                .Where(result => !result.IsValid)
                .SelectMany(result => result.Errors)
                .Where(error => error != null)
                .ToList();

            var validationErrors = validationResults.Select(error => new ValidateError
            (
                 error.PropertyName,
                 error.ErrorMessage
            )).ToList();

            if (validationErrors.Count != 0)
            {
                throw new ValidationException(validationErrors);
            }

            return next(cancellationToken);
        }
    }
}