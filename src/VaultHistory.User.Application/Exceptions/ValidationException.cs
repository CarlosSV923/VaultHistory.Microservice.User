namespace VaultHistory.User.Application.Exceptions
{
    public sealed record ValidateError(string PropertyName, string ErrorMessage);
    public sealed class ValidationException(IEnumerable<ValidateError> errors) : Exception("Validation error")
    {
        public IEnumerable<ValidateError> Errors { get; private set; } = errors;
    }
}