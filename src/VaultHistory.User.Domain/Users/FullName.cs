using VaultHistory.User.Domain.Abstractions;

namespace VaultHistory.User.Domain.Users
{
    public sealed record FullName
    {
        private const int MaxLength = 50;

        private FullName(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }

        public string FirstName { get; }
        public string LastName { get; }

        public static Result<FullName> Create(string? firstName, string? lastName)
        {
            if (string.IsNullOrWhiteSpace(firstName))
            {
                return Result.Failure<FullName>(UserErrors.FirstNameRequired);
            }

            if (string.IsNullOrWhiteSpace(lastName))
            {
                return Result.Failure<FullName>(UserErrors.LastNameRequired);
            }

            var normalizedFirstName = firstName.Trim();
            var normalizedLastName = lastName.Trim();

            if (normalizedFirstName.Length > MaxLength)
            {
                return Result.Failure<FullName>(UserErrors.FirstNameTooLong);
            }

            if (normalizedLastName.Length > MaxLength)
            {
                return Result.Failure<FullName>(UserErrors.LastNameTooLong);
            }

            return Result.Success(new FullName(normalizedFirstName, normalizedLastName));
        }

        /// <summary>
        /// Rehydrates a name stored by the shared persistence schema.  That schema
        /// keeps a name in one <c>fullname</c> column, while the domain keeps its
        /// first and last-name invariants explicit.
        /// </summary>
        public static FullName FromPersistedValue(string value)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);

            var parts = value.Trim().Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
            return parts.Length == 2
                ? new FullName(parts[0], parts[1])
                : new FullName(parts[0], string.Empty);
        }

        public string GetFullName() => $"{FirstName} {LastName}";
        public override string ToString() => GetFullName();
    }
}
