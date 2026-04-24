using VaultHistory.User.Domain.Abstractions;

namespace VaultHistory.User.Domain.Users
{
    public static class UserErrors
    {
        public static readonly Error DataRequired = new("User.DataRequired", "User data is required.");
        public static readonly Error FullNameRequired = new("User.FullNameRequired", "User full name is required.");
        public static readonly Error EmailRequired = new("User.EmailRequired", "Email is required.");
        public static readonly Error InvalidEmailFormat = new("User.InvalidEmailFormat", "Email format is invalid.");
        public static readonly Error PasswordRequired = new("User.PasswordRequired", "Password is required.");
        public static readonly Error PasswordHashRequired = new("User.PasswordHashRequired", "Password hash is required.");
        public static readonly Error PasswordSaltRequired = new("User.PasswordSaltRequired", "Password salt is required.");
        public static readonly Error FirstNameRequired = new("User.FirstNameRequired", "First name is required.");
        public static readonly Error LastNameRequired = new("User.LastNameRequired", "Last name is required.");
        public static readonly Error FirstNameTooLong = new("User.FirstNameTooLong", "First name exceeds the maximum allowed length.");
        public static readonly Error LastNameTooLong = new("User.LastNameTooLong", "Last name exceeds the maximum allowed length.");
        public static readonly Error BirthDateCannotBeInFuture = new("User.BirthDateCannotBeInFuture", "Birth date cannot be in the future.");
        public static readonly Error UserNotFound = new("User.NotFound", "User not found.");
        public static readonly Error PasswordTooShort = new("User.PasswordTooShort", "Password must be at least 8 characters long.");
        public static readonly Error PasswordInvalidFormat = new("User.PasswordInvalidFormat", "Password does not match the required format.");
        public static readonly Error UserAlreadyExists = new("User.UserAlreadyExists", "A user with the same email already exists.");
        public static readonly Error PasswordHashingFailed = new("User.PasswordHashingFailed", "Failed to hash the password.");
        public static readonly Error InvalidUserId = new("User.InvalidUserId", "The provided user ID is invalid.");
        public static readonly Error InvalidPassword = new("User.InvalidPassword", "The provided password is incorrect.");
        public static readonly Error UserInactive = new("User.Inactive", "User is not active.");
    }
}
