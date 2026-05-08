using VaultHistory.User.Application.Abstractions;
using VaultHistory.User.Domain.Abstractions;
using VaultHistory.User.Domain.Users.Interfaces;

namespace VaultHistory.User.Application.Commands.UpdateUser
{
    internal sealed class UpdateUserCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork
        ) : ICommandHandler<UpdateUserCommand>
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        public async Task<Result> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {

            _userRepository.Update(request.User);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();

        }
    }
}