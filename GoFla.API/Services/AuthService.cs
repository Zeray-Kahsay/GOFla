using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using GoFla.API.Commons;
using GoFla.API.Configuration;
using GoFla.API.Data;
using GoFla.API.Domain;
using GoFla.API.DTOs;
using GoFla.API.DTOs.Auth;
using GoFla.API.Extensions;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace GoFla.API.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly JwtSettings _jwtSettings;
    private readonly AppDbContext _context;
    private readonly IExternalAuthService _externalAuthService;

    public AuthService(
      UserManager<User> userManager,
      SignInManager<User> signInManager,
      IOptions<JwtSettings> jwtSettings,
      AppDbContext context,
      IExternalAuthService externalAuthService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtSettings = jwtSettings.Value;
        _context = context;
        _externalAuthService = externalAuthService;
    }

    public async Task<Result<AuthResponseDto>> RegisterAsync(RegisterDto dto, CancellationToken cancellationToken = default)
    {
        var existingUser = await _userManager.FindByEmailAsync(dto.Email);
        if (existingUser is not null)
        {
            return Result<AuthResponseDto>.Failure("User with this email already exists.", "EMAIL_ALREADY_EXISTS"); 
        }

        var user = new User
        {
            UserName = dto.Email,
            Email = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
        {
            var errors = result.Errors.ToDictionary(e => e.Code, e => new[]{e.Description});
            return Result<AuthResponseDto>.ValidationFailure(errors);
        }

        // create cart for the user
        var cart = new Cart { UserId = user.Id };
        _context.Carts.Add(cart);
        await _context.SaveChangesAsync(cancellationToken);

        var tokens = await GenerateTokensAsync(user);

        return Result<AuthResponseDto>.Success(new AuthResponseDto
        {
            User = user.ToUserDto(),
            Token = tokens.accessToken,
            RefreshToken = tokens.refreshToken
        });
    }
    public async Task<Result<AuthResponseDto>> LoginAsync(LoginDto dto, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user is null)
        {
            return Result<AuthResponseDto>.Failure("Invalid email or password.", "INVALID_CREDENTIALS");
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
        if (!result.Succeeded)
        {
            return Result<AuthResponseDto>.Failure("Invalid email or password.", "INVALID_CREDENTIALS");
        }

        var tokens = await GenerateTokensAsync(user);

        return Result<AuthResponseDto>.Success(new AuthResponseDto
        {
            User = user.ToUserDto(),
            Token = tokens.accessToken,
            RefreshToken = tokens.refreshToken
        });
    }

    public async Task<Result<AuthResponseDto>> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var storedToken = await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken, cancellationToken);
        
        if (storedToken is null || !storedToken.IsActive)
        {
            return Result<AuthResponseDto>.Failure("Invalid or expired refresh token.", "INVALID_REFRESH_TOKEN");
        }

        // Revoke the old token
        storedToken.IsRevoked = true;
        storedToken.RevokedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        // Generate new tokens
        var tokens = await GenerateTokensAsync(storedToken.User);

        return Result<AuthResponseDto>.Success(new AuthResponseDto
        {
            User = storedToken.User.ToUserDto(),
            Token = tokens.accessToken,
            RefreshToken = tokens.refreshToken
        });
    }
    public async Task<Result<bool>> RevokeTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var storedToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken, cancellationToken);
        
        if (storedToken is null )
        {
            return Result<bool>.Failure("Refresh token not found.", "REFRESH_TOKEN_NOT_FOUND");
        }

        storedToken.IsRevoked = true;
        storedToken.RevokedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
    public async Task<Result<AuthResponseDto>> ExternalLoginAsync(string provider, string accessToken, CancellationToken cancellationToken = default)
    {
        var userInfo = await _externalAuthService.GetUserInfoAsync(provider, accessToken);
        if (userInfo is null)
        {
            return Result<AuthResponseDto>.Failure("Failed to get user info from provider.", "INVALID_EXTERNAL_AUTH");
        }

        var user = await _userManager.FindByEmailAsync(userInfo.Email);
        if (user is null)
        {
            // Register new user
            user = new User
            {
                UserName = userInfo.Email,
                Email = userInfo.Email,
                FirstName = userInfo.FirstName,
                LastName = userInfo.LastName,
                ProfileImageUrl = userInfo.ProfileImageUrl,
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                var errors = result.Errors.ToDictionary(e => e.Code, e => new[] { e.Description });
                return Result<AuthResponseDto>.Failure("Failed to create user.", "USER_CREATION_FAILED");
            }

            // create cart for the user
            var cart = new Cart { UserId = user.Id };
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync(cancellationToken);

            // Add external login info
            var loginResult = await _userManager.AddLoginAsync(user, new UserLoginInfo(provider, userInfo.ProviderId, provider));
            if (!loginResult.Succeeded)
            {
                return Result<AuthResponseDto>.Failure("Failed to link external login.", "EXTERNAL_LOGIN_LINK_FAILED");
            }
        }
        else
        {
            // Check if the external login is already linked
            var existingLogin = await _userManager.FindByLoginAsync(provider, userInfo.ProviderId);
            if (existingLogin is null)
            {
                var loginResult = await _userManager.AddLoginAsync(user, new UserLoginInfo(provider, userInfo.ProviderId, provider));
                if (!loginResult.Succeeded)
                {
                    return Result<AuthResponseDto>.Failure("Failed to link external login.", "EXTERNAL_LOGIN_LINK_FAILED");
                }
            }
        }

        var tokens = await GenerateTokensAsync(user);

        return Result<AuthResponseDto>.Success(new AuthResponseDto
        {
            User = user.ToUserDto(),
            Token = tokens.accessToken,
            RefreshToken = tokens.refreshToken
        });
    }
    public async Task<Result<bool>> ChangePasswordAsync(string userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return Result<bool>.Failure("User not found.", "USER_NOT_FOUND");
        }

        var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        if (!result.Succeeded)
        {
            var errors = result.Errors.ToDictionary(e => e.Code, e => new[] { e.Description });
            return Result<bool>.ValidationFailure(errors);
        }

        return Result<bool>.Success(true);
    }


    public async Task<Result<UserDto>> GetCurrentUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return Result<UserDto>.Failure("User not found.", "USER_NOT_FOUND");
        }
        return Result<UserDto>.Success(user.ToUserDto());
    }


    // Helper method to generate JWT and Refresh Token
    private async Task<(string accessToken, string refreshToken)> GenerateTokensAsync(User user)
    {
      var accessToken = GenerateJwtToken(user);
      var refreshToken = GenerateRefreshToken();

      // Store refresh token
      var refreshTokenEntity = new RefreshToken
      {
          UserId = user.Id,
          Token = refreshToken,
          ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationInDays),
          CreatedAt = DateTime.UtcNow
      };

        _context.RefreshTokens.Add(refreshTokenEntity);
        await _context.SaveChangesAsync();
        return (accessToken, refreshToken);
    }

    private string GenerateJwtToken(User user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}")
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }



}
