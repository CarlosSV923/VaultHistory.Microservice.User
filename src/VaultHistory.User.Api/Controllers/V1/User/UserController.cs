using MediatR;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using VaultHistory.User.Api.Utils;
using VaultHistory.User.Domain.Abstractions;
using Microsoft.AspNetCore.Authorization;
using VaultHistory.User.Api.Security;
using VaultHistory.User.Application.Providers.UserContext;


namespace VaultHistory.User.Api.Controllers.V1.User
{
    
    [ApiController]
    [ApiVersion(ApiVersions.V1)]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class UserController (
        IMediator mediator,
        IUserContextProvider userContextProvider
    ) : ControllerBase
    {
        private IActionResult Failure(Error error)
        {
            var statusCode = UserErrorStatusMapper.ToStatusCode(error);
            return statusCode == StatusCodes.Status400BadRequest
                ? BadRequest(error)
                : StatusCode(statusCode, error);
        }

        [HttpDelete]
        [JwtAuthorize]
        [ProducesResponseType(typeof(DeactivateResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeactivateUser(CancellationToken cancellationToken)
        {
            var id = userContextProvider.GetUserId();
            var request = new DeactivateRequest(id);
            var useCaseInput = UserMappers.Map(request);
            var response = await mediator.Send(useCaseInput, cancellationToken);

            if (response.IsFailure)
            {
                return Failure(response.Error);
            }

            return Ok(UserMappers.Map(response.Value));
        }

        [HttpPost("change-password")]
        [JwtAuthorize]
        [ProducesResponseType(typeof(ChangePasswordResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request, CancellationToken cancellationToken)
        {
            var id = userContextProvider.GetUserId();
            var useCaseInput = UserMappers.Map(id, request);
            var response = await mediator.Send(useCaseInput, cancellationToken);

            if (response.IsFailure)
            {
                return Failure(response.Error);
            }

            return Ok(UserMappers.Map(response.Value));
        }

        [HttpGet]
        [JwtAuthorize]
        [ProducesResponseType(typeof(GetByIdResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(CancellationToken cancellationToken)
        {
            var id = userContextProvider.GetUserId();
            var request = new GetByIdRequest(id);
            var useCaseInput = UserMappers.Map(request);
            var response = await mediator.Send(useCaseInput, cancellationToken);

            if (response.IsFailure)
            {
                return Failure(response.Error);
            }

            return Ok(UserMappers.Map(response.Value));
        }

        [HttpGet("by-email")]
        [JwtAuthorize]
        [ProducesResponseType(typeof(GetByEmailResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByEmail([FromQuery] string email, CancellationToken cancellationToken)
        {
            var id = userContextProvider.GetUserId();
            var request = new GetByEmailRequest(email);
            var useCaseInput = UserMappers.Map(id, request);
            var response = await mediator.Send(useCaseInput, cancellationToken);

            if (response.IsFailure)
            {
                return Failure(response.Error);
            }

            return Ok(UserMappers.Map(response.Value));
        }

        [HttpPost("signin")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(SigninResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Signin([FromBody] SigninRequest request,   
            CancellationToken cancellationToken)
        {
            var useCaseInput = UserMappers.Map(request);
            var response = await mediator.Send(useCaseInput, cancellationToken);

            if (response.IsFailure)
            {
                return Failure(response.Error);
            }

            return Ok(UserMappers.Map(response.Value));
        }

        [HttpPost("signup")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(SignupResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Signup([FromBody] SignupRequest request, CancellationToken cancellationToken)
        {
            var useCaseInput = UserMappers.Map(request);
            var response = await mediator.Send(useCaseInput, cancellationToken);

            if (response.IsFailure)
            {
                return Failure(response.Error);
            }

            return Ok(UserMappers.Map(response.Value));
        }

        [HttpPut]
        [JwtAuthorize]
        [ProducesResponseType(typeof(UpdateResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update([FromBody] UpdateRequest request,
            CancellationToken cancellationToken)
        {
            var id = userContextProvider.GetUserId();
            var useCaseInput = UserMappers.Map(id, request);
            var response = await mediator.Send(useCaseInput, cancellationToken);

            if (response.IsFailure)
            {
                return Failure(response.Error);
            }

            return Ok(UserMappers.Map(response.Value));
        }
    }
}
