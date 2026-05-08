using VaultHistory.User.Application.Abstractions;
using VaultHistory.User.Application.Commands.CreateUser;
using VaultHistory.User.Application.Commands.UpdateUser;
using VaultHistory.User.Application.Queries.GetUserByEmail;
using VaultHistory.User.Application.Queries.GetUserById;
using VaultHistory.User.Application.Queries.VerifyUserEmailExists;
using VaultHistory.User.Domain.Abstractions;
using VaultHistory.User.Domain.Users;
using VaultHistory.User.Domain.Users.Interfaces;
using VaultHistory.User.Domain.Users.ValueObjects;

namespace VaultHistory.User.Application.UnitTests.Handlers;

public sealed class ApplicationHandlersTests
{
    [Fact]
    public async Task CreateUserCommandHandler_ShouldAddUserAndSaveChanges()
    {
        var repository = new FakeUserRepository();
        var unitOfWork = new FakeUnitOfWork();
        var handler = CreateHandler<ICommandHandler<CreateUserCommand>>(
            "VaultHistory.User.Application.Commands.CreateUser.CreateUserCommandHandler",
            repository,
            unitOfWork);
        var user = CreateUser("john.create@example.com");

        var result = await handler.Handle(new CreateUserCommand(user), CancellationToken.None);

        Assert.True(result.IsSucceeded);
        Assert.Same(user, repository.AddedUser);
        Assert.Equal(1, unitOfWork.SaveChangesCalls);
    }

    [Fact]
    public async Task UpdateUserCommandHandler_ShouldUpdateUserAndSaveChanges()
    {
        var repository = new FakeUserRepository();
        var unitOfWork = new FakeUnitOfWork();
        var handler = CreateHandler<ICommandHandler<UpdateUserCommand>>(
            "VaultHistory.User.Application.Commands.UpdateUser.UpdateUserCommandHandler",
            repository,
            unitOfWork);
        var user = CreateUser("john.update@example.com");

        var result = await handler.Handle(new UpdateUserCommand(user), CancellationToken.None);

        Assert.True(result.IsSucceeded);
        Assert.Same(user, repository.UpdatedUser);
        Assert.Equal(1, unitOfWork.SaveChangesCalls);
    }

    [Fact]
    public async Task GetUserByIdQueryHandler_ShouldFail_WhenUserDoesNotExist()
    {
        var repository = new FakeUserRepository();
        var handler = CreateHandler<IQueryHandler<GetUserByIdQuery, Domain.Users.User>>(
            "VaultHistory.User.Application.Queries.GetUserById.GetUserByIdQueryHandler",
            repository);

        var result = await handler.Handle(new GetUserByIdQuery(UserId.NewId()), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.UserNotFound, result.Error);
    }

    [Fact]
    public async Task GetUserByIdQueryHandler_ShouldReturnUser_WhenUserExists()
    {
        var repository = new FakeUserRepository();
        var handler = CreateHandler<IQueryHandler<GetUserByIdQuery, Domain.Users.User>>(
            "VaultHistory.User.Application.Queries.GetUserById.GetUserByIdQueryHandler",
            repository);
        var user = CreateUser("john.getbyid@example.com");
        repository.Add(user);

        var result = await handler.Handle(new GetUserByIdQuery(user.Id), CancellationToken.None);

        Assert.True(result.IsSucceeded);
        Assert.Equal(user.Id, result.Value.Id);
    }

    [Fact]
    public async Task GetUserByEmailQueryHandler_ShouldFail_WhenUserDoesNotExist()
    {
        var repository = new FakeUserRepository();
        var handler = CreateHandler<IQueryHandler<GetUserByEmailQuery, Domain.Users.User>>(
            "VaultHistory.User.Application.Queries.GetUserByEmail.GetUserByEmailQueryHandler",
            repository);
        var email = Email.Create("missing@example.com").Value;

        var result = await handler.Handle(new GetUserByEmailQuery(email), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.UserNotFound, result.Error);
    }

