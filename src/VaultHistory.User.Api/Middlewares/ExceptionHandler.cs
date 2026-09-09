using System.Net;
using VaultHistory.User.Application.Exceptions;
using VaultHistory.User.Api.Utils;

namespace VaultHistory.User.Api.Middlewares
{
    public class ExceptionHandler(
        ILogger<ExceptionHandler> logger,
        RequestDelegate next)
    {

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while processing the request => {Message}", ex.Message);

                var exceptionDetails = GetExceptionDetails(ex);

                context.Response.StatusCode = exceptionDetails.StatusCode;
                await context.Response.WriteAsJsonAsync(new ApiErrorResponse(
                    exceptionDetails.Code,
                    exceptionDetails.Message,
                    exceptionDetails.Errors));

            }
        }

        private static ExceptionDetails GetExceptionDetails(Exception ex)
        {
            return ex switch
            {
                ValidationException validationException => new ExceptionDetails(
                    (int)HttpStatusCode.BadRequest,
                    "Error.ValidationError",
                    "One or more validation errors occurred.",
                    validationException.Errors
                ),
                _ => new ExceptionDetails(
                    (int)HttpStatusCode.InternalServerError,
                    "Error.InternalServerError",
                    "An unexpected error occurred. Please try again later.",
                    null
                )
            };
        }
    }

    internal record ExceptionDetails(
        int StatusCode,
        string Code,
        string Message,
        IEnumerable<object>? Errors = null
    );
}
