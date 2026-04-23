using VaultHistory.User.Domain.Abstractions;

namespace VaultHistory.User.Domain.Users
{
    public static class UserErrors
    {
        public static readonly Error DataRequired = Error.BuildInternalError("User.DataRequired", "User data is required.");
        public static readonly Error FullNameRequired = Error.BuildInternalError("User.FullNameRequired", "User full name is required.");
        public static readonly Error EmailRequired = Error.BuildInternalError("User.EmailRequired", "Email is required.");
        public static readonly Error InvalidEmailFormat = Error.BuildInternalError("User.InvalidEmailFormat", "Email format is invalid.");
        public static readonly Error PasswordRequired = Error.BuildInternalError("User.PasswordRequired", "Password is required.");
        public static readonly Error PasswordHashRequired = Error.BuildInternalError("User.PasswordHashRequired", "Password hash is required.");
        public static readonly Error PasswordSaltRequired = Error.BuildInternalError("User.PasswordSaltRequired", "Password salt is required.");
        public static readonly Error FirstNameRequired = Error.BuildInternalError("User.FirstNameRequired", "First name is required.");
        public static readonly Error LastNameRequired = Error.BuildInternalError("User.LastNameRequired", "Last name is required.");
        public static readonly Error FirstNameTooLong = Error.BuildInternalError("User.FirstNameTooLong", "First name exceeds the maximum allowed length.");
        public static readonly Error LastNameTooLong = Error.BuildInternalError("User.LastNameTooLong", "Last name exceeds the maximum allowed length.");
        public static readonly Error BirthDateCannotBeInFuture = Error.BuildInternalError("User.BirthDateCannotBeInFuture", "Birth date cannot be in the future.");

        public static readonly Error UserNotFound = Error.BuildInternalError("User.NotFound", "User not found.");
    }
}
