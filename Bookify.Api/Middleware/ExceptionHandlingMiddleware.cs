using Bookify.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Bookify.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    #region Fields

    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly RequestDelegate _next;

    #endregion

    #region Construction

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    #endregion

    #region Private Methods

    private static ExceptionDetails GetExceptionDetails(Exception exception)
    {
        return exception switch
               {
                   ValidationException validationException => new ExceptionDetails(
                                                                                   StatusCodes.Status400BadRequest,
                                                                                   "ValidationFailure",
                                                                                   "Validation error",
                                                                                   "One or more validation errors has occured",
                                                                                   validationException.Errors
                                                                                  ),
                   _ => new ExceptionDetails(
                                             StatusCodes.Status500InternalServerError,
                                             "ServerError",
                                             "Server error",
                                             "AN unexpected error has occured",
                                             null)
               };
    }

    #endregion

    #region Public Methods

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch(Exception exception)
        {
            _logger.LogError(exception, "Exception occured: {Message}", exception.Message);

            var exceptionDetails = GetExceptionDetails(exception);

            var problemDetails = new ProblemDetails
                                 {
                                     Status = exceptionDetails.Status,
                                     Type = exceptionDetails.Type,
                                     Title = exceptionDetails.Title,
                                     Detail = exceptionDetails.Detail
                                 };

            if (exceptionDetails.Errors != null)
            {
                problemDetails.Extensions["errors"] = exceptionDetails.Errors;
            }

            context.Response.StatusCode = exceptionDetails.Status;

            await context.Response.WriteAsJsonAsync(problemDetails);
        }
    }

    #endregion

    private record ExceptionDetails(int Status, string Type, string Title, string Detail, IEnumerable<object>? Errors);
}
