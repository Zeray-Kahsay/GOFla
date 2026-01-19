using System.Security.Claims;
using GoFla.API.Commons;
using GoFla.API.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace GoFla.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BaseController : ControllerBase
{
    protected string GetUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new UnauthorizedException("User ID not found in token.");

    }

    protected string GetUserEmail()
    {
        return User.FindFirstValue(ClaimTypes.Email)
            ?? throw new UnauthorizedException("User email not found in token.");
    }

    protected IActionResult HandleResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
            return Ok(result.Data);

        if (result.ValidationErrors != null)
        {
            return BadRequest(new ApiErrorResponse(
                result.ErrorCode!,
                result.ErrorMessage!,
                result.ValidationErrors
            ));
        }

        var error = new ApiErrorResponse(
            result.ErrorCode!,
            result.ErrorMessage!,
            result.ValidationErrors
        );

        return result.ErrorCode switch
        {
            "NOT_FOUND" => NotFound(error),
            "UNAUTHORIZED" => Unauthorized(error),
            "FORBIDDEN" => StatusCode(StatusCodes.Status403Forbidden, error),
            "BAD_REQUEST" => BadRequest(error),
            _ => StatusCode(StatusCodes.Status500InternalServerError, error)
        };
    }
}
