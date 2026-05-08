using VaultHistory.User.Domain.Abstractions;
using VaultHistory.User.Domain.Users.Events;
using VaultHistory.User.Domain.Users.ValueObjects;

namespace VaultHistory.User.Domain.Users
{
    public sealed class User : Entity<UserId>
    {
        private User() { }
        private User(UserId id, FullName fullName, Email email, Password password, DateOnly? birthDate = null)
         : base(id)
        {
            FullName = fullName;
            Email = email;
            Password = password;
            BirthDate = birthDate;
        }

        public FullName FullName { get; private set; } = null!;
        public Email Email { get; private set; } = null!;
        public Password Password { get; private set; } = null!;

        public DateOnly? BirthDate { get; private set; }

        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; private set; }

        public bool IsActive { get; private set; } = true;

        public static Result<User> Create(CreateUserData data)
        {
            if (data is null)
            {
                return Result.Failure<User>(UserErrors.DataRequired);
            }

            if (data.FullName is null)
            {
                return Result.Failure<User>(UserErrors.FullNameRequired);
            }

            if (data.Email is null)
            {
                return Result.Failure<User>(UserErrors.EmailRequired);
            }

            if (data.Password is null)
            {
                return Result.Failure<User>(UserErrors.PasswordRequired);
            }

            if (data.BirthDate is not null && data.BirthDate > DateOnly.FromDateTime(DateTime.UtcNow))
            {
                return Result.Failure<User>(UserErrors.BirthDateCannotBeInFuture);
            }

            var user = new User(UserId.NewId(), data.FullName, data.Email, data.Password, data.BirthDate);
            user.AddDomainEvent(new CreateUserEvent(user.Id));
            return Result.Success(user);
        }

        public Result Update(UpdateUserData data)
        {
            if (data is null)
            {
                return Result.Failure(UserErrors.DataRequired);
            }

            if (data.FullName is not null)
            {
                var nameResult = ChangeFullName(data.FullName);
                if (nameResult.IsFailure)
                {
                    return nameResult;
                }
            }

            if (data.UpdateBirthDate)
            {
                var birthDateResult = SetBirthDate(data.BirthDate);
                if (birthDateResult.IsFailure)
                {
                    return birthDateResult;
                }
            }

            return Result.Success();
        }

        public Result ChangeFullName(FullName fullName)
        {
            if (fullName is null)
            {
                return Result.Failure(UserErrors.FullNameRequired);
            }

            if (fullName == FullName)
            {
                return Result.Success();
            }

            FullName = fullName;
            Touch();
            AddDomainEvent(new UserFullNameChangedEvent(Id));
            return Result.Success();
        }

        public Result ChangeEmail(Email email)
        {
            if (email is null)
            {
                return Result.Failure(UserErrors.EmailRequired);
            }

            if (email == Email)
            {
                return Result.Success();
            }

            Email = email;
            Touch();
            AddDomainEvent(new UserEmailChangedEvent(Id));
            return Result.Success();
        }

        public Result ChangePassword(Password password)
        {
            if (password is null)
            {
                return Result.Failure(UserErrors.PasswordRequired);
            }

            if (password == Password)
            {
                return Result.Success();
            }

            Password = password;
            Touch();
            AddDomainEvent(new UserPasswordChangedEvent(Id));
            return Result.Success();
        }

        public Result SetBirthDate(DateOnly? birthDate)
        {
            if (birthDate is not null && birthDate > DateOnly.FromDateTime(DateTime.UtcNow))
            {
                return Result.Failure(UserErrors.BirthDateCannotBeInFuture);
            }

            if (BirthDate == birthDate)
            {
                return Result.Success();
            }

            BirthDate = birthDate;
            Touch();
            AddDomainEvent(new UserBirthDateChangedEvent(Id));
            return Result.Success();
        }

        public void Activate()
        {
            if (IsActive)
            {
                return;
            }

            IsActive = true;
            Touch();
            AddDomainEvent(new UserActivatedEvent(Id));
        }

        public void Deactivate()
        {
            if (!IsActive)
            {
                return;
            }

            IsActive = false;
            Touch();
            AddDomainEvent(new UserDeactivatedEvent(Id));
        }

        private void Touch() => UpdatedAt = DateTime.UtcNow;

    }
}