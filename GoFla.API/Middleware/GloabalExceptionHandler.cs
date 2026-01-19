using GoFla.API.Commons;
using GoFla.API.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace GoFla.API.Middleware;

public class GloabalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GloabalExceptionHandler> _logger;
    private readonly IHostEnvironment _env;

    public GloabalExceptionHandler(ILogger<GloabalExceptionHandler> logger, IHostEnvironment env)
    {
        _logger = logger;
        _env = env;
    }
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "An error occurred: {Message}", exception.Message);
        
        var (statusCode, errorDto) = exception switch
        {
            NotFoundException notFound => (StatusCodes.Status404NotFound, new ApiErrorResponse(
                notFound.Message,
                notFound.ErrorCode,
                null
            )),
            ValidationException validationEx => (StatusCodes.Status400BadRequest, new ApiErrorResponse(
                validationEx.Message,
                validationEx.ErrorCode,
                validationEx.ValidationErrors
            )),
            UnauthorizedException unauthorizedEx => (StatusCodes.Status401Unauthorized, new ApiErrorResponse(
                unauthorizedEx.Message,
                unauthorizedEx.ErrorCode,
                null
            )),
            ForbiddenException forbiddenEx => (StatusCodes.Status403Forbidden, new ApiErrorResponse(
                forbiddenEx.Message,
                forbiddenEx.ErrorCode,
                null
            )),
           _ => (StatusCodes.Status500InternalServerError, new ApiErrorResponse(
                "An unexpected error occurred.",
                "INTERNAL_SERVER_ERROR",
                null
            )),
        };

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/json";
        await httpContext.Response.WriteAsJsonAsync(errorDto, cancellationToken);
        
        return true;

    }
}
