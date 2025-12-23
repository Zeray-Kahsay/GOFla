using System.Security.Claims;
using GoFla.API.Commons;
using Microsoft.AspNetCore.Mvc;

namespace GoFla.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BaseController : ControllerBase
{
    protected string GetUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("User ID not found");
    }

    protected string? GetUserEmail()
    {
        return User.FindFirstValue(ClaimTypes.Email);
    }

    protected IActionResult HandleResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
        {
            return Ok(result.Data);
        }

        if (result.ValidationErrors != null)
        {
            return BadRequest(new
            {
                message = result.ErrorMessage,
                code = result.ErrorCode,
                errors = result.ValidationErrors
            });
        }

        return result.ErrorCode switch
        {
            "NOT_FOUND" => NotFound(new { message = result.ErrorMessage, code = result.ErrorCode }),
            "UNAUTHORIZED" => Unauthorized(new { message = result.ErrorMessage, code = result.ErrorCode }),
            "FORBIDDEN" => StatusCode(StatusCodes.Status403Forbidden, new { message = result.ErrorMessage, code = result.ErrorCode }),
            _ => BadRequest(new { message = result.ErrorMessage, code = result.ErrorCode })
        };
    }
}
