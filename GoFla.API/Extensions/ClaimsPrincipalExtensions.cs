using System;
using System.Security.Claims;
using GoFla.API.Exceptions;

namespace GoFla.API.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string? GetUsername(this ClaimsPrincipal user)
    {
       return user.FindFirstValue(ClaimTypes.Name) ?? throw new UnauthorizedException("Cannot get username from token, UNAUTHORIZED");
       
    }

    public static string? GetUserId(this ClaimsPrincipal user)
    {
        return  user.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedException("Cannot get user ID from token, UNAUTHORIZED");
    
    }
}