    [Fact]
    public async Task GetUserByEmailQueryHandler_ShouldReturnUser_WhenUserExists()
    {
        var repository = new FakeUserRepository();
        var handler = CreateHandler<IQueryHandler<GetUserByEmailQuery, Domain.Users.User>>(
            "VaultHistory.User.Application.Queries.GetUserByEmail.GetUserByEmailQueryHandler",
            repository);
        var user = CreateUser("john.getbyemail@example.com");
        repository.Add(user);

        var result = await handler.Handle(new GetUserByEmailQuery(user.Email), CancellationToken.None);

        Assert.True(result.IsSucceeded);
        Assert.Equal(user.Id, result.Value.Id);
    }

    [Fact]
    public async Task VerifyUserEmailExistsQueryHandler_ShouldFail_WhenUserDoesNotExist()
    {
        var repository = new FakeUserRepository();
        var handler = CreateHandler<IQueryHandler<VerifyUserEmailExistsQuery>>(
            "VaultHistory.User.Application.Queries.VerifyUserEmailExists.VerifyUserEmailExistsQueryHandler",
            repository);
        var email = Email.Create("missing.verify@example.com").Value;

        var result = await handler.Handle(new VerifyUserEmailExistsQuery(email), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.UserNotFound, result.Error);
    }

    [Fact]
    public async Task VerifyUserEmailExistsQueryHandler_ShouldSucceed_WhenUserExists()
    {
        var repository = new FakeUserRepository();
        var handler = CreateHandler<IQueryHandler<VerifyUserEmailExistsQuery>>(
            "VaultHistory.User.Application.Queries.VerifyUserEmailExists.VerifyUserEmailExistsQueryHandler",
            repository);
        var user = CreateUser("exists.verify@example.com");
        repository.Add(user);

        var result = await handler.Handle(new VerifyUserEmailExistsQuery(user.Email), CancellationToken.None);

        Assert.True(result.IsSucceeded);
    }

    private static THandler CreateHandler<THandler>(string fullTypeName, params object[] constructorArgs)
    {
        var applicationAssembly = typeof(VaultHistory.User.Application.DependencyInjection).Assembly;
        var handlerType = applicationAssembly.GetType(fullTypeName, throwOnError: true)!;
        return (THandler)Activator.CreateInstance(handlerType, constructorArgs)!;
    }

    private static Domain.Users.User CreateUser(string email)
    {
        var data = new CreateUserData(
            FullName.Create("John", "Doe").Value,
            Email.Create(email).Value,
            Password.Create("hash", "salt").Value,
            new DateOnly(1990, 1, 1));

        return Domain.Users.User.Create(data).Value;
    }

    private sealed class FakeUnitOfWork : IUnitOfWork
    {
        public int SaveChangesCalls { get; private set; }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SaveChangesCalls++;
            return Task.FromResult(1);
        }
    }

    private sealed class FakeUserRepository : IUserRepository
    {
        private readonly List<Domain.Users.User> _users = [];

        public Domain.Users.User? AddedUser { get; private set; }
        public Domain.Users.User? UpdatedUser { get; private set; }

        public void Add(Domain.Users.User user)
        {
            AddedUser = user;
            _users.Add(user);
        }

        public void Update(Domain.Users.User user)
        {
            UpdatedUser = user;
            var index = _users.FindIndex(u => u.Id == user.Id);
            if (index >= 0)
            {
                _users[index] = user;
                return;
            }

            _users.Add(user);
        }

        public Task<Domain.Users.User?> GetByIdAsync(UserId id, CancellationToken cancellationToken = default)
        {
            var user = _users.FirstOrDefault(u => u.Id == id);
            return Task.FromResult(user);
        }

        public Task<Domain.Users.User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default)
        {
            var user = _users.FirstOrDefault(u => u.Email == email);
            return Task.FromResult(user);
        }

        public Task<bool> VerifyEmailExistsAsync(Email email, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_users.Any(u => u.Email == email));
        }
    }
}