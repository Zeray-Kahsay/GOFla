using System.Security.Claims;
using GoFla.API.DTOs;
using GoFla.API.DTOs.Auth;
using GoFla.API.Extensions;
using GoFla.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoFla.API.Controllers;

public class AuthController(IAuthService authService) : BaseController
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto, CancellationToken cancellationToken)
    {
        var result = await authService.RegisterAsync(dto, cancellationToken);
        
        if (!result.IsSuccess)
        {
            return result.ValidationErrors != null
                ? BadRequest(new { Errors = result.ValidationErrors })
                : BadRequest(new { Error = result.ErrorMessage, code = result.ErrorCode });
        }

        return Ok(result.Data);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto, CancellationToken cancellationToken)
    {
        var result = await authService.LoginAsync(dto, cancellationToken);
        
        if (!result.IsSuccess)
        {
            return Unauthorized(new {message = result.ErrorMessage, code = result.ErrorCode});
        }

        return Ok(result.Data);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto dto, CancellationToken cancellationToken)
    {
        var result = await authService.RefreshTokenAsync(dto.RefreshToken, cancellationToken);
        
        if (!result.IsSuccess)
        {
            return Unauthorized(new {message = result.ErrorMessage, code = result.ErrorCode});
        }

        return Ok(result.Data);
    }

    [Authorize]
    [HttpPost("revoke-token")]
    public async Task<IActionResult> RevokeToken([FromBody] RefreshTokenDto dto, CancellationToken cancellationToken)
    {
        var result = await authService.RevokeTokenAsync(dto.RefreshToken, cancellationToken);
        
        if (!result.IsSuccess)
        {
            return BadRequest(new {message = result.ErrorMessage, code = result.ErrorCode});
        }

        return Ok(new { Message = "Refresh token revoked successfully." });
    }

    [HttpPost("external-login")]
    public async Task<IActionResult> ExternalLogin([FromBody] ExternalLoginDto dto, CancellationToken cancellationToken)
    {
        var result = await authService.ExternalLoginAsync(dto.Provider, dto.AccessToken, cancellationToken);
        
        if (!result.IsSuccess)
        {
            return BadRequest(new {message = result.ErrorMessage, code = result.ErrorCode});
        }

        return Ok(result.Data);
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken)
    {
        //var userId = User.FindFirst("sub")?.Value;
        //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userId = User.GetUserId();
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "User ID not found in token." });
        }

        var result = await authService.GetCurrentUserAsync(userId, cancellationToken);
        
        if (!result.IsSuccess)
        {
            return NotFound(new {message = result.ErrorMessage, code = result.ErrorCode});
        }

        return Ok(result.Data);
    }

    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto, CancellationToken cancellationToken)
    {
        //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userId = User.GetUserId();
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "User ID not found in token." });
        }

        var result = await authService.ChangePasswordAsync(userId, dto.CurrentPassword, dto.NewPassword, cancellationToken);
        
        if (!result.IsSuccess)
        {
            return result.ValidationErrors != null
                ? BadRequest(new {errors = result.ValidationErrors})
                : BadRequest(new {message = result.ErrorMessage, code = result.ErrorCode});

         }
      

        return Ok(new { Message = "Password changed successfully." });
    }
}
