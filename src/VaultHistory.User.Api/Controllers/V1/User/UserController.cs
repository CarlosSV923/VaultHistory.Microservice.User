using MediatR;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using VaultHistory.User.Api.Utils;
using VaultHistory.User.Domain.Abstractions;
using Microsoft.AspNetCore.Authorization;


namespace VaultHistory.User.Api.Controllers.V1.User
{
    
    [ApiController]
    [ApiVersion(ApiVersions.V1)]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class UserController (
        IMediator mediator
    ) : ControllerBase
    {
        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(DeactivateResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeactivateUser(string id, CancellationToken cancellationToken)
        {
            var request = new DeactivateRequest(id);
            var useCaseInput = UserMappers.Map(request);
            var response = await mediator.Send(useCaseInput, cancellationToken);

            if (response.IsFailure)
            {
                return BadRequest(response.Error);
            }

            return Ok(UserMappers.Map(response.Value));
        }

        [HttpPost("{id}/change-password")]
        [Authorize]
        [ProducesResponseType(typeof(ChangePasswordResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ChangePassword(string id, [FromBody] ChangePasswordRequest request, CancellationToken cancellationToken)
        {
            var useCaseInput = UserMappers.Map(id, request);
            var response = await mediator.Send(useCaseInput, cancellationToken);

            if (response.IsFailure)
            {
                return BadRequest(response.Error);
            }

            return Ok(UserMappers.Map(response.Value));
        }

        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(GetByIdResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(string id, CancellationToken cancellationToken)
        {
            var request = new GetByIdRequest(id);
            var useCaseInput = UserMappers.Map(request);
            var response = await mediator.Send(useCaseInput, cancellationToken);

            if (response.IsFailure)
            {
                return BadRequest(response.Error);
            }

            return Ok(UserMappers.Map(response.Value));
        }

        [HttpGet("by-email")]
        [Authorize]
        [ProducesResponseType(typeof(GetByEmailResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByEmail([FromQuery] string email, CancellationToken cancellationToken)
        {
            var request = new GetByEmailRequest(email);
            var useCaseInput = UserMappers.Map(request);
            var response = await mediator.Send(useCaseInput, cancellationToken);

            if (response.IsFailure)
            {
                return BadRequest(response.Error);
            }

            return Ok(UserMappers.Map(response.Value));
        }

        [HttpPost("signin")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(SigninResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Signin([FromBody] SigninRequest request,   
            CancellationToken cancellationToken)
        {
            var useCaseInput = UserMappers.Map(request);
            var response = await mediator.Send(useCaseInput, cancellationToken);

            if (response.IsFailure)
            {
                return BadRequest(response.Error);
            }

            return Ok(UserMappers.Map(response.Value));
        }

        [HttpPost("signup")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(SignupResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Signup([FromBody] SignupRequest request, CancellationToken cancellationToken)
        {
            var useCaseInput = UserMappers.Map(request);
            var response = await mediator.Send(useCaseInput, cancellationToken);

            if (response.IsFailure)
            {
                return BadRequest(response.Error);
            }

            return Ok(UserMappers.Map(response.Value));
        }

        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(UpdateResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateRequest request,
            CancellationToken cancellationToken)
        {
            var useCaseInput = UserMappers.Map(id, request);
            var response = await mediator.Send(useCaseInput, cancellationToken);

            if (response.IsFailure)
            {
                return BadRequest(response.Error);
            }

            return Ok(UserMappers.Map(response.Value));
        }
    }
}