namespace VaultHistory.User.Application.Exceptions
{
    public sealed class ConsurrencyException(string message, Exception innerException) : Exception(message, innerException)
    {
    }
}