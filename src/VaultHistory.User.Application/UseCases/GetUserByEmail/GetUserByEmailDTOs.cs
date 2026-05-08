using VaultHistory.User.Application.Abstractions;

namespace VaultHistory.User.Application.UseCases.GetUserByEmail
{
    public record GetUserByEmailRequestDto(
        string Email
    ) : IUseCase<GetUserByEmailResponseDto>;

    public record GetUserByEmailResponseDto(
        string UserId,
        string FirstName,
        string LastName,
        string Email,
        DateOnly? BirthDate,
        bool IsActive
    );
}