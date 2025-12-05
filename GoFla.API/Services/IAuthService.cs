using System;
using GoFla.API.Commons;
using GoFla.API.DTOs;
using GoFla.API.DTOs.Auth;

namespace GoFla.API.Services;

public interface IAuthService
{
    Task<Result<AuthResponseDto>> RegisterAsync(RegisterDto dto, CancellationToken cancellationToken = default);
    Task<Result<AuthResponseDto>> LoginAsync(LoginDto dto, CancellationToken cancellationToken = default);
    Task<Result<AuthResponseDto>> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task<Result<bool>> RevokeTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task<Result<AuthResponseDto>> ExternalLoginAsync(string provider, string accessToken, CancellationToken cancellationToken = default);
    Task<Result<UserDto>> GetCurrentUserAsync(string userId, CancellationToken cancellationToken = default);
    Task<Result<bool>> ChangePasswordAsync(string userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default);
}
