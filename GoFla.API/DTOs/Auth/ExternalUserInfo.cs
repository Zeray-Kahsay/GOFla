using System;

namespace GoFla.API.DTOs.Auth;

public record ExternalUserInfo
{
    public string ProviderId { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string? ProfileImageUrl { get; init; }
}
