using System;
using System.Security.Claims;

namespace GoFla.API.Services;

public class UserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
{
    public string? UserId =>
     httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    public bool IsAuthenticated => 
        httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
}
