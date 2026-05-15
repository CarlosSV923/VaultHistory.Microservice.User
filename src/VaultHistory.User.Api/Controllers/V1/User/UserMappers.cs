using VaultHistory.User.Application.UseCases.ChangePasswordUser;
using VaultHistory.User.Application.UseCases.DeactivateUser;
using VaultHistory.User.Application.UseCases.GetUserByEmail;
using VaultHistory.User.Application.UseCases.GetUserById;
using VaultHistory.User.Application.UseCases.SigninUser;
using VaultHistory.User.Application.UseCases.SignupUser;
using VaultHistory.User.Application.UseCases.UpdateUser;

namespace VaultHistory.User.Api.Controllers.V1.User
{
    public static class UserMappers
    {

        // Mappers Request -> UseCase RequestDto
        public static GetUserByIdRequestDto Map(GetByIdRequest request) =>
            new(request.Id);

        public static GetUserByEmailRequestDto Map(string requestingUserId, GetByEmailRequest request) =>
            new(request.Email, requestingUserId);

        public static SigninUserRequestDto Map(SigninRequest request) =>
            new(request.Email, request.Password);

        public static SignupUserRequestDto Map(SignupRequest request) =>
            new(request.Email, request.Password, request.FirstName, request.LastName, request.BirthDate);

        public static UpdateUserRequestDto Map(string id, UpdateRequest request) =>
            new(id, request.FirstName, request.LastName, request.BirthDate);

        public static DeactivateUserRequestDto Map(DeactivateRequest request) =>
            new(request.Id);

        public static ChangePasswordUserRequestDto Map(string id, ChangePasswordRequest request) =>
            new(id, request.CurrentPassword, request.NewPassword);


        // Mappers UseCase ResponseDto -> Response
        public static GetByIdResponse Map(GetUserByIdResponseDto response) =>
            new(response.UserId, response.FirstName, response.LastName, response.Email, response.BirthDate ?? default, response.IsActive);
        
        public static GetByEmailResponse Map(GetUserByEmailResponseDto response) =>
            new(response.UserId, response.FirstName, response.LastName, response.Email, response.BirthDate ?? default, response.IsActive);

        public static SigninResponse Map(SigninUserResponseDto response) =>
            new(response.Token, response.Expiration);

        public static SignupResponse Map(SignupUserResponseDto response) =>
            new(response.Token, response.Expiration);
        
        public static UpdateResponse Map(UpdateUserResponseDto response) =>
            new(response.UserId);

        public static DeactivateResponse Map(DeactivateUserResponseDto response) =>
            new(response.UserId);
        
        public static ChangePasswordResponse Map(ChangePasswordUserResponseDto response) =>
            new(response.UserId);
        
    }
}