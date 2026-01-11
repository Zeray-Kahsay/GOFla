using GoFla.API.DTOs;
using GoFla.API.DTOs.Auth;
using GoFla.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoFla.API.Controllers;

public class AuthController(IAuthService authService) : BaseController
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto, CancellationToken cancellationToken)
    {
        return HandleResult(await authService.RegisterAsync(dto, cancellationToken));
           
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto, CancellationToken cancellationToken)
    {
        return HandleResult(await authService.LoginAsync(dto, cancellationToken));
           
    }


    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto dto, CancellationToken cancellationToken)
    {
        return HandleResult(await authService.RefreshTokenAsync(dto.RefreshToken, cancellationToken));
        
    }

    [Authorize]
    [HttpPost("revoke-token")]
    public async Task<IActionResult> RevokeToken([FromBody] RefreshTokenDto dto, CancellationToken cancellationToken)
    {
        return HandleResult(await authService.RevokeTokenAsync(dto.RefreshToken, cancellationToken));
        
    }


    [HttpPost("external-login")]
    public async Task<IActionResult> ExternalLogin([FromBody] ExternalLoginDto dto, CancellationToken cancellationToken)
    {
        return HandleResult(await authService.ExternalLoginAsync(dto.Provider, dto.AccessToken, cancellationToken));
        
      
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        return HandleResult(await authService.GetCurrentUserAsync(userId, cancellationToken));
        
    }

    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto, CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        return HandleResult(await authService.ChangePasswordAsync(userId, dto.CurrentPassword, dto.NewPassword, cancellationToken));
        
    }
}
