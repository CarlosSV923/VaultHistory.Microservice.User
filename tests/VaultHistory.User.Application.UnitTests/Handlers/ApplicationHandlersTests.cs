using System.Runtime.CompilerServices;
using System.Security.Claims;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using VaultHistory.User.Application.Abstractions;
using VaultHistory.User.Application.Commands.CreateUser;
using VaultHistory.User.Application.Commands.UpdateUser;
using VaultHistory.User.Application.Providers.Jwt;
using VaultHistory.User.Application.Providers.PasswordHasher;
using VaultHistory.User.Application.Queries.GetUserByEmail;
using VaultHistory.User.Application.Queries.GetUserById;
using VaultHistory.User.Application.Queries.VerifyUserEmailExists;
using VaultHistory.User.Application.UseCases.SigninUser;
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

    [Fact]
    public async Task SigninUserUseCaseHandler_ShouldPersistSignInEventBeforeReturningToken()
    {
        var user = CreateUser("john.signin@example.com");
        user.ClearDomainEvents();
        var unitOfWork = new FakeUnitOfWork();
        var jwtProvider = new FakeJwtProvider();
        var handler = CreateSigninHandler(
            new FakeMediator((request, _) =>
            {
                Assert.IsType<GetUserByEmailQuery>(request);
                return Result.Success(user);
            }),
            new FakePasswordHasherProvider(Result.Success()),
            jwtProvider,
            unitOfWork);

        var result = await handler.Handle(new SigninUserRequestDto(user.Email.Value, "Passw0rd!"), CancellationToken.None);

        Assert.True(result.IsSucceeded);
        Assert.Equal(1, unitOfWork.SaveChangesCalls);
        Assert.Equal(1, jwtProvider.GenerateTokenCalls);
        Assert.IsType<Domain.Users.Events.UserSignedInEvent>(Assert.Single(user.GetDomainEvents()));
    }

    [Fact]
    public async Task SigninUserUseCaseHandler_ShouldNotReturnToken_WhenSignInEventCannotBePersisted()
    {
        var user = CreateUser("john.signin.failure@example.com");
        user.ClearDomainEvents();
        var jwtProvider = new FakeJwtProvider();
        var handler = CreateSigninHandler(
            new FakeMediator((_, _) => Result.Success(user)),
            new FakePasswordHasherProvider(Result.Success()),
            jwtProvider,
            new FakeUnitOfWork(new InvalidOperationException("Database unavailable")));

        var result = await handler.Handle(new SigninUserRequestDto(user.Email.Value, "Passw0rd!"), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(UserErrors.SigninPersistenceFailed, result.Error);
        Assert.Equal(0, jwtProvider.GenerateTokenCalls);
    }

    private static THandler CreateHandler<THandler>(string fullTypeName, params object[] constructorArgs)
    {
        var applicationAssembly = typeof(VaultHistory.User.Application.DependencyInjection).Assembly;
        var handlerType = applicationAssembly.GetType(fullTypeName, throwOnError: true)!;
        return (THandler)Activator.CreateInstance(handlerType, constructorArgs)!;
    }

    private static IUseCaseHandler<SigninUserRequestDto, SigninUserResponseDto> CreateSigninHandler(
        IMediator mediator,
        IPasswordHasherProvider passwordHasherProvider,
        IJwtProvider jwtProvider,
        IUnitOfWork unitOfWork)
    {
        var applicationAssembly = typeof(VaultHistory.User.Application.DependencyInjection).Assembly;
        var handlerType = applicationAssembly.GetType("VaultHistory.User.Application.UseCases.SigninUser.SigninUserUseCaseHandler", throwOnError: true)!;
        var nullLoggerType = typeof(NullLogger<>).MakeGenericType(handlerType);
        var logger = nullLoggerType.GetField("Instance")!.GetValue(null)!;

        return (IUseCaseHandler<SigninUserRequestDto, SigninUserResponseDto>)Activator.CreateInstance(
            handlerType,
            mediator,
            passwordHasherProvider,
            jwtProvider,
            unitOfWork,
            logger)!;
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

    private sealed class FakeUnitOfWork(Exception? exception = null) : IUnitOfWork
    {
        public int SaveChangesCalls { get; private set; }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SaveChangesCalls++;
            if (exception is not null)
            {
                throw exception;
            }

            return Task.FromResult(1);
        }
    }

    private sealed class FakeMediator(Func<object, CancellationToken, object?> sendHandler) : IMediator
    {
        public Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequest
        {
            sendHandler(request!, cancellationToken);
            return Task.CompletedTask;
        }

        public Task<object?> Send(object request, CancellationToken cancellationToken = default) => Task.FromResult(sendHandler(request, cancellationToken));

        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default) =>
            Task.FromResult((TResponse)sendHandler(request, cancellationToken)!);

        public Task Publish(object notification, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : INotification => Task.CompletedTask;

        public async IAsyncEnumerable<TResponse> CreateStream<TResponse>(IStreamRequest<TResponse> request, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            yield break;
        }

        public async IAsyncEnumerable<object?> CreateStream(object request, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            yield break;
        }
    }

    private sealed class FakePasswordHasherProvider(Result verificationResult) : IPasswordHasherProvider
    {
        public PasswordHasherResult HashPassword(string password) => throw new NotSupportedException();

        public Result VerifyPassword(string password, string hash, string salt) => verificationResult;

        public Result ValidatePassword(string password) => throw new NotSupportedException();
    }

    private sealed class FakeJwtProvider : IJwtProvider
    {
        public int GenerateTokenCalls { get; private set; }

        public JwtGenerateTokenResult GenerateToken(Domain.Users.User user)
        {
            GenerateTokenCalls++;
            return new JwtGenerateTokenResult("token", DateTime.UtcNow.AddHours(1));
        }

        public Result<ClaimsPrincipal> ValidateToken(string token) => throw new NotSupportedException();
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
