using System.Net.Mail;
using VaultHistory.User.Domain.Abstractions;

namespace VaultHistory.User.Domain.Users
{
    public sealed record Email
    {
        private Email(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public static Result<Email> Create(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return Result.Failure<Email>(UserErrors.EmailRequired);
            }

            var normalizedEmail = value.Trim().ToLowerInvariant();

            try
            {
                _ = new MailAddress(normalizedEmail);
            }
            catch (FormatException)
            {
                return Result.Failure<Email>(UserErrors.InvalidEmailFormat);
            }

            return Result.Success(new Email(normalizedEmail));
        }

        public override string ToString() => Value;
    }
}
