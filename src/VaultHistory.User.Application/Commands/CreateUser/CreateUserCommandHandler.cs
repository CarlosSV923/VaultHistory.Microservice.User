using VaultHistory.User.Application.Abstractions;
using VaultHistory.User.Domain.Abstractions;
using VaultHistory.User.Domain.Users.Interfaces;

namespace VaultHistory.User.Application.Commands.CreateUser
{
    internal sealed class CreateUserCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork
        ) : ICommandHandler<CreateUserCommand>
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {

            _userRepository.Add(request.User);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();

        }
    }
}