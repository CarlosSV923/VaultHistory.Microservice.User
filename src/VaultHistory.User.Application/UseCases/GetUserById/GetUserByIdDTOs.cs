using VaultHistory.User.Application.Abstractions;

namespace VaultHistory.User.Application.UseCases.GetUserById
{
    public record GetUserByIdRequestDto(
        string UserId
    ) : IUseCase<GetUserByIdResponseDto>;

    public record GetUserByIdResponseDto(
        string UserId,
        string FirstName,
        string LastName,
        string Email,
        DateOnly? BirthDate,
        bool IsActive,
        bool Notification = false,
        string? Theme = null,
        string? Character = null
    );   
}
