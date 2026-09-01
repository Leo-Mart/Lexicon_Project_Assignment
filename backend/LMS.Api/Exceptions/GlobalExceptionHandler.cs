using LMS.Api.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Api.Exceptions;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        int statusCode;
        string title;

        switch (exception)
        {
            case InvalidDateException invalidDateException:
                statusCode = invalidDateException.StatusCode;
                title = "Invalid date";
                break;

            case OverlappingDateException overlappingDateException:
                statusCode = overlappingDateException.StatusCode;
                title = "Overlapping dates";
                break;

            case KeyNotFoundException:
                statusCode = StatusCodes.Status404NotFound;
                title = "Resource not found";
                break;

            default:
                statusCode = StatusCodes.Status500InternalServerError;
                title = "Internal server error";

                _logger.LogError(
                    exception,
                    "An unhandled exception occurred."
                );
                break;
        }

        ProblemDetails problemDetails = new()
        {
            Status = statusCode,
            Title = title,
            Detail = exception.Message,
            Instance = httpContext.Request.Path
        };

        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsJsonAsync(
            problemDetails,
            cancellationToken
        );

        return true;
    }
}
