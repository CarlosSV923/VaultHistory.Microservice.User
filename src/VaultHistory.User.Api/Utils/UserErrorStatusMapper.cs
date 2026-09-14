using Microsoft.AspNetCore.Http;
using VaultHistory.User.Domain.Abstractions;

namespace VaultHistory.User.Api.Utils;

public static class UserErrorStatusMapper
{
    public static int ToStatusCode(Error error) => error.Code switch
    {
        "User.NotFound" => StatusCodes.Status404NotFound,
        "User.UserAlreadyExists" => StatusCodes.Status409Conflict,
        "User.InvalidUserId" or "User.Inactive" => StatusCodes.Status403Forbidden,
        "User.SigninPersistenceFailed" or "User.PasswordHashingFailed" =>
            StatusCodes.Status500InternalServerError,
        _ => StatusCodes.Status400BadRequest,
    };
}
