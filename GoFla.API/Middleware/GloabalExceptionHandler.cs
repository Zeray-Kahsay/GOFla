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
            NotFoundException notFound => (StatusCodes.Status404NotFound, new ApiErrorDto
            {
                Message = notFound.Message,
                ErrorCode = notFound.ErrorCode
            }),
            ValidationException validationEx => (StatusCodes.Status400BadRequest, new ApiErrorDto
            {
                Message = validationEx.Message,
                ErrorCode = validationEx.ErrorCode,
                ValidationErrors = validationEx.ValidationErrors
            }),
            UnauthorizedException unauthorizedEx => (StatusCodes.Status401Unauthorized, new ApiErrorDto
            {
                Message = unauthorizedEx.Message,
                ErrorCode = unauthorizedEx.ErrorCode
            }),
            ForbiddenException forbiddenEx => (StatusCodes.Status403Forbidden, new ApiErrorDto
            {
                Message = forbiddenEx.Message,
                ErrorCode = forbiddenEx.ErrorCode
            }),
           _ => (StatusCodes.Status500InternalServerError, new ApiErrorDto
            {
                Message = "An unexpected error occurred.",
                ErrorCode = "INTERNAL_SERVER_ERROR",
                StackTrace = _env.IsDevelopment() ? exception.StackTrace : null
            })
        };

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(errorDto, cancellationToken);
        
        return true;

    }
}
