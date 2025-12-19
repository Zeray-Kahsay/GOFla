using System;

namespace GoFla.API.Domain;

public class RefreshToken
{
    public int Id { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsRevoked { get; set; }
    public string? RevokedByIp { get; set; }
    public DateTime? RevokedAt { get; set; }

    // Navigation
    public User User { get; set; } = null!;
    public string UserId { get; set; } = string.Empty;

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsActive => !IsRevoked && !IsExpired;
}
