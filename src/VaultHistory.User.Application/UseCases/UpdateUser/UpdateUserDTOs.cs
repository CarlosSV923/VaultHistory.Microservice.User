using VaultHistory.User.Application.Abstractions;

namespace VaultHistory.User.Application.UseCases.UpdateUser
{
    public record UpdateUserRequestDto(
        string UserId,
        string? FirstName,
        string? LastName,
        DateOnly? DateOfBirth,
        bool? Notification = null,
        string? Theme = null,
        string? Character = null
    ) : IUseCase<UpdateUserResponseDto>;

    public record UpdateUserResponseDto(
        string UserId
    );
}
