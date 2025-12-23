using System;

namespace GoFla.API.Domain;

public class ReviewResponse
{
    public int Id { get; set; }
    public string ResponseText { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Review Review { get; set; } = null!;
    public int ReviewId { get; set; }
    public User Responder { get; set; } = null!;
    public string ResponderId { get; set; } = string.Empty; // Restaurant owner/admin
}
