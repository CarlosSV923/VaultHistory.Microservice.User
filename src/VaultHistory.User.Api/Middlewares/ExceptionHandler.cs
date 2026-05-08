using System.Net;
using Microsoft.AspNetCore.Mvc;
using VaultHistory.User.Application.Exceptions;

namespace VaultHistory.User.Api.Middlewares
{
    public class ExceptionHandler(RequestDelegate next)
    {

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                var exceptionDetails = GetExceptionDetails(ex);

                var problemDetails = new ProblemDetails
                {
                    Status = exceptionDetails.StatusCode,
                    Type = exceptionDetails.Type,
                    Title = exceptionDetails.Title,
                    Detail = exceptionDetails.Details,
                    Instance = context.Request.Path
                };

                if (exceptionDetails.Errors != null)
                {
                    problemDetails.Extensions.Add("errors", exceptionDetails.Errors);
                }

                context.Response.StatusCode = problemDetails.Status.Value;

                await context.Response.WriteAsJsonAsync(problemDetails);

            }
        }

        private static ExceptionDetails GetExceptionDetails(Exception ex)
        {
            return ex switch
            {
                ValidationException validationException => new ExceptionDetails(
                    (int)HttpStatusCode.BadRequest,
                    "ValidationFailure",
                    "Validation error",
                    "Han ocurrido uno o mas errores",
                    validationException.Errors
                ),
                _ => new ExceptionDetails(
                    (int)HttpStatusCode.InternalServerError,
                    "ServerError",
                    "An error occurred while processing your request.",
                    ex.Message,
                    null
                )
            };
        }
    }

    internal record ExceptionDetails(
        int StatusCode,
        string Type,
        string Title,
        string Details,
        IEnumerable<object>? Errors = null
    );
}