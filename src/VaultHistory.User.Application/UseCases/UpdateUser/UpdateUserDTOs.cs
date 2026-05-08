using VaultHistory.User.Application.Abstractions;

namespace VaultHistory.User.Application.UseCases.UpdateUser
{
    public record UpdateUserRequestDto(
        string UserId,
        string? FirstName,
        string? LastName,
        DateOnly? DateOfBirth
    ) : IUseCase<UpdateUserResponseDto>;

    public record UpdateUserResponseDto(
        string UserId
    );
}